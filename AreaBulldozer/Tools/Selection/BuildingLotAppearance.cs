using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Area = Game.Areas.Area;
using AreaFlags = Game.Areas.AreaFlags;
using AreaNode = Game.Areas.Node;
using AreaSurface = Game.Areas.Surface;
using AreaLot = Game.Areas.Lot;
using BuildingLot = Game.Buildings.Lot;
using Plant = Game.Objects.Plant;
using Tree = Game.Objects.Tree;
using Attachment = Game.Objects.Attached;
using Elevation = Game.Objects.Elevation;
using Relative = Game.Objects.Relative;
using SubArea = Game.Areas.SubArea;
using SubObject = Game.Objects.SubObject;
using SubNet = Game.Net.SubNet;
using NetNode = Game.Net.Node;
using NetCurve = Game.Net.Curve;
using NetEdge = Game.Net.Edge;
using NetElevation = Game.Net.Elevation;
using Transform = Game.Objects.Transform;

namespace AreaBulldozer.Tools
{
    internal sealed class BuildingLotAppearance
    {
        private struct SurfaceSnapshot
        {
            public Entity Prefab;
            public AreaNode[] Nodes;
            public AreaFlags Flags;
        }

        private struct DecorationSnapshot
        {
            public Entity Prefab;
            public Transform Transform;
            public Relative? Relative;
            public Elevation? Elevation;
            public PseudoRandomSeed? Seed;
            public Attachment? Attachment;
            public SubObject Entry;
            public Plant? Plant;
            public Tree? Tree;
        }

        private struct NetworkHeight
        {
            public Entity Prefab;
            public NetNode? Node;
            public NetCurve? Curve;
            public NetElevation? Elevation;
        }

        private Transform m_Transform;
        private BuildingLot? m_Lot;
        private Elevation? m_Elevation;
        private readonly List<SurfaceSnapshot> m_Surfaces = new();
        private readonly List<DecorationSnapshot> m_Decoration = new();
        private readonly List<NetworkHeight> m_Network = new();

        internal string DescribeSnapshot() =>
            $"Saved {m_Surfaces.Count} lot areas, {m_Decoration.Count} decorations, {m_Network.Count} network elements at height {m_Transform.m_Position.y:0.00}";

