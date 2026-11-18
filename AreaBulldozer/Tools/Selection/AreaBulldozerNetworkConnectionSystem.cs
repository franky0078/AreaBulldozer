using Game;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Routes;
using System.Collections.Generic;
using Unity.Entities;
using Edge = Game.Net.Edge;

namespace AreaBulldozer.Tools
{
    // A road's lanes and its stop connections are generated in separate game
    // modification passes. Retry routes only after those passes have finished.
    public partial class AreaBulldozerNetworkConnectionSystem : GameSystemBase
    {
        private sealed class PendingNetwork
        {
            public HashSet<Entity> Networks;
            public HashSet<Entity> Routes;
            public int Age;
            public int StableUpdates;
            public int RouteRefreshDelay = -1;
        }

        private readonly List<PendingNetwork> m_Pending = new();
        private readonly HashSet<Entity> m_Lanes = new();
        private readonly HashSet<Entity> m_Updates = new();
        private readonly HashSet<Entity> m_CompletedRoutes = new();

        internal void Track(HashSet<Entity> networks, HashSet<Entity> routes)
        {
            if (networks.Count > 0)
            {
                m_Pending.Add(new PendingNetwork
                {
                    Networks = new HashSet<Entity>(networks),
                    Routes = new HashSet<Entity>(routes)
                });
            }
        }

        protected override void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            m_Pending.Clear();
            m_Lanes.Clear();
            m_Updates.Clear();
            m_CompletedRoutes.Clear();
        }

        protected override void OnUpdate()
        {
            for (int index = m_Pending.Count - 1; index >= 0; index--)
            {
                PendingNetwork pending = m_Pending[index];
                if (++pending.Age > 1024)
                {
                    Mod.Log.Warn("Network undo: timed out waiting for regenerated lanes or transit paths.");
                    m_Pending.RemoveAt(index);
                    continue;
                }

                if (pending.RouteRefreshDelay < 0)
                {
                    if (!AreNetworkLanesReady(pending.Networks))
                    {
                        pending.StableUpdates = 0;
                        continue;
                    }
                    if (++pending.StableUpdates < 2)
                    {
                        continue;
                    }

                    m_Lanes.Clear();
                    m_Updates.Clear();
                    foreach (Entity network in pending.Networks)
                    {
                        CollectNetworkLanes(network);
                    }
                    foreach (Entity route in pending.Routes)
                    {
                        CollectRouteWaypoints(route);
                    }

                    // Applied is the native signal for reconsidering transit
                    // paths when a new travel lane becomes available.
                    foreach (Entity lane in m_Lanes)
                    {
                        EntityManager.AddComponent<Applied>(lane);
                    }
                    foreach (Entity entity in m_Updates)
                    {
                        EntityManager.AddComponent<Updated>(entity);
                    }
                    pending.RouteRefreshDelay = 4;
                    continue;
                }

                if (pending.RouteRefreshDelay > 0)
                {
                    pending.RouteRefreshDelay--;
                    continue;
                }
                m_Updates.Clear();
                m_CompletedRoutes.Clear();
                foreach (Entity route in pending.Routes)
                {
                    if (!IsLive(route) || !EntityManager.HasBuffer<RouteSegment>(route))
                    {
                        m_CompletedRoutes.Add(route);
                        continue;
                    }
                    if (!AreRouteConnectionsReady(route))
                    {
                        continue;
                    }
                    DynamicBuffer<RouteSegment> segments = EntityManager.GetBuffer<RouteSegment>(route, true);
                    foreach (RouteSegment entry in segments)
                    {
                        if (IsLive(entry.m_Segment) && EntityManager.HasComponent<PathTargets>(entry.m_Segment))
                        {
                            m_Updates.Add(entry.m_Segment);
                        }
                    }
                    m_CompletedRoutes.Add(route);
                }

                foreach (Entity segment in m_Updates)
                {
                    PathTargets targets = EntityManager.GetComponentData<PathTargets>(segment);
                    // Match RoutePathSystem's forced retry sentinel. Keeping an
                    // old endpoint pair can otherwise leave a failed path cached.
                    targets.m_StartLane = new Entity { Index = -1 };
                    EntityManager.SetComponentData(segment, targets);
                    EntityManager.AddComponent<Updated>(segment);
                }
                if (m_Updates.Count > 0)
                {
                    Mod.LogDiagnosticInfo($"Network undo refreshed {m_Updates.Count} transit segment(s) on {m_CompletedRoutes.Count} affected line(s).");
                }
                pending.Routes.ExceptWith(m_CompletedRoutes);
                if (pending.Routes.Count == 0) m_Pending.RemoveAt(index);
            }
        }

