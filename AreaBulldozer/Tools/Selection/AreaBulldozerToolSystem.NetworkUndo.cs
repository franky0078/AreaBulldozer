using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using NetNode = Game.Net.Node;
using NetElevation = Game.Net.Elevation;
using NetComposition = Game.Net.Composition;
using BuildOrder = Game.Net.BuildOrder;
using Upgraded = Game.Net.Upgraded;
using Road = Game.Net.Road;
using Roundabout = Game.Net.Roundabout;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private struct NetworkNodeSnapshot
        {
            public Entity Prefab;
            public NetNode Node;
            public NetElevation? Elevation;
            public Upgraded? Upgrade;
            public PseudoRandomSeed? Seed;
            public Roundabout? Roundabout;
            public Game.Net.Fixed? Fixed;
            public bool Native;
            public Game.Tools.EditorContainer? EditorContainer;
        }

        private struct NetworkSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public Edge Edge;
            public Curve Curve;
            public NetComposition Composition;
            public BuildOrder? BuildOrder;
            public NetElevation? Elevation;
            public Upgraded? Upgrade;
            public PseudoRandomSeed? Seed;
            public Road? Road;
            public NetCondition? Condition;
            public Game.Net.Fixed? Fixed;
            public bool Native;
            public Game.Tools.EditorContainer? EditorContainer;
            public ConnectedNode[] Connections;
            public bool CarriesElectricity;
            public bool HasTramTrack;
        }

        private readonly List<NetworkSnapshot> m_NetworkUndo = new();
        private readonly Dictionary<Entity, NetworkNodeSnapshot> m_NetworkUndoNodes = new();

        private void RecordNetworkForUndo(Entity entity)
        {
            if (!EntityManager.HasComponent<Edge>(entity) ||
                !EntityManager.HasComponent<Curve>(entity) ||
                !EntityManager.HasComponent<NetComposition>(entity) ||
                !TryGetUndoPrefab<NetData>(entity, out Entity prefab))
            {
                return;
            }

            Edge edge = EntityManager.GetComponentData<Edge>(entity);
            if (!CaptureNetworkUndoNode(edge.m_Start, prefab) || !CaptureNetworkUndoNode(edge.m_End, prefab))
            {
                return;
            }

            m_NetworkUndo.Add(new NetworkSnapshot
            {
                Original = entity,
                Prefab = prefab,
                Edge = edge,
                Curve = EntityManager.GetComponentData<Curve>(entity),
                Composition = EntityManager.GetComponentData<NetComposition>(entity),
                BuildOrder = ReadUndoComponent<BuildOrder>(entity),
                Elevation = ReadUndoComponent<NetElevation>(entity),
                Upgrade = ReadUndoComponent<Upgraded>(entity),
                Seed = ReadUndoComponent<PseudoRandomSeed>(entity),
                Road = ReadUndoComponent<Road>(entity),
                Condition = ReadUndoComponent<NetCondition>(entity),
                Fixed = ReadUndoComponent<Game.Net.Fixed>(entity),
                Native = EntityManager.HasComponent<Native>(entity),
                EditorContainer = ReadUndoComponent<Game.Tools.EditorContainer>(entity),
                CarriesElectricity = EntityManager.HasComponent<Game.Net.ElectricityConnection>(entity),
                HasTramTrack = EntityManager.HasComponent<Game.Net.TramTrack>(entity),
                Connections = EntityManager.HasBuffer<ConnectedNode>(entity)
                    ? CopyUndoBuffer<ConnectedNode>(entity)
                    : Array.Empty<ConnectedNode>()
            });
        }

        private bool CaptureNetworkUndoNode(Entity entity, Entity edgePrefab)
        {
            if (m_NetworkUndoNodes.ContainsKey(entity))
            {
                return true;
            }

            if (!IsEntityUsable(entity, allowOverridden: true) ||
                !EntityManager.HasComponent<NetNode>(entity))
            {
                return false;
            }

            // A junction can use a specialized prefab. The incident edge's
            // network prefab still provides a valid node archetype.
            if (!TryGetUndoPrefab<NetData>(entity, out Entity prefab))
            {
                prefab = edgePrefab;
            }

            m_NetworkUndoNodes.Add(entity, new NetworkNodeSnapshot
            {
                Prefab = prefab,
                Node = EntityManager.GetComponentData<NetNode>(entity),
                Elevation = ReadUndoComponent<NetElevation>(entity),
                Upgrade = ReadUndoComponent<Upgraded>(entity),
                Seed = ReadUndoComponent<PseudoRandomSeed>(entity),
                Roundabout = ReadUndoComponent<Roundabout>(entity),
                Fixed = ReadUndoComponent<Game.Net.Fixed>(entity),
                Native = EntityManager.HasComponent<Native>(entity),
                EditorContainer = ReadUndoComponent<Game.Tools.EditorContainer>(entity)
            });
            return true;
        }

        private bool IsNetworkDeletionPending()
        {
            foreach (NetworkSnapshot saved in m_NetworkUndo)
            {
                if (m_PendingDeletion.Contains(saved.Original) ||
                    m_PendingDeletion.Contains(saved.Edge.m_Start) ||
                    m_PendingDeletion.Contains(saved.Edge.m_End))
                {
                    return true;
                }
            }

            return false;
        }

        private Dictionary<Entity, Entity> RestoreNetworksForUndo(ref int restored, ref int skipped)
        {
            var replacements = new Dictionary<Entity, Entity>();
            var created = new List<Entity>();
            var accepted = new List<NetworkSnapshot>();
            var updated = new HashSet<Entity>();
            try
            {
                // Validate the complete edge before creating either endpoint.
                // Node identity, including separate nodes at the same position,
                // is preserved through the map shared by every restored edge.
                foreach (NetworkSnapshot saved in m_NetworkUndo)
                {
                    if (IsEntityUsable(saved.Original, allowOverridden: true))
                    {
                        replacements[saved.Original] = saved.Original;
                        skipped++;
                        continue;
                    }

                    if (!CanRestoreNetwork(saved))
                    {
                        skipped++;
                        continue;
                    }

                    Entity start = RestoreNetworkUndoNode(saved.Edge.m_Start, replacements, created);
                    Entity end = RestoreNetworkUndoNode(saved.Edge.m_End, replacements, created);
                    EntityArchetype archetype = EntityManager.GetComponentData<NetData>(saved.Prefab).m_EdgeArchetype;
                    Entity edge = CreateNetworkUndoEntity(archetype, created);
                    if (!EntityManager.HasComponent<Edge>(edge) ||
                        !EntityManager.HasComponent<Curve>(edge) ||
                        !EntityManager.HasComponent<NetComposition>(edge) ||
                        !EntityManager.HasComponent<PrefabRef>(edge))
                    {
                        throw new InvalidOperationException("The network edge prefab has an incomplete archetype.");
                    }

                    EntityManager.SetComponentData(edge, new PrefabRef(saved.Prefab));
                    EntityManager.SetComponentData(edge, new Edge { m_Start = start, m_End = end });
                    EntityManager.SetComponentData(edge, GetRestoredNetworkCurve(saved, start, end));
                    EntityManager.SetComponentData(edge, saved.Composition);
                    if (saved.BuildOrder is BuildOrder buildOrder)
                    {
                        SetUndoComponent(edge, buildOrder);
                    }
                    RestoreNetworkUndoProperties(edge, saved.Elevation, saved.Upgrade, saved.Seed);
                    if (saved.Native) EntityManager.AddComponent<Native>(edge);
                    if (saved.EditorContainer is Game.Tools.EditorContainer editorContainer)
                    {
                        SetUndoComponent(edge, editorContainer);
                    }
                    if (saved.Road is Road road)
                    {
                        SetUndoComponent(edge, road);
                    }
                    if (saved.Condition is NetCondition condition)
                    {
                        SetUndoComponent(edge, condition);
                    }
                    if (saved.Fixed is Game.Net.Fixed fixedData)
                    {
                        SetUndoComponent(edge, fixedData);
                    }
                    if (saved.CarriesElectricity)
                    {
                        // GenerateEdgesSystem adds this tag dynamically. It
                        // is not guaranteed to be in the prefab archetype.
                        // Created then lets ElectricityEdgeGraphSystem build
                        // fresh flow nodes and reconnect existing endpoints.
                        EntityManager.AddComponent<Game.Net.ElectricityConnection>(edge);
                    }
                    if (saved.HasTramTrack)
                    {
                        EntityManager.AddComponent<Game.Net.TramTrack>(edge);
                    }
                    replacements.Add(saved.Original, edge);
                    accepted.Add(saved);
                    updated.Add(start);
                    updated.Add(end);
                    updated.Add(edge);
                }

                // Complete links after all nodes and edges exist. The native
                // geometry, lane, utility and aggregate systems rebuild from
                // Created/Updated instead of inheriting deleted lane entities.
                foreach (NetworkSnapshot saved in accepted)
                {
                    Entity edge = replacements[saved.Original];
                    Edge data = EntityManager.GetComponentData<Edge>(edge);
                    AddRestoredNetworkEdge(data.m_Start, edge, updated);
                    AddRestoredNetworkEdge(data.m_End, edge, updated);
                    RestoreNetworkConnections(saved, edge, replacements, updated);
                }
                foreach (Entity entity in created)
                {
                    EntityManager.AddComponent<Created>(entity);
                }
                foreach (Entity entity in updated)
                {
                    if (IsEntityUsable(entity, allowOverridden: true))
                    {
                        EntityManager.AddComponent<Updated>(entity);
                        EntityManager.AddComponent<BatchesUpdated>(entity);
                    }
                }
                restored += accepted.Count;
            }
            catch
            {
                // Existing junctions must not retain references to a failed
                // restoration. Remove our links before destroying our entities.
                foreach (NetworkSnapshot saved in accepted)
                {
                    Entity edge = replacements[saved.Original];
                    Edge data = EntityManager.GetComponentData<Edge>(edge);
                    RemoveRestoredNetworkEdge(data.m_Start, edge);
                    RemoveRestoredNetworkEdge(data.m_End, edge);
                }
                for (int i = created.Count - 1; i >= 0; i--)
                {
                    EntityManager.DestroyEntity(created[i]);
                }
                throw;
            }

            if (accepted.Count > 0)
            {
                RestoreNetworkTransitForUndo(replacements, updated);
            }
            else
            {
                ClearNetworkTransitUndo();
            }
            m_NetworkUndo.Clear();
            m_NetworkUndoNodes.Clear();
            return replacements;
        }

        private bool CanRestoreNetwork(NetworkSnapshot saved)
        {
            return IsEntityUsable(saved.Prefab) &&
                EntityManager.HasComponent<NetData>(saved.Prefab) &&
                (saved.Composition.m_Edge == Entity.Null || IsEntityUsable(saved.Composition.m_Edge)) &&
                (saved.Composition.m_StartNode == Entity.Null || IsEntityUsable(saved.Composition.m_StartNode)) &&
                (saved.Composition.m_EndNode == Entity.Null || IsEntityUsable(saved.Composition.m_EndNode)) &&
                !EntityManager.GetComponentData<NetData>(saved.Prefab).m_EdgeArchetype.Equals(default(EntityArchetype)) &&
                CanRestoreNetworkUndoNode(saved.Edge.m_Start) &&
                CanRestoreNetworkUndoNode(saved.Edge.m_End);
        }

        private bool CanRestoreNetworkUndoNode(Entity original)
        {
            if (!m_NetworkUndoNodes.TryGetValue(original, out NetworkNodeSnapshot saved))
            {
                return false;
            }

            if (IsEntityUsable(original, allowOverridden: true))
            {
                return EntityManager.HasComponent<NetNode>(original);
            }

            return IsEntityUsable(saved.Prefab) &&
                EntityManager.HasComponent<NetData>(saved.Prefab) &&
                !EntityManager.GetComponentData<NetData>(saved.Prefab).m_NodeArchetype.Equals(default(EntityArchetype));
        }

        private Curve GetRestoredNetworkCurve(NetworkSnapshot saved, Entity start, Entity end)
        {
            Curve curve = saved.Curve;
            float3 startDelta = EntityManager.GetComponentData<NetNode>(start).m_Position -
                m_NetworkUndoNodes[saved.Edge.m_Start].Node.m_Position;
            float3 endDelta = EntityManager.GetComponentData<NetNode>(end).m_Position -
                m_NetworkUndoNodes[saved.Edge.m_End].Node.m_Position;
            if (math.lengthsq(startDelta) > 0f || math.lengthsq(endDelta) > 0f)
            {
                // Deleting adjacent segments can move a surviving junction.
                // Keep its current connections and align our curve to it.
                curve.m_Bezier.a += startDelta;
                curve.m_Bezier.b += startDelta;
                curve.m_Bezier.c += endDelta;
                curve.m_Bezier.d += endDelta;
                curve.m_Length = MathUtils.Length(curve.m_Bezier);
            }
            return curve;
        }

        private Entity RestoreNetworkUndoNode(
            Entity original, Dictionary<Entity, Entity> replacements, List<Entity> created)
        {
            if (replacements.TryGetValue(original, out Entity node))
            {
                return node;
            }

            if (IsEntityUsable(original, allowOverridden: true))
            {
                replacements.Add(original, original);
                return original;
            }

            NetworkNodeSnapshot saved = m_NetworkUndoNodes[original];
            node = CreateNetworkUndoEntity(
                EntityManager.GetComponentData<NetData>(saved.Prefab).m_NodeArchetype, created);
            if (!EntityManager.HasComponent<NetNode>(node) || !EntityManager.HasComponent<PrefabRef>(node))
            {
                throw new InvalidOperationException("The network node prefab has an incomplete archetype.");
            }

            EntityManager.SetComponentData(node, new PrefabRef(saved.Prefab));
            EntityManager.SetComponentData(node, saved.Node);
            RestoreNetworkUndoProperties(node, saved.Elevation, saved.Upgrade, saved.Seed);
            if (saved.Native) EntityManager.AddComponent<Native>(node);
            if (saved.EditorContainer is Game.Tools.EditorContainer editorContainer)
            {
                SetUndoComponent(node, editorContainer);
            }
            if (saved.Roundabout is Roundabout roundabout)
            {
                SetUndoComponent(node, roundabout);
            }
            if (saved.Fixed is Game.Net.Fixed fixedData)
            {
                SetUndoComponent(node, fixedData);
            }
            replacements.Add(original, node);
            return node;
        }

        private Entity CreateNetworkUndoEntity(EntityArchetype archetype, List<Entity> created)
        {
            using NativeArray<Entity> result = EntityManager.CreateEntity(archetype, 1, Allocator.Temp);
            Entity entity = result[0];
            created.Add(entity);
            return entity;
        }

        private void RestoreNetworkUndoProperties(
            Entity entity, NetElevation? elevation, Upgraded? upgrade, PseudoRandomSeed? seed)
        {
            if (elevation is NetElevation savedElevation)
            {
                SetUndoComponent(entity, savedElevation);
            }
            if (upgrade is Upgraded savedUpgrade)
            {
                SetUndoComponent(entity, savedUpgrade);
            }
            if (seed is PseudoRandomSeed savedSeed)
            {
                SetUndoComponent(entity, savedSeed);
            }
        }

        private void AddRestoredNetworkEdge(Entity node, Entity edge, HashSet<Entity> updated)
        {
            DynamicBuffer<ConnectedEdge> connections = EntityManager.HasBuffer<ConnectedEdge>(node)
                ? EntityManager.GetBuffer<ConnectedEdge>(node)
                : EntityManager.AddBuffer<ConnectedEdge>(node);
            for (int i = 0; i < connections.Length; i++)
            {
                Entity neighbor = connections[i].m_Edge;
                if (neighbor == edge)
                {
                    return;
                }
                if (IsEntityUsable(neighbor) && EntityManager.HasComponent<Edge>(neighbor))
                {
                    updated.Add(neighbor);
                    Edge data = EntityManager.GetComponentData<Edge>(neighbor);
                    updated.Add(data.m_Start);
                    updated.Add(data.m_End);
                }
            }
            connections.Add(new ConnectedEdge { m_Edge = edge });
        }

        private void RemoveRestoredNetworkEdge(Entity node, Entity edge)
        {
            if (!EntityManager.HasBuffer<ConnectedEdge>(node))
            {
                return;
            }

            DynamicBuffer<ConnectedEdge> connections = EntityManager.GetBuffer<ConnectedEdge>(node);
            for (int i = connections.Length - 1; i >= 0; i--)
            {
                if (connections[i].m_Edge == edge)
                {
                    connections.RemoveAt(i);
                }
            }
        }

        private void RestoreNetworkConnections(
            NetworkSnapshot saved, Entity edge, Dictionary<Entity, Entity> replacements, HashSet<Entity> updated)
        {
            foreach (ConnectedNode original in saved.Connections)
            {
                Entity node = replacements.TryGetValue(original.m_Node, out Entity replacement)
                    ? replacement
                    : original.m_Node;
                if (!IsEntityUsable(node) || !EntityManager.HasComponent<NetNode>(node))
                {
                    continue;
                }

                DynamicBuffer<ConnectedNode> connections = EntityManager.HasBuffer<ConnectedNode>(edge)
                    ? EntityManager.GetBuffer<ConnectedNode>(edge)
                    : EntityManager.AddBuffer<ConnectedNode>(edge);
                ConnectedNode connection = original;
                connection.m_Node = node;
                connections.Add(connection);
                updated.Add(node);
            }
        }
    }
}
