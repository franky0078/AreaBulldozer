using Game.Common;
using Game.Objects;
using Game.Prefabs;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using SubObject = Game.Objects.SubObject;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private struct VegetationSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public Transform Transform;
            public Plant Plant;
            public Tree? Tree;
            public Elevation? Elevation;
            public PseudoRandomSeed? Seed;
            public Entity Parent;
            public bool HadOwner;
            public SubObject? ParentEntry;
            public Relative? Relative;
            public Attached? Attachment;
            public Transform? ParentTransform;
            public PrefabRef? ParentPrefab;
        }

        private readonly List<VegetationSnapshot> m_VegetationUndo = new();
        private bool m_DeletionUndoRequested;

        internal int DeletionUndoCount => m_VegetationUndo.Count + m_BuildingUndo.Count + m_CarUndo.Count +
            m_DecorationUndo.Count + m_SurfaceUndo.Count + m_NetworkUndo.Count;

        private void ClearDeletionUndo()
        {
            m_VegetationUndo.Clear();
            m_BuildingUndo.Clear();
            m_CarUndo.Clear();
            m_DecorationUndo.Clear();
            m_SurfaceUndo.Clear();
            m_NetworkUndo.Clear();
            m_NetworkUndoNodes.Clear();
            ClearNetworkTransitUndo();
        }

        internal void RequestDeletionUndo()
        {
            if (IsToolActive && DeletionUndoCount > 0)
            {
                m_DeletionUndoRequested = true;
            }
        }

        private void RecordVegetationForUndo(Entity entity)
        {
            if (!EntityManager.HasComponent<Plant>(entity) ||
                !EntityManager.HasComponent<PrefabRef>(entity) ||
                !EntityManager.HasComponent<Transform>(entity))
            {
                return;
            }

            Entity prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
            if (!IsEntityUsable(prefab) || !EntityManager.HasComponent<ObjectData>(prefab))
            {
                return;
            }

            bool hadOwner = EntityManager.HasComponent<Owner>(entity);
            Entity parent = hadOwner
                ? EntityManager.GetComponentData<Owner>(entity).m_Owner
                : Entity.Null;
            bool hasIndexedParent = m_SpatialAssetVegetation.TryGetValue(entity, out Entity indexedParent);
            if (!hadOwner && hasIndexedParent)
            {
                parent = indexedParent;
            }

            if ((hadOwner || hasIndexedParent) && !IsRestorableVegetationParent(parent))
            {
                return;
            }

            Attached? attachment = ReadUndoComponent<Attached>(entity);
            if (attachment is Attached attached && (parent == Entity.Null || attached.m_Parent != parent))
            {
                return;
            }

            SubObject? parentEntry = null;
            if (parent != Entity.Null && EntityManager.HasBuffer<SubObject>(parent))
            {
                DynamicBuffer<SubObject> children = EntityManager.GetBuffer<SubObject>(parent, true);
                for (int index = 0; index < children.Length; index++)
                {
                    if (children[index].m_SubObject == entity)
                    {
                        parentEntry = children[index];
                        break;
                    }
                }
            }

            if (hasIndexedParent && !parentEntry.HasValue)
            {
                return;
            }

            m_VegetationUndo.Add(new VegetationSnapshot
            {
                Original = entity,
                Prefab = prefab,
                Transform = EntityManager.GetComponentData<Transform>(entity),
                Plant = EntityManager.GetComponentData<Plant>(entity),
                Tree = ReadUndoComponent<Tree>(entity),
                Elevation = ReadUndoComponent<Elevation>(entity),
                Seed = ReadUndoComponent<PseudoRandomSeed>(entity),
                Parent = parent,
                HadOwner = hadOwner,
                ParentEntry = parentEntry,
                Relative = ReadUndoComponent<Relative>(entity),
                Attachment = attachment,
                ParentTransform = parent == Entity.Null ? null : ReadUndoComponent<Transform>(parent),
                ParentPrefab = parent == Entity.Null ? null : ReadUndoComponent<PrefabRef>(parent)
            });
        }

        private bool IsRestorableVegetationParent(Entity parent)
        {
            bool foundBuilding = false;
            for (int depth = 0; depth < 8; depth++)
            {
                if (!IsEntityUsable(parent, allowOverridden: true) ||
                    m_PendingDeletion.Contains(parent) ||
                    IsNetworkEntity(parent))
                {
                    return false;
                }

                foundBuilding |= IsBuildingEntity(parent) ||
                    EntityManager.HasComponent<Game.Buildings.Park>(parent);
                if (!EntityManager.HasComponent<Owner>(parent))
                {
                    return foundBuilding;
                }

                parent = EntityManager.GetComponentData<Owner>(parent).m_Owner;
            }

            return false;
        }

        private T? ReadUndoComponent<T>(Entity entity)
            where T : unmanaged, IComponentData
        {
            return EntityManager.HasComponent<T>(entity)
                ? EntityManager.GetComponentData<T>(entity)
                : (T?)null;
        }

        private bool ProcessDeletionUndo()
        {
            if (!m_DeletionUndoRequested)
            {
                return false;
            }

            // Wait for the tool's deletion commands to be applied before adding replacements.
            if (IsNetworkDeletionPending())
            {
                return true;
            }

            foreach (BuildingSnapshot snapshot in m_BuildingUndo)
            {
                if (m_PendingDeletion.Contains(snapshot.Original))
                {
                    return true;
                }
            }

            foreach (VegetationSnapshot snapshot in m_VegetationUndo)
            {
                if (m_PendingDeletion.Contains(snapshot.Original))
                {
                    return true;
                }
            }

            foreach (CarSnapshot snapshot in m_CarUndo)
            {
                if (m_PendingDeletion.Contains(snapshot.Original) ||
                    (snapshot.Trailers != null &&
                     snapshot.Trailers.Exists(trailer => m_PendingDeletion.Contains(trailer.Original))))
                {
                    return true;
                }
            }

            if (m_DecorationUndo.Exists(snapshot => m_PendingDeletion.Contains(snapshot.Original)) ||
                m_SurfaceUndo.Exists(snapshot => m_PendingDeletion.Contains(snapshot.Original)))
            {
                return true;
            }

            m_DeletionUndoRequested = false;
            ResetContinuousDeleteState();
            CancelLargeSelectionConfirmation();

            int restored = 0;
            int skipped = 0;
            HashSet<Entity> restoredChildren = new();
            try
            {
                Dictionary<Entity, Entity> restoredNetworks = RestoreNetworksForUndo(ref restored, ref skipped);
                if (m_BuildingUndo.Count > 0)
                {
                    HashSet<float3> occupied = GetOccupiedBuildingPositions();
                    while (m_BuildingUndo.Count > 0)
                    {
                        int last = m_BuildingUndo.Count - 1;
                        BuildingSnapshot snapshot = m_BuildingUndo[last];
                        if (restoredNetworks.TryGetValue(snapshot.OriginalRoad, out Entity restoredRoad))
                        {
                            snapshot.OriginalRoad = restoredRoad;
                        }
                        if (!occupied.Contains(snapshot.Transform.m_Position) && RestoreBuilding(snapshot))
                        {
                            occupied.Add(snapshot.Transform.m_Position);
                            restored++;
                        }
                        else
                        {
                            skipped++;
                        }

                        m_BuildingUndo.RemoveAt(last);
                    }
                }

                while (m_SurfaceUndo.Count > 0)
                {
                    int last = m_SurfaceUndo.Count - 1;
                    if (RestoreSurface(m_SurfaceUndo[last]))
                    {
                        restored++;
                    }
                    else
                    {
                        skipped++;
                    }
                    m_SurfaceUndo.RemoveAt(last);
                }

                while (m_DecorationUndo.Count > 0)
                {
                    int last = m_DecorationUndo.Count - 1;
                    if (RestoreDecoration(m_DecorationUndo[last], restoredChildren))
                    {
                        restored++;
                    }
                    else
                    {
                        skipped++;
                    }
                    m_DecorationUndo.RemoveAt(last);
                }

                while (m_VegetationUndo.Count > 0)
                {
                    int last = m_VegetationUndo.Count - 1;
                    VegetationSnapshot snapshot = m_VegetationUndo[last];

                    if (RestoreVegetation(snapshot, restoredChildren))
                    {
                        restored++;
                    }
                    else
                    {
                        skipped++;
                    }

                    m_VegetationUndo.RemoveAt(last);
                }

                while (m_CarUndo.Count > 0)
                {
                    int last = m_CarUndo.Count - 1;
                    if (RestoreCar(m_CarUndo[last]))
                    {
                        restored++;
                    }
                    else
                    {
                        skipped++;
                    }

                    m_CarUndo.RemoveAt(last);
                }
            }
            finally
            {
                if (restored > 0)
                {
                    ClearSelectionPreview();
                    RequestSpatialIndexSync();
                }
            }

            SafeLogInfo($"Deletion undo: restored {restored}, skipped {skipped}.");
            return true;
        }

        private void SetUndoComponent<T>(Entity entity, T component)
            where T : unmanaged, IComponentData
        {
            if (EntityManager.HasComponent<T>(entity))
            {
                EntityManager.SetComponentData(entity, component);
            }
            else
            {
                EntityManager.AddComponentData(entity, component);
            }
        }

        private bool RestoreVegetation(VegetationSnapshot snapshot, HashSet<Entity> restoredChildren)
        {
            if (IsEntityUsable(snapshot.Original, allowOverridden: true) ||
                !IsEntityUsable(snapshot.Prefab) ||
                !EntityManager.HasComponent<ObjectData>(snapshot.Prefab))
            {
                return false;
            }

            Transform placement = snapshot.Transform;
            int parentEntryIndex = -1;
            if (snapshot.Parent != Entity.Null)
            {
                if (!IsRestorableVegetationParent(snapshot.Parent) ||
                    (snapshot.ParentPrefab is PrefabRef parentPrefab &&
                     (!EntityManager.HasComponent<PrefabRef>(snapshot.Parent) ||
                      EntityManager.GetComponentData<PrefabRef>(snapshot.Parent).m_Prefab != parentPrefab.m_Prefab)))
                {
                    return false;
                }

                if (snapshot.ParentTransform is Transform previousParent &&
                    EntityManager.HasComponent<Transform>(snapshot.Parent))
                {
                    Transform currentParent = EntityManager.GetComponentData<Transform>(snapshot.Parent);
                    quaternion rotationDelta = math.mul(currentParent.m_Rotation, math.inverse(previousParent.m_Rotation));
                    placement.m_Position = currentParent.m_Position +
                        math.rotate(rotationDelta, placement.m_Position - previousParent.m_Position);
                    placement.m_Rotation = math.mul(rotationDelta, placement.m_Rotation);
                }

                if (snapshot.ParentEntry.HasValue &&
                    EntityManager.HasBuffer<SubObject>(snapshot.Parent))
                {
                    DynamicBuffer<SubObject> children = EntityManager.GetBuffer<SubObject>(snapshot.Parent, true);
                    for (int index = 0; index < children.Length; index++)
                    {
                        Entity child = children[index].m_SubObject;
                        if (child == snapshot.Original)
                        {
                            parentEntryIndex = index;
                        }
                        else if (!restoredChildren.Contains(child) &&
                                 IsEntityUsable(child, allowOverridden: true) &&
                                 EntityManager.HasComponent<Plant>(child) &&
                                 EntityManager.HasComponent<PrefabRef>(child) &&
                                 EntityManager.GetComponentData<PrefabRef>(child).m_Prefab == snapshot.Prefab &&
                                 EntityManager.HasComponent<Transform>(child))
                        {
                            Transform existing = EntityManager.GetComponentData<Transform>(child);
                            if (math.distancesq(existing.m_Position, placement.m_Position) < 0.0001f &&
                                math.abs(math.dot(existing.m_Rotation.value, placement.m_Rotation.value)) > 0.9999f)
                            {
                                // The game or another mod already regenerated this child.
                                return false;
                            }
                        }
                    }
                }
            }

            EntityArchetype archetype =
                EntityManager.GetComponentData<ObjectData>(snapshot.Prefab).m_Archetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return false;
            }

            using NativeArray<Entity> plants = EntityManager.CreateEntity(archetype, 1, Allocator.Temp);
            Entity plant = plants[0];

            if (!EntityManager.HasComponent<Plant>(plant) ||
                (snapshot.Tree.HasValue && !EntityManager.HasComponent<Tree>(plant)))
            {
                EntityManager.DestroyEntity(plant);
                return false;
            }

            try
            {
                EntityManager.SetComponentData(plant, new PrefabRef(snapshot.Prefab));
                EntityManager.SetComponentData(plant, placement);
                EntityManager.SetComponentData(plant, snapshot.Plant);

                if (snapshot.Tree is Tree tree)
                {
                    EntityManager.SetComponentData(plant, tree);
                }

                if (snapshot.Elevation is Elevation elevation)
                {
                    SetUndoComponent(plant, elevation);
                }

                if (snapshot.Seed is PseudoRandomSeed seed &&
                    EntityManager.HasComponent<PseudoRandomSeed>(plant))
                {
                    EntityManager.SetComponentData(plant, seed);
                }

                if (snapshot.HadOwner)
                {
                    SetUndoComponent(plant, new Owner { m_Owner = snapshot.Parent });
                }

                if (snapshot.Relative is Relative relative)
                {
                    SetUndoComponent(plant, relative);
                }

                if (snapshot.Attachment is Attached attached)
                {
                    SetUndoComponent(plant, attached);
                }

                EntityManager.AddComponent<Updated>(plant);
                EntityManager.AddComponent<BatchesUpdated>(plant);

                if (snapshot.ParentEntry is SubObject entry)
                {
                    entry.m_SubObject = plant;
                    DynamicBuffer<SubObject> children = EntityManager.HasBuffer<SubObject>(snapshot.Parent)
                        ? EntityManager.GetBuffer<SubObject>(snapshot.Parent)
                        : EntityManager.AddBuffer<SubObject>(snapshot.Parent);

                    if (parentEntryIndex >= 0)
                    {
                        children[parentEntryIndex] = entry;
                    }
                    else
                    {
                        children.Add(entry);
                    }

                    restoredChildren.Add(plant);
                }

                return true;
            }
            catch
            {
                EntityManager.DestroyEntity(plant);
                throw;
            }
        }
    }
}