        internal static BuildingLotAppearance Capture(EntityManager manager, Entity building)
        {
            var result = new BuildingLotAppearance
            {
                m_Transform = manager.GetComponentData<Transform>(building),
                m_Lot = Read<BuildingLot>(manager, building),
                m_Elevation = Read<Elevation>(manager, building)
            };
            if (manager.HasBuffer<SubArea>(building))
            {
                foreach (SubArea subArea in manager.GetBuffer<SubArea>(building, true))
                {
                    Entity entity = subArea.m_Area;
                    if (!TryGetPrefab(manager, entity, out Entity prefab) ||
                        (!manager.HasComponent<AreaSurface>(entity) && !manager.HasComponent<AreaLot>(entity)) ||
                        !manager.HasComponent<Area>(entity) ||
                        !manager.HasComponent<AreaData>(prefab) ||
                        !manager.HasBuffer<AreaNode>(entity))
                    {
                        continue;
                    }

                    DynamicBuffer<AreaNode> nodes = manager.GetBuffer<AreaNode>(entity, true);
                    if (nodes.Length >= 3)
                    {
                        var copy = new AreaNode[nodes.Length];
                        for (int index = 0; index < nodes.Length; index++)
                        {
                            copy[index] = nodes[index];
                        }

                        result.m_Surfaces.Add(new SurfaceSnapshot
                        {
                            Prefab = prefab,
                            Nodes = copy,
                            Flags = manager.GetComponentData<Area>(entity).m_Flags
                        });
                    }
                }
            }

            if (manager.HasBuffer<SubObject>(building))
            {
                foreach (SubObject child in manager.GetBuffer<SubObject>(building, true))
                {
                    Entity entity = child.m_SubObject;
                    // Preserve vegetation and visible static lot decorations, including decals.
                    if (!TryGetPrefab(manager, entity, out Entity prefab) ||
                        (!manager.HasComponent<Game.Objects.Static>(entity) && !manager.HasComponent<Plant>(entity)) ||
                        manager.HasComponent<Game.Buildings.Building>(entity) ||
                        manager.HasComponent<Game.Objects.SpawnLocation>(entity) ||
                        !manager.HasComponent<Transform>(entity) ||
                        !manager.HasComponent<ObjectData>(prefab) ||
                        !manager.HasComponent<ObjectGeometryData>(prefab))
                    {
                        continue;
                    }

                    Attachment? attachment = Read<Attachment>(manager, entity);
                    if (attachment is Attachment attached && attached.m_Parent != building)
                    {
                        continue;
                    }

                    result.m_Decoration.Add(new DecorationSnapshot
                    {
                        Prefab = prefab,
                        Transform = manager.GetComponentData<Transform>(entity),
                        Relative = Read<Relative>(manager, entity),
                        Elevation = Read<Elevation>(manager, entity),
                        Seed = Read<PseudoRandomSeed>(manager, entity),
                        Attachment = attachment,
                        Entry = child,
                        Plant = Read<Plant>(manager, entity),
                        Tree = Read<Tree>(manager, entity)
                    });
                }
            }

            if (manager.HasBuffer<SubNet>(building))
            {
                foreach (SubNet child in manager.GetBuffer<SubNet>(building, true))
                {
                    Entity entity = child.m_SubNet;
                    if (!TryGetPrefab(manager, entity, out Entity prefab) ||
                        !manager.HasComponent<Owner>(entity) ||
                        manager.GetComponentData<Owner>(entity).m_Owner != building)
                    {
                        continue;
                    }

                    NetNode? node = Read<NetNode>(manager, entity);
                    NetCurve? curve = Read<NetCurve>(manager, entity);
                    if (node.HasValue || curve.HasValue)
                    {
                        result.m_Network.Add(new NetworkHeight
                        {
                            Prefab = prefab,
                            Node = node,
                            Curve = curve,
                            Elevation = Read<NetElevation>(manager, entity)
                        });
                    }
                }
            }

            return result;
        }

        // The construction and ground-height passes can change the vertical alignment.
        // Leave the asset's new site alone if it was moved before these updates ran.
        internal bool RestoreFoundation(EntityManager manager, Entity building)
        {
            if (!IsAtOriginalSite(manager, building))
            {
                return false;
            }

            manager.SetComponentData(building, m_Transform);
            if (m_Elevation is Elevation elevation)
            {
                Set(manager, building, elevation);
            }

            if (m_Lot is BuildingLot lot)
            {
                Set(manager, building, lot);
            }

            return true;
        }

        private bool IsAtOriginalSite(EntityManager manager, Entity building)
        {
            if (!manager.HasComponent<Transform>(building))
            {
                return false;
            }

            Transform current = manager.GetComponentData<Transform>(building);
            float3 forward = new float3(0f, 0f, 1f);
            float2 currentDirection = math.normalizesafe(math.rotate(current.m_Rotation, forward).xz);
            float2 savedDirection = math.normalizesafe(math.rotate(m_Transform.m_Rotation, forward).xz);
            return math.distancesq(current.m_Position.xz, m_Transform.m_Position.xz) <= 0.01f &&
                   math.dot(currentDirection, savedDirection) > 0.9999f;
        }

        internal void RestoreLotElements(EntityManager manager, Entity building)
        {
            if (!IsAtOriginalSite(manager, building))
            {
                return;
            }

            foreach (SurfaceSnapshot snapshot in m_Surfaces)
            {
                if (!manager.Exists(snapshot.Prefab) || !manager.HasComponent<AreaData>(snapshot.Prefab))
                {
                    continue;
                }

                Entity surface = FindMatchingSurface(manager, building, snapshot);
                if (surface == Entity.Null)
                {
                    RestoreSurface(manager, building, snapshot);
                }
                else
                {
                    ApplySavedSurface(manager, surface, snapshot);
                }
            }

            foreach (DecorationSnapshot snapshot in m_Decoration)
            {
                if (!manager.Exists(snapshot.Prefab) || !manager.HasComponent<ObjectData>(snapshot.Prefab))
                {
                    continue;
                }

                Entity decoration = FindMatchingDecoration(manager, building, snapshot);
                if (decoration == Entity.Null)
                {
                    RestoreDecoration(manager, building, snapshot);
                }
                else
                {
                    ApplySavedDecoration(manager, decoration, building, snapshot);
                }
            }
        }

