using Colossal.Collections;
using Colossal.Mathematics;
using Game;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Building = Game.Buildings.Building;
using Destroyed = Game.Common.Destroyed;
using Edge = Game.Net.Edge;
using NetSearchSystem = Game.Net.SearchSystem;
using NetNode = Game.Net.Node;
using SpawnLocation = Game.Objects.SpawnLocation;
using SubObject = Game.Objects.SubObject;
using Transform = Game.Objects.Transform;
using UnderConstruction = Game.Objects.UnderConstruction;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerBuildingConnectionSystem : GameSystemBase
    {
        private const int ConnectionRefreshDelay = 4;

        private struct PendingBuilding
        {
            public Entity Entity;
            public Entity OriginalRoad;
            public int RefreshNetworkAt;
            public bool WaitReported;
            public bool PausedCompletionRequested;
            public BuildingLotAppearance LotAppearance;
        }

        private readonly List<PendingBuilding> m_Pending = new();
        private readonly List<Entity> m_NetworkRefresh = new();
        private readonly HashSet<Entity> m_Updates = new();
        private readonly HashSet<Entity> m_ScannedSubObjects = new();
        private NetSearchSystem m_NetSearch;
        private SimulationSystem m_Simulation;
        private BuildingConstructionSystem m_Construction;
        private EntityQuery m_ConstructionQuery;
        private readonly HashSet<Entity> m_PausedCandidates = new();
        private int m_Tick;
        private bool m_PausedWaitReported;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_NetSearch = World.GetOrCreateSystemManaged<NetSearchSystem>();
            m_Simulation = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_Construction = World.GetOrCreateSystemManaged<BuildingConstructionSystem>();
            // Match the vanilla construction query so a paused completion pass
            // cannot advance an unrelated construction site.
            m_ConstructionQuery = GetEntityQuery(
                ComponentType.ReadOnly<Building>(),
                ComponentType.ReadOnly<UnderConstruction>(),
                ComponentType.Exclude<Destroyed>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());
        }

        protected override void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            m_Pending.Clear();
            m_NetworkRefresh.Clear();
            m_Updates.Clear();
            m_ScannedSubObjects.Clear();
            m_PausedCandidates.Clear();
            m_Tick = 0;
            m_PausedWaitReported = false;
        }

        internal void Track(Entity building, Entity originalRoad, BuildingLotAppearance lotAppearance)
        {
            m_Pending.Add(new PendingBuilding
            {
                Entity = building,
                OriginalRoad = originalRoad,
                LotAppearance = lotAppearance
            });

            Mod.LogDiagnosticInfo($"Building undo queued for {building.Index}. {lotAppearance.DescribeSnapshot()}.");
        }

        protected override void OnUpdate()
        {
            if (m_Pending.Count == 0)
            {
                return;
            }

            m_Tick++;
            m_Updates.Clear();
            m_NetworkRefresh.Clear();
            m_ScannedSubObjects.Clear();
            m_PausedCandidates.Clear();

            for (int index = m_Pending.Count - 1; index >= 0; index--)
            {
                PendingBuilding pending = m_Pending[index];
                Entity building = pending.Entity;
                if (!IsLive(building) || !EntityManager.HasComponent<Building>(building))
                {
                    RemovePending(index);
                    continue;
                }

                // Construction creates the lot and its access points. Until it completes, the
                // street/footpath systems would rebuild against an incomplete building.
                if (EntityManager.HasComponent<UnderConstruction>(building))
                {
                    UnderConstruction construction = EntityManager.GetComponentData<UnderConstruction>(building);
                    if (!pending.WaitReported)
                    {
                        pending.WaitReported = true;
                        m_Pending[index] = pending;
                        Mod.LogDiagnosticInfo(
                            $"Building undo waiting for construction of {building.Index}. Progress {construction.m_Progress}, simulation speed {m_Simulation.selectedSpeed}.");
                    }

                    if (m_Simulation.selectedSpeed <= 0f && !pending.PausedCompletionRequested &&
                        construction.m_Progress >= 100 && !EntityManager.HasComponent<Destroyed>(building))
                    {
                        m_PausedCandidates.Add(building);
                    }

                    continue;
                }

                if (pending.RefreshNetworkAt == 0)
                {
                    Mod.LogDiagnosticInfo($"Building undo construction completed for {building.Index}.");
                    bool originalSite = pending.LotAppearance.RestoreFoundation(EntityManager, building);
                    if (originalSite)
                    {
                        EntityManager.AddComponent<BatchesUpdated>(building);
                        QueueEdgeAndNodes(pending.OriginalRoad);
                    }

                    m_Updates.Add(building);
                    CollectAccessObjects(building, depth: 0);
                    pending.RefreshNetworkAt = m_Tick + ConnectionRefreshDelay;
                    m_Pending[index] = pending;
                }
                else if (m_Tick >= pending.RefreshNetworkAt)
                {
                    pending.LotAppearance.RestoreOwnedNetworkHeight(EntityManager, building);
                    pending.LotAppearance.RestoreLotElements(EntityManager, building);
                    CollectAccessObjects(building, depth: 0);
                    m_NetworkRefresh.Add(building);
                    Building buildingData = EntityManager.GetComponentData<Building>(building);
                    int areaCount = EntityManager.HasBuffer<Game.Areas.SubArea>(building)
                        ? EntityManager.GetBuffer<Game.Areas.SubArea>(building, true).Length : 0;
                    int netCount = EntityManager.HasBuffer<Game.Net.SubNet>(building)
                        ? EntityManager.GetBuffer<Game.Net.SubNet>(building, true).Length : 0;
                    Mod.LogDiagnosticInfo(
                        $"Building undo lot restored for {building.Index}. Area entries {areaCount}, network entries {netCount}, road {(buildingData.m_RoadEdge == Entity.Null ? "none" : buildingData.m_RoadEdge.Index.ToString())}.");
                    RemovePending(index);
                }
            }

            if (m_NetworkRefresh.Count > 0)
            {
                CollectNearbyNetworkEdges();
            }

            // Structural writes come after reading the game buffers and search tree.
            foreach (Entity entity in m_Updates)
            {
                EntityManager.AddComponent<Updated>(entity);
            }

            CompletePausedUndoConstruction();
        }

        private void CompletePausedUndoConstruction()
        {
            if (m_PausedCandidates.Count == 0)
            {
                m_PausedWaitReported = false;
                return;
            }

            // PostSimulation also runs at speed zero, with the end-frame command
            // barrier open. Never run a second pass while normal simulation is active.
            if (m_ConstructionQuery.CalculateEntityCount() != m_PausedCandidates.Count)
            {
                LogPausedWaitOnce();
                return;
            }

            using NativeArray<Entity> construction = m_ConstructionQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in construction)
            {
                if (!m_PausedCandidates.Contains(entity))
                {
                    LogPausedWaitOnce();
                    return;
                }
            }

            for (int index = 0; index < m_Pending.Count; index++)
            {
                PendingBuilding pending = m_Pending[index];
                if (m_PausedCandidates.Contains(pending.Entity))
                {
                    pending.PausedCompletionRequested = true;
                    m_Pending[index] = pending;
                }
            }

            m_PausedWaitReported = false;
            try
            {
                m_Construction.Update();
                Mod.LogDiagnosticInfo($"Building undo requested a paused construction pass for {m_PausedCandidates.Count} building(s).");
            }
            catch (System.InvalidOperationException exception)
            {
                // Keep the records queued. Normal simulation can finish the buildings.
                Mod.Log.Warn($"Building undo could not run the paused construction pass: {exception.Message}");
            }
        }

        private void LogPausedWaitOnce()
        {
            if (m_PausedWaitReported)
            {
                return;
            }

            m_PausedWaitReported = true;
            Mod.LogDiagnosticInfo("Building undo is waiting for unpause because the game has other construction sites.");
        }

        private void RemovePending(int index)
        {
            int last = m_Pending.Count - 1;
            m_Pending[index] = m_Pending[last];
            m_Pending.RemoveAt(last);
        }

        private bool IsLive(Entity entity) =>
            EntityManager.Exists(entity) &&
            !EntityManager.HasComponent<Deleted>(entity) &&
            !EntityManager.HasComponent<Temp>(entity);

        private void CollectAccessObjects(Entity entity, int depth)
        {
            if (depth >= 8 || !m_ScannedSubObjects.Add(entity))
            {
                return;
            }

            if (EntityManager.HasBuffer<SubObject>(entity))
            {
                DynamicBuffer<SubObject> children = EntityManager.GetBuffer<SubObject>(entity, true);
                foreach (SubObject child in children)
                {
                    if (!IsLive(child.m_SubObject))
                    {
                        continue;
                    }

                    if (EntityManager.HasComponent<SpawnLocation>(child.m_SubObject) ||
                        EntityManager.HasBuffer<Game.Net.SubLane>(child.m_SubObject))
                    {
                        m_Updates.Add(child.m_SubObject);
                    }

                    CollectAccessObjects(child.m_SubObject, depth + 1);
                }
            }

            if (EntityManager.HasBuffer<Game.Net.SubNet>(entity))
            {
                DynamicBuffer<Game.Net.SubNet> nets = EntityManager.GetBuffer<Game.Net.SubNet>(entity, true);
                foreach (Game.Net.SubNet net in nets)
                {
                    if (IsLive(net.m_SubNet))
                    {
                        m_Updates.Add(net.m_SubNet);
                        QueueEdgeAndNodes(net.m_SubNet);
                        CollectAccessObjects(net.m_SubNet, depth + 1);
                    }
                }
            }
        }

        private void QueueEdgeAndNodes(Entity entity)
        {
            if (!IsLive(entity) || !EntityManager.HasComponent<Edge>(entity))
            {
                return;
            }

            m_Updates.Add(entity);
            Edge edge = EntityManager.GetComponentData<Edge>(entity);
            if (IsLive(edge.m_Start) && EntityManager.HasComponent<NetNode>(edge.m_Start))
            {
                m_Updates.Add(edge.m_Start);
            }

            if (IsLive(edge.m_End) && EntityManager.HasComponent<NetNode>(edge.m_End))
            {
                m_Updates.Add(edge.m_End);
            }
        }

        private void CollectNearbyNetworkEdges()
        {
            NativeQuadTree<Entity, QuadTreeBoundsXZ> tree = m_NetSearch.GetNetSearchTree(true, out JobHandle ready);
            ready.Complete();

            using NativeList<Entity> matches = new NativeList<Entity>(32, Allocator.Temp);
            foreach (Entity building in m_NetworkRefresh)
            {
                Building data = EntityManager.GetComponentData<Building>(building);
                QueueEdgeAndNodes(data.m_RoadEdge);

                if (!EntityManager.HasComponent<Transform>(building))
                {
                    continue;
                }

                float radius = 32f;
                if (EntityManager.HasComponent<PrefabRef>(building))
                {
                    Entity prefab = EntityManager.GetComponentData<PrefabRef>(building).m_Prefab;
                    if (EntityManager.Exists(prefab) && EntityManager.HasComponent<BuildingData>(prefab))
                    {
                        BuildingData prefabData = EntityManager.GetComponentData<BuildingData>(prefab);
                        radius += 4f * math.length((float2)prefabData.m_LotSize);
                    }
                }

                float3 center = EntityManager.GetComponentData<Transform>(building).m_Position;
                float3 extents = new float3(radius, 24f, radius);
                matches.Clear();
                var visitor = new NetworkVisitor(new Bounds3(center - extents, center + extents), matches);
                tree.Iterate(ref visitor, 0);

                foreach (Entity edge in matches)
                {
                    if (IsLive(edge) && EntityManager.HasComponent<Edge>(edge) &&
                        !BelongsToBuilding(edge, building))
                    {
                        QueueEdgeAndNodes(edge);
                    }
                }
            }
        }

        private bool BelongsToBuilding(Entity entity, Entity building)
        {
            for (int depth = 0; depth < 8 && IsLive(entity) && EntityManager.HasComponent<Owner>(entity); depth++)
            {
                entity = EntityManager.GetComponentData<Owner>(entity).m_Owner;
                if (entity == building)
                {
                    return true;
                }
            }

            return false;
        }

        private struct NetworkVisitor : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
        {
            private Bounds3 m_Bounds;
            private NativeList<Entity> m_Matches;

            internal NetworkVisitor(Bounds3 bounds, NativeList<Entity> matches)
            {
                m_Bounds = bounds;
                m_Matches = matches;
            }

            public bool Intersect(QuadTreeBoundsXZ bounds) => MathUtils.Intersect(bounds.m_Bounds, m_Bounds);

            public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
            {
                if (Intersect(bounds))
                {
                    m_Matches.Add(entity);
                }
            }
        }
    }
}