        private bool IsLive(Entity entity) => entity != Entity.Null && EntityManager.Exists(entity) &&
            !EntityManager.HasComponent<Deleted>(entity) && !EntityManager.HasComponent<Game.Tools.Temp>(entity);

        private bool AreNetworkLanesReady(HashSet<Entity> networks)
        {
            foreach (Entity network in networks)
            {
                if (!IsLive(network))
                {
                    continue;
                }
                if (EntityManager.HasComponent<Created>(network) || EntityManager.HasComponent<Updated>(network))
                {
                    return false;
                }
                if (EntityManager.HasComponent<Edge>(network) &&
                    (!EntityManager.HasBuffer<SubLane>(network) ||
                     EntityManager.GetBuffer<SubLane>(network, true).Length == 0))
                {
                    return false;
                }
            }
            return true;
        }

        private void CollectNetworkLanes(Entity network)
        {
            if (!IsLive(network) || !EntityManager.HasBuffer<SubLane>(network))
            {
                return;
            }
            DynamicBuffer<SubLane> lanes = EntityManager.GetBuffer<SubLane>(network, true);
            foreach (SubLane entry in lanes)
            {
                if (IsLive(entry.m_SubLane)) m_Lanes.Add(entry.m_SubLane);
            }
        }

        private void CollectRouteWaypoints(Entity route)
        {
            if (!IsLive(route) || !EntityManager.HasBuffer<RouteWaypoint>(route))
            {
                return;
            }
            DynamicBuffer<RouteWaypoint> waypoints = EntityManager.GetBuffer<RouteWaypoint>(route, true);
            foreach (RouteWaypoint entry in waypoints)
            {
                Entity waypoint = entry.m_Waypoint;
                if (!IsLive(waypoint)) continue;
                m_Updates.Add(waypoint);
                if (EntityManager.HasComponent<Connected>(waypoint))
                {
                    Entity stop = EntityManager.GetComponentData<Connected>(waypoint).m_Connected;
                    if (IsLive(stop)) m_Updates.Add(stop);
                }
            }
        }

        private bool AreRouteConnectionsReady(Entity route)
        {
            if (EntityManager.HasBuffer<RouteWaypoint>(route))
            {
                DynamicBuffer<RouteWaypoint> waypoints = EntityManager.GetBuffer<RouteWaypoint>(route, true);
                foreach (RouteWaypoint entry in waypoints)
                {
                    Entity waypoint = entry.m_Waypoint;
                    if (!IsLive(waypoint) || !EntityManager.HasComponent<RouteLane>(waypoint))
                    {
                        return false;
                    }
                    RouteLane lane = EntityManager.GetComponentData<RouteLane>(waypoint);
                    if (!IsLive(lane.m_StartLane) || !IsLive(lane.m_EndLane)) return false;
                }
            }
            DynamicBuffer<RouteSegment> segments = EntityManager.GetBuffer<RouteSegment>(route, true);
            foreach (RouteSegment entry in segments)
            {
                Entity segment = entry.m_Segment;
                if (IsLive(segment) && EntityManager.HasComponent<PathOwner>(segment) &&
                    (EntityManager.GetComponentData<PathOwner>(segment).m_State & PathFlags.Pending) != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
