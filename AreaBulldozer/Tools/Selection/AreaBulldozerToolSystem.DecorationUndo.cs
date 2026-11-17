using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Area = Game.Areas.Area;
using AreaNode = Game.Areas.Node;
using AreaSpace = Game.Areas.Space;
using AreaSurface = Game.Areas.Surface;
using ObjectElevation = Game.Objects.Elevation;
using ObjectStack = Game.Objects.Stack;
using ObjectChild = Game.Objects.SubObject;
using ObjectTransform = Game.Objects.Transform;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private struct DecorationSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public ObjectTransform Transform;
            public ObjectElevation? Elevation;
            public PseudoRandomSeed? Seed;
            public ObjectStack? Stack;
            public Quantity? Quantity;
            public CustomMeshColor[] Colors;
            public Entity Parent;
            public Entity Owner;
            public PrefabRef? ParentPrefab;
            public ObjectTransform? ParentTransform;
            public Relative? Relative;
            public Attached? Attachment;
            public bool WasInParentBuffer;
        }

        private struct SurfaceSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public Area Area;
            public AreaNode[] Nodes;
            public bool IsSurface;
        }

        private readonly List<DecorationSnapshot> m_DecorationUndo = new();
        private readonly List<SurfaceSnapshot> m_SurfaceUndo = new();

        private void RecordDecorationForUndo(Entity entity)
        {
            if ((EntityManager.HasBuffer<ObjectChild>(entity) &&
                 EntityManager.GetBuffer<ObjectChild>(entity, true).Length > 0) ||
                !EntityManager.HasComponent<ObjectTransform>(entity) ||
                !TryGetUndoPrefab<ObjectData>(entity, out Entity prefab))
            {
                return;
            }

            Entity owner = ReadUndoComponent<Owner>(entity)?.m_Owner ?? Entity.Null;
            Attached? attachment = ReadUndoComponent<Attached>(entity);
            Entity attachmentParent = attachment?.m_Parent ?? Entity.Null;
            bool hasIndexedParent = m_SpatialLinkedProps.TryGetValue(entity, out Entity indexedParent);
            Entity parent = owner != Entity.Null ? owner : attachmentParent;
            if (parent == Entity.Null)
            {
                parent = indexedParent;
            }

            if ((hasIndexedParent && (indexedParent == Entity.Null || indexedParent != parent)) ||
                (parent != Entity.Null && !CanRestoreDecorationParent(parent)) ||
                (attachmentParent != Entity.Null && !CanRestoreDecorationParent(attachmentParent)))
            {
                return;
            }

            CustomMeshColor[] colors = null;
            if (EntityManager.HasBuffer<CustomMeshColor>(entity) &&
                EntityManager.IsComponentEnabled<CustomMeshColor>(entity))
            {
                colors = CopyUndoBuffer<CustomMeshColor>(entity);
            }

            m_DecorationUndo.Add(new DecorationSnapshot
            {
                Original = entity,
                Prefab = prefab,
                Transform = EntityManager.GetComponentData<ObjectTransform>(entity),
                Elevation = ReadUndoComponent<ObjectElevation>(entity),
                Seed = ReadUndoComponent<PseudoRandomSeed>(entity),
                Stack = ReadUndoComponent<ObjectStack>(entity),
                Quantity = ReadUndoComponent<Quantity>(entity),
                Colors = colors,
                Parent = parent,
                Owner = owner,
                ParentPrefab = parent == Entity.Null ? null : ReadUndoComponent<PrefabRef>(parent),
                ParentTransform = parent == Entity.Null ? null : ReadUndoComponent<ObjectTransform>(parent),
                Relative = ReadUndoComponent<Relative>(entity),
                Attachment = attachmentParent == Entity.Null ? null : attachment,
                WasInParentBuffer = FindDecorationChild(parent, entity) >= 0
            });
        }

        private bool CanRestoreDecorationParent(Entity parent)
        {
            return IsEntityUsable(parent, allowOverridden: true) && !m_PendingDeletion.Contains(parent);
        }

        private int FindDecorationChild(Entity parent, Entity child)
        {
            if (parent != Entity.Null && EntityManager.HasBuffer<ObjectChild>(parent))
            {
                DynamicBuffer<ObjectChild> children = EntityManager.GetBuffer<ObjectChild>(parent, true);
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i].m_SubObject == child)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void RecordSurfaceForUndo(Entity entity)
        {
            if (EntityManager.HasComponent<Owner>(entity) ||
                EntityManager.HasComponent<Game.Areas.Lot>(entity) ||
                (EntityManager.HasBuffer<Game.Areas.SubArea>(entity) &&
                 EntityManager.GetBuffer<Game.Areas.SubArea>(entity, true).Length > 0) ||
                !EntityManager.HasComponent<Area>(entity) ||
                !EntityManager.HasBuffer<AreaNode>(entity) ||
                !TryGetUndoPrefab<AreaData>(entity, out Entity prefab))
            {
                return;
            }

            if (EntityManager.GetBuffer<AreaNode>(entity, true).Length < 3)
            {
                return;
            }

            m_SurfaceUndo.Add(new SurfaceSnapshot
            {
                Original = entity,
                Prefab = prefab,
                Area = EntityManager.GetComponentData<Area>(entity),
                Nodes = CopyUndoBuffer<AreaNode>(entity),
                IsSurface = EntityManager.HasComponent<AreaSurface>(entity)
            });
        }

        private bool TryGetUndoPrefab<T>(Entity entity, out Entity prefab)
            where T : unmanaged, IComponentData
        {
            prefab = ReadUndoComponent<PrefabRef>(entity)?.m_Prefab ?? Entity.Null;
            return IsEntityUsable(prefab) && EntityManager.HasComponent<T>(prefab);
        }

        private T[] CopyUndoBuffer<T>(Entity entity) where T : unmanaged, IBufferElementData
        {
            DynamicBuffer<T> buffer = EntityManager.GetBuffer<T>(entity, true);
            T[] copy = new T[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                copy[i] = buffer[i];
            }
            return copy;
        }

        private bool RestoreDecoration(DecorationSnapshot saved, HashSet<Entity> restoredChildren)
        {
            if (IsEntityUsable(saved.Original, allowOverridden: true) ||
                !IsEntityUsable(saved.Prefab) || !EntityManager.HasComponent<ObjectData>(saved.Prefab))
            {
                return false;
            }

            ObjectTransform placement = saved.Transform;
            if (saved.Parent != Entity.Null)
            {
                if (!CanRestoreDecorationParent(saved.Parent) ||
                    (saved.ParentPrefab is PrefabRef expected &&
                     ReadUndoComponent<PrefabRef>(saved.Parent)?.m_Prefab != expected.m_Prefab))
                {
                    return false;
                }

                if (saved.ParentTransform is ObjectTransform previous &&
                    ReadUndoComponent<ObjectTransform>(saved.Parent) is ObjectTransform current)
                {
                    quaternion rotationDelta = math.mul(current.m_Rotation, math.inverse(previous.m_Rotation));
                    placement.m_Position = current.m_Position +
                        math.rotate(rotationDelta, placement.m_Position - previous.m_Position);
                    placement.m_Rotation = math.mul(rotationDelta, placement.m_Rotation);
                }

                if (HasRegeneratedDecoration(saved, placement, restoredChildren))
                {
                    return false;
                }
            }

            if (saved.Attachment is Attached savedAttachment && !CanRestoreDecorationParent(savedAttachment.m_Parent))
            {
                return false;
            }

            EntityArchetype archetype = EntityManager.GetComponentData<ObjectData>(saved.Prefab).m_Archetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return false;
            }

            using NativeArray<Entity> created = EntityManager.CreateEntity(archetype, 1, Allocator.Temp);
            Entity entity = created[0];
            if (!EntityManager.HasComponent<PrefabRef>(entity) ||
                !EntityManager.HasComponent<ObjectTransform>(entity))
            {
                EntityManager.DestroyEntity(entity);
                return false;
            }

            try
            {
                EntityManager.SetComponentData(entity, new PrefabRef(saved.Prefab));
                EntityManager.SetComponentData(entity, placement);
                if (saved.Elevation is ObjectElevation elevation)
                {
                    SetUndoComponent(entity, elevation);
                }
                if (saved.Seed is PseudoRandomSeed seed && EntityManager.HasComponent<PseudoRandomSeed>(entity))
                {
                    EntityManager.SetComponentData(entity, seed);
                }
                if (saved.Stack is ObjectStack stack && EntityManager.HasComponent<ObjectStack>(entity))
                {
                    EntityManager.SetComponentData(entity, stack);
                }
                if (saved.Quantity is Quantity quantity && EntityManager.HasComponent<Quantity>(entity))
                {
                    EntityManager.SetComponentData(entity, quantity);
                }
                if (saved.Colors != null && EntityManager.HasBuffer<CustomMeshColor>(entity))
                {
                    DynamicBuffer<CustomMeshColor> colors = EntityManager.GetBuffer<CustomMeshColor>(entity);
                    colors.Clear();
                    foreach (CustomMeshColor color in saved.Colors)
                    {
                        colors.Add(color);
                    }
                    EntityManager.SetComponentEnabled<CustomMeshColor>(entity, true);
                }
                if (saved.Owner != Entity.Null)
                {
                    SetUndoComponent(entity, new Owner { m_Owner = saved.Owner });
                }
                if (saved.Relative is Relative relative)
                {
                    SetUndoComponent(entity, relative);
                }
                if (saved.Attachment is Attached attachment)
                {
                    attachment.m_OldParent = Entity.Null;
                    SetUndoComponent(entity, attachment);
                    EntityManager.AddComponent<Created>(entity);
                }
                EntityManager.AddComponent<Updated>(entity);
                EntityManager.AddComponent<BatchesUpdated>(entity);

                if (saved.WasInParentBuffer)
                {
                    int oldIndex = FindDecorationChild(saved.Parent, saved.Original);
                    DynamicBuffer<ObjectChild> children = EntityManager.HasBuffer<ObjectChild>(saved.Parent)
                        ? EntityManager.GetBuffer<ObjectChild>(saved.Parent)
                        : EntityManager.AddBuffer<ObjectChild>(saved.Parent);
                    ObjectChild entry = new ObjectChild { m_SubObject = entity };
                    if (oldIndex >= 0)
                    {
                        children[oldIndex] = entry;
                    }
                    else
                    {
                        children.Add(entry);
                    }
                    restoredChildren.Add(entity);
                }

                return true;
            }
            catch
            {
                EntityManager.DestroyEntity(entity);
                throw;
            }
        }

        private bool HasRegeneratedDecoration(
            DecorationSnapshot saved, ObjectTransform placement, HashSet<Entity> restoredChildren)
        {
            if (!EntityManager.HasBuffer<ObjectChild>(saved.Parent))
            {
                return false;
            }

            DynamicBuffer<ObjectChild> children = EntityManager.GetBuffer<ObjectChild>(saved.Parent, true);
            for (int i = 0; i < children.Length; i++)
            {
                Entity child = children[i].m_SubObject;
                if (child == saved.Original || restoredChildren.Contains(child) ||
                    !IsEntityUsable(child, allowOverridden: true) ||
                    ReadUndoComponent<PrefabRef>(child)?.m_Prefab != saved.Prefab ||
                    !(ReadUndoComponent<ObjectTransform>(child) is ObjectTransform existing))
                {
                    continue;
                }

                if (math.distancesq(existing.m_Position, placement.m_Position) < 0.0001f &&
                    math.abs(math.dot(existing.m_Rotation.value, placement.m_Rotation.value)) > 0.9999f)
                {
                    return true;
                }
            }

            return false;
        }

        private bool RestoreSurface(SurfaceSnapshot saved)
        {
            if (IsEntityUsable(saved.Original, allowOverridden: true) ||
                !IsEntityUsable(saved.Prefab) || !EntityManager.HasComponent<AreaData>(saved.Prefab))
            {
                return false;
            }

            EntityArchetype archetype = EntityManager.GetComponentData<AreaData>(saved.Prefab).m_Archetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return false;
            }

            using NativeArray<Entity> created = EntityManager.CreateEntity(archetype, 1, Allocator.Temp);
            Entity entity = created[0];
            bool matchingKind = saved.IsSurface
                ? EntityManager.HasComponent<AreaSurface>(entity)
                : EntityManager.HasComponent<AreaSpace>(entity);
            if (!matchingKind || !EntityManager.HasComponent<PrefabRef>(entity) ||
                !EntityManager.HasComponent<Area>(entity) || !EntityManager.HasBuffer<AreaNode>(entity))
            {
                EntityManager.DestroyEntity(entity);
                return false;
            }

            try
            {
                EntityManager.SetComponentData(entity, new PrefabRef(saved.Prefab));
                EntityManager.SetComponentData(entity, saved.Area);
                DynamicBuffer<AreaNode> nodes = EntityManager.GetBuffer<AreaNode>(entity);
                foreach (AreaNode node in saved.Nodes)
                {
                    nodes.Add(node);
                }
                EntityManager.AddComponent<Created>(entity);
                EntityManager.AddComponent<Updated>(entity);
                return true;
            }
            catch
            {
                EntityManager.DestroyEntity(entity);
                throw;
            }
        }
    }
}