        internal void RestoreOwnedNetworkHeight(EntityManager manager, Entity building)
        {
            if (m_Network.Count == 0 || !IsAtOriginalSite(manager, building) || !manager.HasBuffer<SubNet>(building))
            {
                return;
            }

            // Snapshot the buffer before any structural write. The game has already rebuilt
            // these networks, so keep their new endpoints and lane references.
            DynamicBuffer<SubNet> buffer = manager.GetBuffer<SubNet>(building, true);
            var children = new Entity[buffer.Length];
            for (int index = 0; index < children.Length; index++)
            {
                children[index] = buffer[index].m_SubNet;
            }

            foreach (Entity entity in children)
            {
                if (!TryGetPrefab(manager, entity, out Entity prefab) ||
                    !manager.HasComponent<Owner>(entity) ||
                    manager.GetComponentData<Owner>(entity).m_Owner != building)
                {
                    continue;
                }

                NetNode? currentNode = Read<NetNode>(manager, entity);
                NetCurve? currentCurve = Read<NetCurve>(manager, entity);
                foreach (NetworkHeight saved in m_Network)
                {
                    if (saved.Prefab != prefab)
                    {
                        continue;
                    }

                    if (currentNode is NetNode node && saved.Node is NetNode savedNode &&
                        SamePlanPosition(node.m_Position, savedNode.m_Position))
                    {
                        node.m_Position.y = savedNode.m_Position.y;
                        manager.SetComponentData(entity, node);
                    }
                    else if (currentCurve is NetCurve curve && saved.Curve is NetCurve savedCurve &&
                             SamePlanPosition(curve.m_Bezier.a, savedCurve.m_Bezier.a) &&
                             SamePlanPosition(curve.m_Bezier.b, savedCurve.m_Bezier.b) &&
                             SamePlanPosition(curve.m_Bezier.c, savedCurve.m_Bezier.c) &&
                             SamePlanPosition(curve.m_Bezier.d, savedCurve.m_Bezier.d) &&
                             CanRestoreCurveHeight(manager, entity, building, savedCurve))
                    {
                        curve.m_Bezier.a.y = savedCurve.m_Bezier.a.y;
                        curve.m_Bezier.b.y = savedCurve.m_Bezier.b.y;
                        curve.m_Bezier.c.y = savedCurve.m_Bezier.c.y;
                        curve.m_Bezier.d.y = savedCurve.m_Bezier.d.y;
                        curve.m_Length = MathUtils.Length(curve.m_Bezier);
                        manager.SetComponentData(entity, curve);
                    }
                    else
                    {
                        continue;
                    }

                    if (saved.Elevation is NetElevation elevation)
                    {
                        Set(manager, entity, elevation);
                    }

                    manager.AddComponent<Updated>(entity);
                    break;
                }
            }
        }

        private static bool CanRestoreCurveHeight(EntityManager manager, Entity entity, Entity building, NetCurve saved)
        {
            if (!manager.HasComponent<NetEdge>(entity))
            {
                return true;
            }

            NetEdge edge = manager.GetComponentData<NetEdge>(entity);
            return CanRestoreEndpointHeight(manager, edge.m_Start, building, saved.m_Bezier.a.y) &&
                   CanRestoreEndpointHeight(manager, edge.m_End, building, saved.m_Bezier.d.y);
        }

