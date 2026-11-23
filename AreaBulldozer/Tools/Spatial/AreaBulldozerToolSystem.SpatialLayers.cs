using AreaBulldozer.Diagnostics;
using Game.Objects;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    // Incrementally maintained point layers of the spatial preview index.
    //
    // Trees, props, buildings, spawn locations and missing-asset objects make up
    // almost all indexed objects (e.g. 229,000 of 229,600). Instead of rebuilding
    // them completely whenever something changes, every ECS chunk remembers which
    // entities it contributed. A sync only re-indexes chunks whose entities were
    // added/removed (order change) or whose Transform was written since the last
    // sync, and drops chunks that no longer exist. Unchanged chunks cost one
    // version comparison.
    public partial class AreaBulldozerToolSystem
    {
        private sealed class SpatialChunkRecord
        {
            public readonly List<Entity> Entities = new();
            public readonly List<long> Keys = new();
            public bool Seen;
        }

        private sealed class SpatialChunkLayer
        {
            public SpatialChunkLayer(
                SpatialCandidateKind kind,
                EntityQuery query,
                float cellSize)
            {
                Kind = kind;
                Query = query;
                Index = new SpatialCandidateIndex(cellSize);
            }

            public SpatialCandidateKind Kind { get; }

            public EntityQuery Query { get; }

            public SpatialCandidateIndex Index { get; }

            public readonly Dictionary<ulong, SpatialChunkRecord> Chunks = new();

            public bool Active;

            public uint SyncedVersion;

            public int Count;

            public void Clear()
            {
                Index.Clear();
                Chunks.Clear();
                Count = 0;
                SyncedVersion = 0;
            }
        }

        private SpatialChunkLayer m_VegetationLayer;
        private SpatialChunkLayer m_BuildingLayer;
        private SpatialChunkLayer m_StaticObjectLayer;
        private SpatialChunkLayer m_SpawnLocationLayer;
        private SpatialChunkLayer m_MissingAssetLayer;
        private SpatialChunkLayer[] m_SpatialLayers;

        private readonly List<SpatialChunkRecord> m_ChangedChunkRecords = new();
        private readonly List<ArchetypeChunk> m_ChangedChunks = new();
        private readonly List<ulong> m_VanishedChunkIds = new();
        private readonly Dictionary<long, HashSet<Entity>> m_RemovalGroups = new();
        private readonly List<HashSet<Entity>> m_RemovalSetPool = new();

        // True while a full rebuild runs: only then the per-category diagnostic
        // counters are updated (they describe the last full build).
        private bool m_SpatialCountingCategories;

        private void InitializeSpatialLayers()
        {
            m_VegetationLayer = new(SpatialCandidateKind.Vegetation, m_PlantQuery, kSpatialCellSize);
            m_BuildingLayer = new(SpatialCandidateKind.Building, m_BuildingQuery, kSpatialCellSize);
            m_StaticObjectLayer = new(SpatialCandidateKind.StaticObject, m_StaticObjectQuery, kSpatialCellSize);
            m_SpawnLocationLayer = new(SpatialCandidateKind.SpawnLocation, m_SpawnLocationQuery, kSpatialCellSize);
            m_MissingAssetLayer = new(SpatialCandidateKind.MissingAsset, m_MissingAssetObjectQuery, kSpatialCellSize);

            m_SpatialLayers = new[]
            {
                m_VegetationLayer,
                m_BuildingLayer,
                m_StaticObjectLayer,
                m_SpawnLocationLayer,
                m_MissingAssetLayer
            };
        }

        private void ClearSpatialLayers()
        {
            if (m_SpatialLayers == null)
            {
                return;
            }

            foreach (SpatialChunkLayer layer in m_SpatialLayers)
            {
                layer.Clear();
                layer.Active = false;
            }
        }

        private int SpatialLayerObjectCount
        {
            get
            {
                int count = 0;
                if (m_SpatialLayers != null)
                {
                    foreach (SpatialChunkLayer layer in m_SpatialLayers)
                    {
                        if (layer.Active)
                        {
                            count += layer.Count;
                        }
                    }
                }

                return count;
            }
        }

        private int SpatialLayerCellCount
        {
            get
            {
                int count = 0;
                if (m_SpatialLayers != null)
                {
                    foreach (SpatialChunkLayer layer in m_SpatialLayers)
                    {
                        if (layer.Active)
                        {
                            count += layer.Index.CellCount;
                        }
                    }
                }

                return count;
            }
        }

        private void AppendSpatialLayerCandidates(
            float2 center,
            float radius,
            List<SpatialCandidate> target)
        {
            if (m_SpatialLayers == null)
            {
                return;
            }

            foreach (SpatialChunkLayer layer in m_SpatialLayers)
            {
                if (layer.Active)
                {
                    layer.Index.AppendQuery(center, radius, target);
                }
            }
        }

        // Full build of every active layer (part of RebuildSpatialIndex).
        private void BuildSpatialLayers()
        {
            m_VegetationLayer.Active = m_SpatialAssetVegetationIndexed;
            m_BuildingLayer.Active = true;
            m_StaticObjectLayer.Active = true;
            m_SpawnLocationLayer.Active = true;
            m_MissingAssetLayer.Active = m_SpatialMissingAssetsEnabled;

            m_SpatialCountingCategories = true;
            try
            {
                foreach (SpatialChunkLayer layer in m_SpatialLayers)
                {
                    layer.Clear();
                    if (layer.Active)
                    {
                        SyncSpatialLayer(layer, fullBuild: true);
                    }
                }
            }
            finally
            {
                m_SpatialCountingCategories = false;
            }
        }

        // Returns the number of re-indexed or removed chunks.
        private int SyncSpatialLayers()
        {
            int changed = 0;
            foreach (SpatialChunkLayer layer in m_SpatialLayers)
            {
                if (layer.Active)
                {
                    changed += SyncSpatialLayer(layer, fullBuild: false);
                }
            }

            return changed;
        }

        private int SyncSpatialLayer(
            SpatialChunkLayer layer,
            bool fullBuild)
        {
            // Jobs that write these chunks must be finished before reading them.
            layer.Query.CompleteDependency();

            EntityTypeHandle entityHandle = GetEntityTypeHandle();
            ComponentTypeHandle<Transform> transformHandle =
                GetComponentTypeHandle<Transform>(true);
            ComponentTypeHandle<Game.Prefabs.PrefabRef> prefabRefHandle =
                GetComponentTypeHandle<Game.Prefabs.PrefabRef>(true);

            // For these two layers the query already excludes every entity type
            // that IsMissingAssetObject rejects (buildings, nets, prefabs,
            // effects, live paths), so the missing check reduces to the cached
            // prefab lookup. This avoids ~10 component checks per object on the
            // up to 230,000 trees and objects when the missing-assets filter is on.
            bool fastMissingCheck =
                m_SpatialMissingAssetsEnabled &&
                (layer.Kind == SpatialCandidateKind.MissingAsset ||
                 layer.Kind == SpatialCandidateKind.Vegetation);

            uint lastVersion = layer.SyncedVersion;

            m_ChangedChunkRecords.Clear();
            m_ChangedChunks.Clear();
            m_VanishedChunkIds.Clear();

            foreach (SpatialChunkRecord record in layer.Chunks.Values)
            {
                record.Seen = false;
            }

            using NativeArray<ArchetypeChunk> chunks =
                layer.Query.ToArchetypeChunkArray(Allocator.Temp);

            // Pass 1: find changed and new chunks.
            for (int index = 0; index < chunks.Length; index++)
            {
                ArchetypeChunk chunk = chunks[index];
                ulong chunkId = chunk.SequenceNumber;

                if (layer.Chunks.TryGetValue(chunkId, out SpatialChunkRecord record))
                {
                    record.Seen = true;

                    if (!fullBuild &&
                        !chunk.DidOrderChange(lastVersion) &&
                        !chunk.DidChange(ref transformHandle, lastVersion))
                    {
                        continue;
                    }
                }
                else
                {
                    record = new SpatialChunkRecord { Seen = true };
                    layer.Chunks.Add(chunkId, record);
                }

                m_ChangedChunkRecords.Add(record);
                m_ChangedChunks.Add(chunk);
            }

            foreach (KeyValuePair<ulong, SpatialChunkRecord> pair in layer.Chunks)
            {
                if (!pair.Value.Seen)
                {
                    m_VanishedChunkIds.Add(pair.Key);
                }
            }

            // Pass 2: remove everything that is going to be re-indexed or that
            // no longer exists. All removals happen before any addition, so an
            // entity that moved to another chunk is not removed again after it
            // was added from its new chunk.
            foreach (ulong chunkId in m_VanishedChunkIds)
            {
                RemoveSpatialChunkRecord(layer, layer.Chunks[chunkId]);
                layer.Chunks.Remove(chunkId);
            }

            foreach (SpatialChunkRecord record in m_ChangedChunkRecords)
            {
                RemoveSpatialChunkRecord(layer, record);
            }

            // Pass 3: index the changed and new chunks.
            for (int index = 0; index < m_ChangedChunks.Count; index++)
            {
                ArchetypeChunk chunk = m_ChangedChunks[index];
                SpatialChunkRecord record = m_ChangedChunkRecords[index];

                NativeArray<Entity> entities = chunk.GetNativeArray(entityHandle);
                NativeArray<Transform> transforms = chunk.GetNativeArray(ref transformHandle);

                bool chunkHasPrefabRef = fastMissingCheck && chunk.Has(ref prefabRefHandle);
                NativeArray<Game.Prefabs.PrefabRef> prefabRefs = chunkHasPrefabRef
                    ? chunk.GetNativeArray(ref prefabRefHandle)
                    : default;

                for (int entityIndex = 0; entityIndex < entities.Length; entityIndex++)
                {
                    Entity entity = entities[entityIndex];
                    float3 position = transforms[entityIndex].m_Position;

                    // 0 = unknown (full check), 1 = not missing, 2 = missing.
                    int missingState = 0;
                    if (fastMissingCheck)
                    {
                        missingState = chunkHasPrefabRef && IsMissingPrefab(prefabRefs[entityIndex].m_Prefab)
                            ? 2
                            : 1;
                    }

                    if (!math.all(math.isfinite(position)) ||
                        !TryCreateLayerCandidate(layer.Kind, entity, position.xz, missingState, out SpatialCandidate candidate))
                    {
                        continue;
                    }

                    long key = layer.Index.AddPoint(candidate);
                    record.Entities.Add(entity);
                    record.Keys.Add(key);
                    layer.Count++;

                    if (m_SpatialCountingCategories)
                    {
                        CountSpatialCandidate(candidate);
                    }
                }
            }

            layer.SyncedVersion = EntityManager.GlobalSystemVersion;

            int changed = m_ChangedChunks.Count + m_VanishedChunkIds.Count;

            m_ChangedChunkRecords.Clear();
            m_ChangedChunks.Clear();
            m_VanishedChunkIds.Clear();

            return changed;
        }

        private void RemoveSpatialChunkRecord(
            SpatialChunkLayer layer,
            SpatialChunkRecord record)
        {
            if (record.Entities.Count == 0)
            {
                return;
            }

            for (int index = 0; index < record.Entities.Count; index++)
            {
                long key = record.Keys[index];
                if (!m_RemovalGroups.TryGetValue(key, out HashSet<Entity> group))
                {
                    int last = m_RemovalSetPool.Count - 1;
                    if (last >= 0)
                    {
                        group = m_RemovalSetPool[last];
                        m_RemovalSetPool.RemoveAt(last);
                    }
                    else
                    {
                        group = new HashSet<Entity>();
                    }

                    m_RemovalGroups.Add(key, group);
                }

                group.Add(record.Entities[index]);
            }

            foreach (KeyValuePair<long, HashSet<Entity>> pair in m_RemovalGroups)
            {
                layer.Index.RemoveFromCell(pair.Key, pair.Value);
                pair.Value.Clear();
                m_RemovalSetPool.Add(pair.Value);
            }

            m_RemovalGroups.Clear();

            layer.Count -= record.Entities.Count;
            record.Entities.Clear();
            record.Keys.Clear();
        }

        // Same acceptance rules as the former full rebuild of these categories.
        // missingState: 0 = unknown (full IsMissingAssetObject check),
        // 1 = known not missing, 2 = known missing (see SyncSpatialLayer).
        private bool TryCreateLayerCandidate(
            SpatialCandidateKind kind,
            Entity entity,
            float2 position,
            int missingState,
            out SpatialCandidate candidate)
        {
            candidate = default;

            // Most objects are not missing: reject them before any other check.
            if (kind == SpatialCandidateKind.MissingAsset && missingState == 1)
            {
                return false;
            }

            if (kind == SpatialCandidateKind.Vegetation)
            {
                bool isMissing = missingState == 0
                    ? m_SpatialMissingAssetsEnabled && IsMissingAssetObject(entity)
                    : missingState == 2;

                if ((m_PendingDeletion != null && m_PendingDeletion.Contains(entity)) ||
                    isMissing)
                {
                    return false;
                }

                candidate = new SpatialCandidate(
                    entity,
                    position,
                    SpatialCandidateKind.Vegetation,
                    StaticObjectCategory.None);

                return true;
            }

            if (!IsEntityUsable(
                    entity,
                    allowOverridden: kind == SpatialCandidateKind.MissingAsset))
            {
                return false;
            }

            if (m_PendingDeletion != null &&
                m_PendingDeletion.Contains(entity))
            {
                return false;
            }

            bool missingAsset = missingState == 0
                ? m_SpatialMissingAssetsEnabled && IsMissingAssetObject(entity)
                : missingState == 2;
            if (kind == SpatialCandidateKind.MissingAsset ? !missingAsset : missingAsset)
            {
                return false;
            }

            if (kind == SpatialCandidateKind.Building &&
                !IsRootBuildingEntity(entity))
            {
                return false;
            }

            if (kind == SpatialCandidateKind.StaticObject &&
                !IsActivityLocationTarget(entity) &&
                (TryGetPointMarkerPrefab(entity, out _) || !IsStaticObjectEntity(entity)))
            {
                return false;
            }

            if (kind == SpatialCandidateKind.SpawnLocation &&
                !IsSpawnPointMarker(entity))
            {
                return false;
            }

            StaticObjectCategory staticCategory =
                kind == SpatialCandidateKind.StaticObject
                    ? GetStaticObjectCategory(entity)
                    : StaticObjectCategory.None;

            candidate = new SpatialCandidate(
                entity,
                position,
                kind,
                staticCategory);

            return true;
        }

        // True if roads, paths, areas, asset lanes or the sub-object owners used
        // by the main index had entities added or removed since it was built.
        private bool HaveMainSpatialSourcesChanged()
        {
            uint version = m_SpatialMainIndexVersion;

            return
                QueryHasOrderChange(m_NetEdgeQuery, version) ||
                QueryHasOrderChange(m_SurfaceAreaQuery, version) ||
                ((m_SpatialAssetVegetationIndexed || m_SpatialLinkedPropsIndexed) &&
                    QueryHasOrderChange(m_SubObjectOwnerQuery, version)) ||
                (m_SpatialAssetLanesEnabled &&
                    QueryHasOrderChange(m_SubLaneOwnerQuery, version));
        }

        private static bool QueryHasOrderChange(
            EntityQuery query,
            uint version)
        {
            using NativeArray<ArchetypeChunk> chunks =
                query.ToArchetypeChunkArray(Allocator.Temp);

            for (int index = 0; index < chunks.Length; index++)
            {
                if (chunks[index].DidOrderChange(version))
                {
                    return true;
                }
            }

            return false;
        }

        // Brings the index up to date without a full rebuild. The main index
        // (roads, paths, areas, asset lanes) is rebuilt only when its sources
        // changed and allowMainRebuild is set. Returns true if anything changed.
        private bool SyncSpatialIndex(bool allowMainRebuild)
        {
            if (!m_SpatialIndexReady)
            {
                return false;
            }

            long syncStart = PerfTrace.Begin();

            int changedChunks = SyncSpatialLayers();

            bool mainRebuilt = false;
            if (allowMainRebuild && HaveMainSpatialSourcesChanged())
            {
                RebuildMainSpatialIndex();
                mainRebuilt = true;
            }

            m_SpatialSyncRequested = false;

            PerfTrace.End(PerfTrace.SpatialSync, syncStart, changedChunks);

            if (Mod.DiagnosticLoggingEnabled &&
                (changedChunks > 0 || mainRebuilt))
            {
                SafeLogInfo(
                    $"Spatial preview index synchronized: {changedChunks} chunks updated" +
                    (mainRebuilt ? ", roads/paths/areas rebuilt" : string.Empty) +
                    $", {m_SpatialIndexedObjectCount + SpatialLayerObjectCount} objects.");
            }

            return changedChunks > 0 || mainRebuilt;
        }

        // For changes the tool made itself (e.g. undo): synchronize on the next
        // query instead of rebuilding everything.
        private void RequestSpatialIndexSync()
        {
            m_SpatialSyncRequested = true;
        }
    }
}
