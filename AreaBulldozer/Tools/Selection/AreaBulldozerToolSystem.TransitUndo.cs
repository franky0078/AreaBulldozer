using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Routes;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using ObjectTransform = Game.Objects.Transform;
using ObjectElevation = Game.Objects.Elevation;
using Attached = Game.Objects.Attached;
using TransportStop = Game.Routes.TransportStop;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private struct NetworkStopSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public ObjectTransform Transform;
            public Attached? Attachment;
            public Owner? Owner;
            public ObjectElevation? Elevation;
            public PseudoRandomSeed? Seed;
            public bool Native;
        }

        private readonly Dictionary<Entity, NetworkStopSnapshot> m_NetworkUndoStops = new();
        private readonly HashSet<Entity> m_NetworkUndoRoutes = new();
        private readonly Dictionary<Entity, Entity> m_NetworkUndoWaypointStops = new();

        private void ClearNetworkTransitUndo()
        {
            m_NetworkUndoStops.Clear();
            m_NetworkUndoRoutes.Clear();
            m_NetworkUndoWaypointStops.Clear();
        }

        private void RecordNetworkTransitForUndo(HashSet<Entity> selectedEdges)
        {
            if (selectedEdges.Count == 0)
            {
                return;
            }

            var affected = new HashSet<Entity>(selectedEdges);
            foreach (Entity edge in selectedEdges)
            {
                if (EntityManager.HasComponent<Edge>(edge))
                {
                    Edge data = EntityManager.GetComponentData<Edge>(edge);
                    affected.Add(data.m_Start);
                    affected.Add(data.m_End);
                }
            }

            // Attached roadside stops are removed together with their road, even
            // when the object filters are off. Preserve those independent stops.
            using EntityQuery stopQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TransportStop>(), ComponentType.ReadOnly<ObjectTransform>(),
                ComponentType.ReadOnly<PrefabRef>(), ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Game.Tools.Temp>());
            using NativeArray<Entity> stops = stopQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity stop in stops)
            {
                if (!BelongsToUndoNetwork(stop, affected) || m_NetworkUndoStops.ContainsKey(stop) ||
                    !TryGetUndoPrefab<ObjectData>(stop, out Entity prefab))
                {
                    continue;
                }

                m_NetworkUndoStops.Add(stop, new NetworkStopSnapshot
                {
                    Original = stop,
                    Prefab = prefab,
                    Transform = EntityManager.GetComponentData<ObjectTransform>(stop),
                    Attachment = ReadUndoComponent<Attached>(stop),
                    Owner = ReadUndoComponent<Owner>(stop),
                    Elevation = ReadUndoComponent<ObjectElevation>(stop),
                    Seed = ReadUndoComponent<PseudoRandomSeed>(stop),
                    Native = EntityManager.HasComponent<Native>(stop)
                });
            }

            using EntityQuery routeQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Route>(), ComponentType.ReadOnly<TransportLine>(),
                ComponentType.ReadOnly<RouteSegment>(), ComponentType.ReadOnly<RouteWaypoint>(),
                ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Game.Tools.Temp>());
            using NativeArray<Entity> routes = routeQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity route in routes)
            {
                bool usesNetwork = false;
                DynamicBuffer<RouteWaypoint> waypoints = EntityManager.GetBuffer<RouteWaypoint>(route, true);
                foreach (RouteWaypoint item in waypoints)
                {
                    Entity waypoint = item.m_Waypoint;
                    if (ReadUndoComponent<Connected>(waypoint) is Connected connected)
                    {
                        usesNetwork |= BelongsToUndoNetwork(connected.m_Connected, affected);
                        if (m_NetworkUndoStops.ContainsKey(connected.m_Connected))
                        {
                            m_NetworkUndoWaypointStops[waypoint] = connected.m_Connected;
                        }
                    }
                    if (ReadUndoComponent<AccessLane>(waypoint) is AccessLane access)
                    {
                        usesNetwork |= BelongsToUndoNetwork(access.m_Lane, affected);
                    }
                    if (ReadUndoComponent<RouteLane>(waypoint) is RouteLane lane)
                    {
                        usesNetwork |= BelongsToUndoNetwork(lane.m_StartLane, affected) ||
                            BelongsToUndoNetwork(lane.m_EndLane, affected);
                    }
                }

                DynamicBuffer<RouteSegment> segments = EntityManager.GetBuffer<RouteSegment>(route, true);
                foreach (RouteSegment item in segments)
                {
                    if (usesNetwork || m_NetworkUndoRoutes.Contains(route)) break;
                    if (!EntityManager.HasBuffer<Game.Pathfind.PathElement>(item.m_Segment))
                    {
                        continue;
                    }

                    DynamicBuffer<Game.Pathfind.PathElement> path =
                        EntityManager.GetBuffer<Game.Pathfind.PathElement>(item.m_Segment, true);
                    foreach (Game.Pathfind.PathElement element in path)
                    {
                        if (BelongsToUndoNetwork(element.m_Target, affected))
                        {
                            usesNetwork = true;
                            break;
                        }
                    }
                }
                if (usesNetwork)
                {
                    m_NetworkUndoRoutes.Add(route);
                }
            }
        }

        private bool BelongsToUndoNetwork(Entity entity, HashSet<Entity> networks)
        {
            for (int depth = 0; depth < 8 && entity != Entity.Null && EntityManager.Exists(entity); depth++)
            {
                if (networks.Contains(entity))
                {
                    return true;
                }
                if (ReadUndoComponent<Attached>(entity) is Attached attached &&
                    attached.m_Parent != Entity.Null && networks.Contains(attached.m_Parent))
                {
                    return true;
                }
                entity = ReadUndoComponent<Owner>(entity)?.m_Owner ?? Entity.Null;
            }
            return false;
        }

        private void RestoreNetworkTransitForUndo(Dictionary<Entity, Entity> replacements, HashSet<Entity> networks)
        {
            foreach (NetworkStopSnapshot saved in m_NetworkUndoStops.Values)
            {
                if (IsEntityUsable(saved.Original, allowOverridden: true))
                {
                    if (saved.Attachment is Attached previousAttachment &&
                        replacements.TryGetValue(previousAttachment.m_Parent, out Entity restoredParent))
                    {
                        previousAttachment.m_OldParent = previousAttachment.m_Parent;
                        previousAttachment.m_Parent = restoredParent;
                        SetUndoComponent(saved.Original, previousAttachment);
                        EntityManager.AddComponent<Updated>(saved.Original);
                    }
                    if (saved.Owner is Owner previousOwner &&
                        replacements.TryGetValue(previousOwner.m_Owner, out Entity remappedOwner))
                    {
                        previousOwner.m_Owner = remappedOwner;
                        SetUndoComponent(saved.Original, previousOwner);
                    }
                    replacements[saved.Original] = saved.Original;
                    continue;
                }

                if (!IsEntityUsable(saved.Prefab) || !EntityManager.HasComponent<ObjectData>(saved.Prefab))
                {
                    continue;
                }
                Attached? attachment = saved.Attachment;
                Owner? owner = saved.Owner;
                if (attachment is Attached attached)
                {
                    attached.m_Parent = RemapUndoNetworkParent(attached.m_Parent, replacements);
                    attached.m_OldParent = Entity.Null;
                    if (!IsEntityUsable(attached.m_Parent, allowOverridden: true))
                    {
                        continue;
                    }
                    attachment = attached;
                }
                if (owner is Owner owned)
                {
                    owned.m_Owner = RemapUndoNetworkParent(owned.m_Owner, replacements);
                    if (!IsEntityUsable(owned.m_Owner, allowOverridden: true))
                    {
                        continue;
                    }
                    owner = owned;
                }

                EntityArchetype archetype = EntityManager.GetComponentData<ObjectData>(saved.Prefab).m_Archetype;
                if (archetype.Equals(default(EntityArchetype)))
                {
                    continue;
                }
                using NativeArray<Entity> created = EntityManager.CreateEntity(archetype, 1, Allocator.Temp);
                Entity stop = created[0];
                if (!EntityManager.HasComponent<PrefabRef>(stop) ||
                    !EntityManager.HasComponent<ObjectTransform>(stop) ||
                    !EntityManager.HasComponent<TransportStop>(stop))
                {
                    EntityManager.DestroyEntity(stop);
                    continue;
                }

                EntityManager.SetComponentData(stop, new PrefabRef(saved.Prefab));
                EntityManager.SetComponentData(stop, saved.Transform);
                if (attachment is Attached restoredAttachment) SetUndoComponent(stop, restoredAttachment);
                if (owner is Owner restoredOwner) SetUndoComponent(stop, restoredOwner);
                if (saved.Elevation is ObjectElevation elevation) SetUndoComponent(stop, elevation);
                if (saved.Seed is PseudoRandomSeed seed) SetUndoComponent(stop, seed);
                if (saved.Native) EntityManager.AddComponent<Native>(stop);
                EntityManager.AddComponent<Created>(stop);
                EntityManager.AddComponent<Updated>(stop);
                EntityManager.AddComponent<BatchesUpdated>(stop);
                replacements[saved.Original] = stop;
            }

            foreach (KeyValuePair<Entity, Entity> item in m_NetworkUndoWaypointStops)
            {
                if (IsEntityUsable(item.Key) && replacements.TryGetValue(item.Value, out Entity stop) &&
                    ReadUndoComponent<Connected>(item.Key) is Connected current &&
                    (current.m_Connected == item.Value || current.m_Connected == Entity.Null ||
                     !IsEntityUsable(current.m_Connected)))
                {
                    if (IsEntityUsable(current.m_Connected) && current.m_Connected != stop &&
                        EntityManager.HasBuffer<ConnectedRoute>(current.m_Connected))
                    {
                        DynamicBuffer<ConnectedRoute> oldRoutes = EntityManager.GetBuffer<ConnectedRoute>(current.m_Connected);
                        for (int i = oldRoutes.Length - 1; i >= 0; i--)
                        {
                            if (oldRoutes[i].m_Waypoint == item.Key) oldRoutes.RemoveAt(i);
                        }
                    }
                    current.m_Connected = stop;
                    EntityManager.SetComponentData(item.Key, current);
                    // Updated waypoints do not get the Created-only reference
                    // initialization. Rebuild the inverse stop link explicitly.
                    DynamicBuffer<ConnectedRoute> routes = EntityManager.HasBuffer<ConnectedRoute>(stop)
                        ? EntityManager.GetBuffer<ConnectedRoute>(stop)
                        : EntityManager.AddBuffer<ConnectedRoute>(stop);
                    bool linked = false;
                    foreach (ConnectedRoute route in routes) linked |= route.m_Waypoint == item.Key;
                    if (!linked) routes.Add(new ConnectedRoute { m_Waypoint = item.Key });
                    EntityManager.AddComponent<Updated>(item.Key);
                }
            }

            World.GetOrCreateSystemManaged<AreaBulldozerNetworkConnectionSystem>()
                .Track(networks, m_NetworkUndoRoutes);
            ClearNetworkTransitUndo();
        }

        private static Entity RemapUndoNetworkParent(Entity original, Dictionary<Entity, Entity> replacements) =>
            replacements.TryGetValue(original, out Entity replacement) ? replacement : original;
    }
}