        private static bool CanRestoreEndpointHeight(EntityManager manager, Entity entity, Entity building, float height)
        {
            if (!manager.Exists(entity) || !manager.HasComponent<NetNode>(entity))
            {
                return false;
            }

            return (manager.HasComponent<Owner>(entity) && manager.GetComponentData<Owner>(entity).m_Owner == building) ||
                   math.abs(manager.GetComponentData<NetNode>(entity).m_Position.y - height) <= 0.15f;
        }

        private static bool SamePlanPosition(float3 left, float3 right) =>
            math.distancesq(left.xz, right.xz) <= 0.15f * 0.15f;

        private static bool TryGetPrefab(EntityManager manager, Entity entity, out Entity prefab)
        {
            prefab = Entity.Null;
            if (entity == Entity.Null || !manager.Exists(entity) ||
                manager.HasComponent<Deleted>(entity) || manager.HasComponent<Temp>(entity) ||
                !manager.HasComponent<PrefabRef>(entity))
            {
                return false;
            }

            prefab = manager.GetComponentData<PrefabRef>(entity).m_Prefab;
            return prefab != Entity.Null && manager.Exists(prefab) && !manager.HasComponent<Deleted>(prefab);
        }

        private static Entity FindMatchingSurface(EntityManager manager, Entity building, SurfaceSnapshot snapshot)
        {
            if (!manager.HasBuffer<SubArea>(building))
            {
                return Entity.Null;
            }

            foreach (SubArea child in manager.GetBuffer<SubArea>(building, true))
            {
                Entity entity = child.m_Area;
                if (!TryGetPrefab(manager, entity, out Entity prefab) || prefab != snapshot.Prefab ||
                    !manager.HasBuffer<AreaNode>(entity) || !manager.HasComponent<Area>(entity))
                {
                    continue;
                }

                DynamicBuffer<AreaNode> nodes = manager.GetBuffer<AreaNode>(entity, true);
                if (nodes.Length == snapshot.Nodes.Length && SameOutline(nodes, snapshot.Nodes))
                {
                    return entity;
                }
            }

            return Entity.Null;
        }

        private static bool SameOutline(DynamicBuffer<AreaNode> existing, AreaNode[] saved)
        {
            const float toleranceSquared = 0.15f * 0.15f;
            int count = saved.Length;
            for (int start = 0; start < count; start++)
            {
                if (math.distancesq(existing[start].m_Position.xz, saved[0].m_Position.xz) > toleranceSquared)
                {
                    continue;
                }

                bool forward = true;
                bool backward = true;
                for (int index = 1; index < count && (forward || backward); index++)
                {
                    float2 position = saved[index].m_Position.xz;
                    forward &= math.distancesq(existing[(start + index) % count].m_Position.xz, position) <= toleranceSquared;
                    backward &= math.distancesq(existing[(start - index + count) % count].m_Position.xz, position) <= toleranceSquared;
                }

                if (forward || backward)
                {
                    return true;
                }
            }

            return false;
        }

        private static Entity FindMatchingDecoration(EntityManager manager, Entity building, DecorationSnapshot snapshot)
        {
            if (!manager.HasBuffer<SubObject>(building))
            {
                return Entity.Null;
            }

            foreach (SubObject child in manager.GetBuffer<SubObject>(building, true))
            {
                Entity entity = child.m_SubObject;
                if (TryGetPrefab(manager, entity, out Entity prefab) && prefab == snapshot.Prefab &&
                    manager.HasComponent<Transform>(entity))
                {
                    Transform current = manager.GetComponentData<Transform>(entity);
                    if (math.distancesq(current.m_Position.xz, snapshot.Transform.m_Position.xz) <= 0.01f &&
                        math.abs(math.dot(current.m_Rotation.value, snapshot.Transform.m_Rotation.value)) > 0.9999f)
                    {
                        return entity;
                    }
                }
            }

            return Entity.Null;
        }

        private static void RestoreSurface(EntityManager manager, Entity building, SurfaceSnapshot snapshot)
        {
            EntityArchetype archetype = manager.GetComponentData<AreaData>(snapshot.Prefab).m_Archetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return;
            }

            using NativeArray<Entity> created = manager.CreateEntity(archetype, 1, Allocator.Temp);
            Entity surface = created[0];
            if (!manager.HasComponent<PrefabRef>(surface) || !manager.HasBuffer<AreaNode>(surface) ||
                !manager.HasComponent<Area>(surface) ||
                (!manager.HasComponent<AreaSurface>(surface) && !manager.HasComponent<AreaLot>(surface)))
            {
                manager.DestroyEntity(surface);
                return;
            }

            manager.SetComponentData(surface, new PrefabRef(snapshot.Prefab));
            Set(manager, surface, new Owner { m_Owner = building });
            ApplySavedSurface(manager, surface, snapshot);
            DynamicBuffer<SubArea> surfaces = manager.HasBuffer<SubArea>(building)
                ? manager.GetBuffer<SubArea>(building)
                : manager.AddBuffer<SubArea>(building);
            surfaces.Add(new SubArea { m_Area = surface });
        }

        private static void ApplySavedSurface(EntityManager manager, Entity surface, SurfaceSnapshot snapshot)
        {
            manager.SetComponentData(surface, new Area { m_Flags = snapshot.Flags });
            DynamicBuffer<AreaNode> nodes = manager.GetBuffer<AreaNode>(surface);
            nodes.Clear();
            foreach (AreaNode node in snapshot.Nodes)
            {
                nodes.Add(node);
            }

            manager.AddComponent<Updated>(surface);
        }

        private static void RestoreDecoration(EntityManager manager, Entity building, DecorationSnapshot snapshot)
        {
            EntityArchetype archetype = manager.GetComponentData<ObjectData>(snapshot.Prefab).m_Archetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return;
            }

            using NativeArray<Entity> created = manager.CreateEntity(archetype, 1, Allocator.Temp);
            Entity decal = created[0];
            if (!manager.HasComponent<PrefabRef>(decal) || !manager.HasComponent<Transform>(decal))
            {
                manager.DestroyEntity(decal);
                return;
            }

            manager.SetComponentData(decal, new PrefabRef(snapshot.Prefab));
            Set(manager, decal, new Owner { m_Owner = building });
            ApplySavedDecoration(manager, decal, building, snapshot);
            SubObject entry = snapshot.Entry;
            entry.m_SubObject = decal;
            DynamicBuffer<SubObject> children = manager.HasBuffer<SubObject>(building)
                ? manager.GetBuffer<SubObject>(building)
                : manager.AddBuffer<SubObject>(building);
            children.Add(entry);
        }

        private static void ApplySavedDecoration(EntityManager manager, Entity decal, Entity building, DecorationSnapshot snapshot)
        {
            manager.SetComponentData(decal, snapshot.Transform);
            if (snapshot.Plant is Plant plant && manager.HasComponent<Plant>(decal))
            {
                manager.SetComponentData(decal, plant);
            }

            if (snapshot.Tree is Tree tree && manager.HasComponent<Tree>(decal))
            {
                manager.SetComponentData(decal, tree);
            }

            if (snapshot.Relative is Relative relative)
            {
                Set(manager, decal, relative);
            }

            if (snapshot.Elevation is Elevation elevation)
            {
                Set(manager, decal, elevation);
            }

            if (snapshot.Seed is PseudoRandomSeed seed && manager.HasComponent<PseudoRandomSeed>(decal))
            {
                manager.SetComponentData(decal, seed);
            }

            if (snapshot.Attachment is Attachment attachment)
            {
                attachment.m_Parent = building;
                Set(manager, decal, attachment);
            }

            manager.AddComponent<Updated>(decal);
            manager.AddComponent<BatchesUpdated>(decal);
        }

        private static T? Read<T>(EntityManager manager, Entity entity) where T : unmanaged, IComponentData =>
            manager.HasComponent<T>(entity) ? manager.GetComponentData<T>(entity) : (T?)null;

        private static void Set<T>(EntityManager manager, Entity entity, T data) where T : unmanaged, IComponentData
        {
            if (manager.HasComponent<T>(entity))
            {
                manager.SetComponentData(entity, data);
            }
            else
            {
                manager.AddComponentData(entity, data);
            }
        }
    }
}
