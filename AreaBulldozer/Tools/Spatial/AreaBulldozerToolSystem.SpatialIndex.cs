using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private const float kSpatialCellSize = 64f;
        private const float kSpatialRefreshCheckInterval = 2f;
        private const float kSpatialRefreshIdleSeconds = 0.6f;
        private const float kSpatialRefreshMinRebuildInterval = 15f;

        private float3 m_SpatialRefreshProbePosition;
        private float m_SpatialRefreshProbeRotation;
        private float m_SpatialLastBrushActivityTime;
        private float m_SpatialLastRefreshRebuildTime;

        private SpatialCandidateIndex m_SpatialIndex;

        private bool m_SpatialIndexReady;
        private int m_SpatialIndexedObjectCount;
        private int m_SpatialSourceEntityCount = -1;
        private float m_NextSpatialRefreshCheckTime;

        private int m_SpatialBuildingCount;
        private int m_SpatialRoadCount;
        private int m_SpatialRoadQueryCount;
        private int m_SpatialRejectedRoadOwnerCount;
        private int m_SpatialRoadEndpointFailureCount;
        private int m_SpatialPathCount;
        private int m_SpatialPathQueryCount;
        private int m_SpatialRejectedPathCount;
        private int m_SpatialPathEndpointFailureCount;
        private HashSet<string> m_SpatialRejectedPathPrefabSamples;
        private int m_SpatialRailwayCount;
        private int m_SpatialRailwayQueryCount;
        private int m_SpatialRejectedRailwayCount;
        private int m_SpatialRailwayEndpointFailureCount;
        private HashSet<string> m_SpatialRejectedRailwayPrefabSamples;
        private int m_SpatialSurfaceAreaCount;
        private int m_SpatialSurfaceAreaQueryCount;
        private int m_SpatialInvalidSurfaceAreaCount;
        private int m_SpatialGeneralPropCount;
        private int m_SpatialStreetLightCount;
        private int m_SpatialQuantityObjectCount;
        private int m_SpatialBrandingObjectCount;
        private int m_SpatialActivityLocationCount;
        private int m_SpatialSpawnLocationCount;
        private int m_SpatialAssetLaneCount;
        private int m_SpatialAssetLaneOwnerCount;
        private int m_SpatialAssetLaneFallbackPositionCount;
        private int m_SpatialSubLaneOwnerScanCount;
        private int m_SpatialSubLaneEntryCount;
        private int m_SpatialRejectedNetworkLaneOwnerCount;
        private int m_SpatialMissingLanePrefabCount;

        private void InitializeSpatialIndex()
        {
            m_SpatialIndex =
                new SpatialCandidateIndex(
                    kSpatialCellSize);

            m_SpatialRejectedPathPrefabSamples =
                new();

            m_SpatialRejectedRailwayPrefabSamples =
                new();

            ResetSpatialIndexCounters();
            m_SpatialIndexReady = false;
        }

        private void DisposeSpatialIndex()
        {
            ClearSpatialIndex();

            m_SpatialIndex = null;
            m_SpatialRejectedPathPrefabSamples = null;
            m_SpatialRejectedRailwayPrefabSamples = null;
        }

        private void MarkSpatialIndexStale()
        {
            m_SpatialIndexReady = false;
        }

        private void RebuildSpatialIndex(
            bool logDetails = true)
        {
            if (m_SpatialIndex == null)
            {
                InitializeSpatialIndex();
            }

            ClearSpatialIndex();

            System.Diagnostics.Stopwatch stopwatch =
                System.Diagnostics.Stopwatch.StartNew();

            AddQueryToSpatialIndex(
                m_PlantQuery,
                SpatialCandidateKind.Vegetation);

            AddQueryToSpatialIndex(
                m_BuildingQuery,
                SpatialCandidateKind.Building);

            AddRoadEdgesToSpatialIndex();
            AddPathAndRailwayEdgesToSpatialIndex();
            AddSurfaceAreasToSpatialIndex();

            AddQueryToSpatialIndex(
                m_StaticObjectQuery,
                SpatialCandidateKind.StaticObject);

            AddQueryToSpatialIndex(
                m_SpawnLocationQuery,
                SpatialCandidateKind.SpawnLocation);

            AddAssetLanesToSpatialIndex();

            stopwatch.Stop();

            m_SpatialIndexReady = true;

            m_SpatialSourceEntityCount =
                ComputeSpatialSourceEntityCount();

            m_NextSpatialRefreshCheckTime =
                UnityEngine.Time.unscaledTime +
                kSpatialRefreshCheckInterval;

            // Avoid constructing the large diagnostic
            if (!Mod.DiagnosticLoggingEnabled)
            {
                return;
            }

            if (!logDetails)
            {
                SafeLogInfo(
                    $"Spatial preview index refreshed: " +
                    $"{m_SpatialIndexedObjectCount} objects in " +
                    $"{m_SpatialIndex.CellCount} cells, " +
                    $"{stopwatch.ElapsedMilliseconds} ms.");

                return;
            }

            SafeLogInfo(
                $"Spatial preview index built: " +
                $"{m_SpatialIndexedObjectCount} objects in " +
                $"{m_SpatialIndex.CellCount} cells, " +
                $"{stopwatch.ElapsedMilliseconds} ms. " +
                $"Buildings: {m_SpatialBuildingCount}, " +
                $"roads: {m_SpatialRoadCount}, " +
                $"pedestrian paths: {m_SpatialPathCount}, " +
                $"railway tracks: {m_SpatialRailwayCount}, " +
                $"surfaces and spaces: {m_SpatialSurfaceAreaCount}. " +
                $"Road diagnostics - Edge+Road queried: " +
                $"{m_SpatialRoadQueryCount}, " +
                $"non-aggregate owners rejected: " +
                $"{m_SpatialRejectedRoadOwnerCount}, " +
                $"missing/invalid network nodes: " +
                $"{m_SpatialRoadEndpointFailureCount}. " +
                $"Path diagnostics - Edge+PrefabRef queried: " +
                $"{m_SpatialPathQueryCount}, " +
                $"non-path or subordinate networks rejected: " +
                $"{m_SpatialRejectedPathCount}, " +
                $"missing/invalid network nodes: " +
                $"{m_SpatialPathEndpointFailureCount}. " +
                $"Railway diagnostics - Edge+PrefabRef queried: " +
                $"{m_SpatialRailwayQueryCount}, " +
                $"non-railway or subordinate networks rejected: " +
                $"{m_SpatialRejectedRailwayCount}, " +
                $"missing/invalid network nodes: " +
                $"{m_SpatialRailwayEndpointFailureCount}. " +
                $"Area diagnostics - Area+Node queried: " +
                $"{m_SpatialSurfaceAreaQueryCount}, " +
                $"invalid polygons: " +
                $"{m_SpatialInvalidSurfaceAreaCount}. " +
                $"Static categories - general props: " +
                $"{m_SpatialGeneralPropCount}, " +
                $"street lights: {m_SpatialStreetLightCount}, " +
                $"quantity objects: {m_SpatialQuantityObjectCount}, " +
                $"branding: {m_SpatialBrandingObjectCount}, " +
                $"activity locations: " +
                $"{m_SpatialActivityLocationCount}, " +
                $"spawn locations: " +
                $"{m_SpatialSpawnLocationCount}, " +
                $"asset lanes: " +
                $"{m_SpatialAssetLaneCount}, " +
                $"owners with asset lanes: " +
                $"{m_SpatialAssetLaneOwnerCount}, " +
                $"owner-position fallbacks: " +
                $"{m_SpatialAssetLaneFallbackPositionCount}. " +
                $"SubLane diagnostics - owners scanned: " +
                $"{m_SpatialSubLaneOwnerScanCount}, " +
                $"entries scanned: {m_SpatialSubLaneEntryCount}, " +
                $"network owners excluded: " +
                $"{m_SpatialRejectedNetworkLaneOwnerCount}, " +
                $"missing/other lane prefabs: " +
                $"{m_SpatialMissingLanePrefabCount}.");

            if (m_SpatialPathCount == 0 &&
                m_SpatialRejectedPathPrefabSamples != null &&
                m_SpatialRejectedPathPrefabSamples.Count > 0)
            {
                SafeLogInfo(
                    "Pedestrian-path diagnostic prefab samples: " +
                    string.Join(
                        ", ",
                        m_SpatialRejectedPathPrefabSamples));
            }

            if (m_SpatialRailwayCount == 0 &&
                m_SpatialRejectedRailwayPrefabSamples != null &&
                m_SpatialRejectedRailwayPrefabSamples.Count > 0)
            {
                SafeLogInfo(
                    "Railway diagnostic prefab samples: " +
                    string.Join(
                        ", ",
                        m_SpatialRejectedRailwayPrefabSamples));
            }
        }

        private void EnsureSpatialIndexReady()
        {
            if (!m_SpatialIndexReady)
            {
                RebuildSpatialIndex();
            }
        }

        private int ComputeSpatialSourceEntityCount()
        {
            return
                m_PlantQuery.CalculateEntityCount() +
                m_BuildingQuery.CalculateEntityCount() +
                m_RoadEdgeQuery.CalculateEntityCount() +
                m_NetEdgeQuery.CalculateEntityCount() +
                m_SurfaceAreaQuery.CalculateEntityCount() +
                m_StaticObjectQuery.CalculateEntityCount() +
                m_SpawnLocationQuery.CalculateEntityCount() +
                m_SubLaneOwnerQuery.CalculateEntityCount();
        }

        private void RefreshSpatialIndexIfNeeded()
        {
            if (!m_SpatialIndexReady)
            {
                return;
            }

            float currentTime =
                UnityEngine.Time.unscaledTime;

            bool brushMoved =
                math.distancesq(
                    CurrentPosition,
                    m_SpatialRefreshProbePosition) > 0.01f;

            bool brushRotated =
                math.abs(
                    SquareRotationRadians -
                    m_SpatialRefreshProbeRotation) >
                math.radians(0.05f);

            if (brushMoved ||
                brushRotated)
            {
                m_SpatialRefreshProbePosition =
                    CurrentPosition;

                m_SpatialRefreshProbeRotation =
                    SquareRotationRadians;

                m_SpatialLastBrushActivityTime =
                    currentTime;
            }

            if (currentTime < m_NextSpatialRefreshCheckTime)
            {
                return;
            }

            m_NextSpatialRefreshCheckTime =
                currentTime +
                kSpatialRefreshCheckInterval;

            bool applyHeld =
                m_ApplyAction != null &&
                m_ApplyAction.ReadValue<float>() >= 0.5f;

            if (applyHeld ||
                m_ContinuousDeleteActive ||
                m_LargeSelectionConfirmationPending)
            {
                return;
            }

            if (currentTime -
                m_SpatialLastBrushActivityTime <
                kSpatialRefreshIdleSeconds)
            {
                return;
            }

            if (currentTime -
                m_SpatialLastRefreshRebuildTime <
                kSpatialRefreshMinRebuildInterval)
            {
                return;
            }

            int currentCount =
                ComputeSpatialSourceEntityCount();

            if (currentCount == m_SpatialSourceEntityCount)
            {
                return;
            }

            m_SpatialLastRefreshRebuildTime =
                currentTime;

            RebuildSpatialIndex(
                logDetails: false);

            m_LastPreviewRadius = -1f;
        }

        private void ClearSpatialIndex()
        {
            m_SpatialIndex?.Clear();

            ResetSpatialIndexCounters();
            m_SpatialIndexReady = false;
        }

        private void ResetSpatialIndexCounters()
        {
            m_SpatialIndexedObjectCount = 0;

            m_SpatialBuildingCount = 0;
            m_SpatialRoadCount = 0;
            m_SpatialRoadQueryCount = 0;
            m_SpatialRejectedRoadOwnerCount = 0;
            m_SpatialRoadEndpointFailureCount = 0;
            m_SpatialPathCount = 0;
            m_SpatialPathQueryCount = 0;
            m_SpatialRejectedPathCount = 0;
            m_SpatialPathEndpointFailureCount = 0;
            m_SpatialRejectedPathPrefabSamples?.Clear();
            m_SpatialRailwayCount = 0;
            m_SpatialRailwayQueryCount = 0;
            m_SpatialRejectedRailwayCount = 0;
            m_SpatialRailwayEndpointFailureCount = 0;
            m_SpatialRejectedRailwayPrefabSamples?.Clear();
            m_SpatialSurfaceAreaCount = 0;
            m_SpatialSurfaceAreaQueryCount = 0;
            m_SpatialInvalidSurfaceAreaCount = 0;
            m_SpatialGeneralPropCount = 0;
            m_SpatialStreetLightCount = 0;
            m_SpatialQuantityObjectCount = 0;
            m_SpatialBrandingObjectCount = 0;
            m_SpatialActivityLocationCount = 0;
            m_SpatialSpawnLocationCount = 0;
            m_SpatialAssetLaneCount = 0;
            m_SpatialAssetLaneOwnerCount = 0;
            m_SpatialAssetLaneFallbackPositionCount = 0;
            m_SpatialSubLaneOwnerScanCount = 0;
            m_SpatialSubLaneEntryCount = 0;
            m_SpatialRejectedNetworkLaneOwnerCount = 0;
            m_SpatialMissingLanePrefabCount = 0;
        }

        private void AddQueryToSpatialIndex(
            EntityQuery query,
            SpatialCandidateKind kind)
        {
            NativeArray<Entity> entities =
                query.ToEntityArray(
                    Allocator.Temp);

            try
            {
                foreach (Entity entity in entities)
                {
                    if (!IsEntityUsable(entity))
                    {
                        continue;
                    }

                    if (m_PendingDeletion != null &&
                        m_PendingDeletion.Contains(entity))
                    {
                        continue;
                    }

                    if (kind == SpatialCandidateKind.Building &&
                        !IsRootBuildingEntity(entity))
                    {
                        continue;
                    }

                    if (kind == SpatialCandidateKind.StaticObject &&
                        !IsStaticObjectEntity(entity))
                    {
                        continue;
                    }

                    if (!EntityManager.HasComponent<Transform>(
                            entity))
                    {
                        continue;
                    }

                    Transform transform =
                        EntityManager.GetComponentData<Transform>(
                            entity);

                    float2 position =
                        new float2(
                            transform.m_Position.x,
                            transform.m_Position.z);

                    StaticObjectCategory staticCategory =
                        kind == SpatialCandidateKind.StaticObject
                            ? GetStaticObjectCategory(entity)
                            : StaticObjectCategory.None;

                    AddSpatialCandidate(
                        new(
                            entity,
                            position,
                            kind,
                            staticCategory));
                }
            }
            finally
            {
                entities.Dispose();
            }
        }

        private void AddRoadEdgesToSpatialIndex()
        {
            NativeArray<Entity> roadEdges =
                m_RoadEdgeQuery.ToEntityArray(
                    Allocator.Temp);

            m_SpatialRoadQueryCount =
                roadEdges.Length;

            try
            {
                foreach (Entity entity in roadEdges)
                {
                    if (!IsEntityUsable(entity) ||
                        (m_PendingDeletion != null &&
                         m_PendingDeletion.Contains(entity)))
                    {
                        continue;
                    }

                    if (!IsMainRoadEdgeEntity(entity))
                    {
                        m_SpatialRejectedRoadOwnerCount++;
                        continue;
                    }

                    if (!TryGetNetworkEndpointPositions(
                            entity,
                            out float3 startPosition,
                            out float3 endPosition))
                    {
                        m_SpatialRoadEndpointFailureCount++;
                        continue;
                    }

                    AddSpatialCandidate(
                        new(
                            entity,
                            new(
                                startPosition.x,
                                startPosition.z),
                            new(
                                endPosition.x,
                                endPosition.z),
                            SpatialCandidateKind.Road,
                            StaticObjectCategory.None,
                            true));
                }
            }
            finally
            {
                roadEdges.Dispose();
            }
        }

        private void AddPathAndRailwayEdgesToSpatialIndex()
        {
            NativeArray<Entity> netEdges =
                m_NetEdgeQuery.ToEntityArray(
                    Allocator.Temp);

            m_SpatialPathQueryCount =
                netEdges.Length;

            m_SpatialRailwayQueryCount =
                netEdges.Length;

            try
            {
                foreach (Entity entity in netEdges)
                {
                    if (!IsEntityUsable(entity) ||
                        (m_PendingDeletion != null &&
                         m_PendingDeletion.Contains(entity)))
                    {
                        continue;
                    }

                    bool isPedestrianPath =
                        IsMainPedestrianPathEdgeEntity(entity);

                    bool isRailway =
                        !isPedestrianPath &&
                        IsMainRailwayEdgeEntity(entity);

                    if (!isPedestrianPath &&
                        !isRailway)
                    {
                        m_SpatialRejectedPathCount++;
                        m_SpatialRejectedRailwayCount++;

                        AddRejectedPathPrefabSample(entity);
                        AddRejectedRailwayPrefabSample(entity);

                        continue;
                    }

                    if (!TryGetNetworkEndpointPositions(
                            entity,
                            out float3 startPosition,
                            out float3 endPosition))
                    {
                        if (isPedestrianPath)
                        {
                            m_SpatialPathEndpointFailureCount++;
                        }
                        else
                        {
                            m_SpatialRailwayEndpointFailureCount++;
                        }

                        continue;
                    }

                    AddSpatialCandidate(
                        new(
                            entity,
                            new(
                                startPosition.x,
                                startPosition.z),
                            new(
                                endPosition.x,
                                endPosition.z),
                            isPedestrianPath
                                ? SpatialCandidateKind.PedestrianPath
                                : SpatialCandidateKind.Railway,
                            StaticObjectCategory.None,
                            true));
                }
            }
            finally
            {
                netEdges.Dispose();
            }
        }

        private void AddRejectedPathPrefabSample(
            Entity entity)
        {
            if (m_SpatialRejectedPathPrefabSamples == null ||
                m_SpatialRejectedPathPrefabSamples.Count >= 12 ||
                IsMainRoadEdgeEntity(entity) ||
                !TryGetPrefabBase(
                    entity,
                    out PrefabBase prefabBase))
            {
                return;
            }

            string prefabName =
                prefabBase.name ??
                prefabBase.GetType().Name;

            if (!string.IsNullOrWhiteSpace(prefabName))
            {
                m_SpatialRejectedPathPrefabSamples.Add(
                    prefabName);
            }
        }

        private void AddRejectedRailwayPrefabSample(
            Entity entity)
        {
            if (m_SpatialRejectedRailwayPrefabSamples == null ||
                m_SpatialRejectedRailwayPrefabSamples.Count >= 16 ||
                IsMainRoadEdgeEntity(entity) ||
                IsMainPedestrianPathEdgeEntity(entity) ||
                !TryGetPrefabBase(
                    entity,
                    out PrefabBase prefabBase))
            {
                return;
            }

            string prefabName =
                $"{prefabBase.GetType().Name}:" +
                $"{prefabBase.name}";

            if (!string.IsNullOrWhiteSpace(prefabName))
            {
                m_SpatialRejectedRailwayPrefabSamples.Add(
                    prefabName);
            }
        }

        private bool TryGetNetworkEndpointPositions(
            Entity networkEdge,
            out float3 startPosition,
            out float3 endPosition)
        {
            startPosition = default;
            endPosition = default;

            if (!EntityManager.HasComponent<Edge>(networkEdge))
            {
                return false;
            }

            Edge edge =
                EntityManager.GetComponentData<Edge>(
                    networkEdge);

            if (edge.m_Start == Entity.Null ||
                edge.m_End == Entity.Null ||
                !EntityManager.Exists(edge.m_Start) ||
                !EntityManager.Exists(edge.m_End) ||
                !EntityManager.HasComponent<Game.Net.Node>(
                    edge.m_Start) ||
                !EntityManager.HasComponent<Game.Net.Node>(
                    edge.m_End))
            {
                return false;
            }

            Game.Net.Node startNode =
                EntityManager.GetComponentData<Game.Net.Node>(
                    edge.m_Start);

            Game.Net.Node endNode =
                EntityManager.GetComponentData<Game.Net.Node>(
                    edge.m_End);

            startPosition = startNode.m_Position;
            endPosition = endNode.m_Position;

            return true;
        }

        private void AddSurfaceAreasToSpatialIndex()
        {
            NativeArray<Entity> areaEntities =
                m_SurfaceAreaQuery.ToEntityArray(
                    Allocator.Temp);

            m_SpatialSurfaceAreaQueryCount =
                areaEntities.Length;

            try
            {
                foreach (Entity entity in areaEntities)
                {
                    if (!IsEntityUsable(entity) ||
                        (m_PendingDeletion != null &&
                         m_PendingDeletion.Contains(entity)) ||
                        !EntityManager.HasBuffer<Game.Areas.Node>(
                            entity))
                    {
                        m_SpatialInvalidSurfaceAreaCount++;
                        continue;
                    }

                    DynamicBuffer<Game.Areas.Node> nodes =
                        EntityManager.GetBuffer<Game.Areas.Node>(
                            entity,
                            true);

                    if (nodes.Length < 3)
                    {
                        m_SpatialInvalidSurfaceAreaCount++;
                        continue;
                    }

                    float2 minimum =
                        new float2(
                            nodes[0].m_Position.x,
                            nodes[0].m_Position.z);

                    float2 maximum =
                        minimum;

                    bool validPolygon =
                        math.all(math.isfinite(minimum));

                    for (int index = 1;
                         index < nodes.Length && validPolygon;
                         index++)
                    {
                        float2 position =
                            new float2(
                                nodes[index].m_Position.x,
                                nodes[index].m_Position.z);

                        if (!math.all(math.isfinite(position)))
                        {
                            validPolygon = false;
                            break;
                        }

                        minimum =
                            math.min(
                                minimum,
                                position);

                        maximum =
                            math.max(
                                maximum,
                                position);
                    }

                    if (!validPolygon)
                    {
                        m_SpatialInvalidSurfaceAreaCount++;
                        continue;
                    }

                    AddSpatialCandidate(
                        new(
                            entity,
                            minimum,
                            maximum,
                            SpatialCandidateKind.SurfaceArea,
                            StaticObjectCategory.None,
                            false));
                }
            }
            finally
            {
                areaEntities.Dispose();
            }
        }

        private void AddAssetLanesToSpatialIndex()
        {
            NativeArray<Entity> owners =
                m_SubLaneOwnerQuery.ToEntityArray(
                    Allocator.Temp);

            HashSet<Entity> indexedLanes =
                new();

            try
            {
                foreach (Entity ownerEntity in owners)
                {
                    if (!IsEntityUsable(ownerEntity) ||
                        !EntityManager.HasBuffer<Game.Net.SubLane>(
                            ownerEntity))
                    {
                        continue;
                    }

                    m_SpatialSubLaneOwnerScanCount++;

                    if (!IsSupportedAssetLaneOwner(ownerEntity))
                    {
                        m_SpatialRejectedNetworkLaneOwnerCount++;
                        continue;
                    }

                    DynamicBuffer<Game.Net.SubLane> subLanes =
                        EntityManager.GetBuffer<Game.Net.SubLane>(
                            ownerEntity,
                            true);

                    bool ownerContainsAssetLane = false;

                    for (int index = 0;
                         index < subLanes.Length;
                         index++)
                    {
                        Entity laneEntity =
                            subLanes[index].m_SubLane;

                        m_SpatialSubLaneEntryCount++;

                        if (!IsEntityUsable(laneEntity) ||
                            !indexedLanes.Add(laneEntity))
                        {
                            continue;
                        }

                        if (m_PendingDeletion != null &&
                            m_PendingDeletion.Contains(laneEntity))
                        {
                            continue;
                        }

                        if (!IsAssetLaneEntity(laneEntity))
                        {
                            m_SpatialMissingLanePrefabCount++;
                            continue;
                        }

                        if (!TryGetAssetLanePosition(
                                laneEntity,
                                ownerEntity,
                                out float2 position,
                                out bool usedOwnerPosition))
                        {
                            continue;
                        }

                        AddSpatialCandidate(
                            new(
                                laneEntity,
                                position,
                                SpatialCandidateKind.AssetLane,
                                StaticObjectCategory.None));

                        ownerContainsAssetLane = true;

                        if (usedOwnerPosition)
                        {
                            m_SpatialAssetLaneFallbackPositionCount++;
                        }
                    }

                    if (ownerContainsAssetLane)
                    {
                        m_SpatialAssetLaneOwnerCount++;
                    }
                }
            }
            finally
            {
                owners.Dispose();
            }
        }

        private bool IsSupportedAssetLaneOwner(
            Entity ownerEntity)
        {
            if (ownerEntity == Entity.Null ||
                !EntityManager.Exists(ownerEntity))
            {
                return false;
            }

            if (IsBuildingEntity(ownerEntity) ||
                EntityManager.HasComponent<Game.Tools.EditorContainer>(
                    ownerEntity))
            {
                return true;
            }

            return ResolveOwnerScope(ownerEntity) ==
                   OwnerScope.Building;
        }

        private bool IsAssetLaneEntity(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                !EntityManager.HasComponent<PrefabRef>(entity))
            {
                return false;
            }

            return TryGetPrefabBase(
                       entity,
                       out PrefabBase prefabBase) &&
                   (prefabBase is NetLanePrefab ||
                    prefabBase is NetLaneGeometryPrefab);
        }

        private bool TryGetAssetLanePosition(
            Entity laneEntity,
            Entity bufferOwner,
            out float2 position,
            out bool usedOwnerPosition)
        {
            position = float2.zero;
            usedOwnerPosition = false;

            if (TryGetTransformPosition(
                    laneEntity,
                    out position))
            {
                return true;
            }

            Entity currentOwner = bufferOwner;

            for (int depth = 0;
                 depth < 8;
                 depth++)
            {
                if (TryGetTransformPosition(
                        currentOwner,
                        out position))
                {
                    usedOwnerPosition = true;
                    return true;
                }

                if (currentOwner == Entity.Null ||
                    !EntityManager.Exists(currentOwner) ||
                    !EntityManager.HasComponent<Owner>(
                        currentOwner))
                {
                    break;
                }

                Owner owner =
                    EntityManager.GetComponentData<Owner>(
                        currentOwner);

                currentOwner = owner.m_Owner;
            }

            return false;
        }

        private bool TryGetTransformPosition(
            Entity entity,
            out float2 position)
        {
            position = float2.zero;

            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                !EntityManager.HasComponent<Transform>(entity))
            {
                return false;
            }

            Transform transform =
                EntityManager.GetComponentData<Transform>(entity);

            position =
                new float2(
                    transform.m_Position.x,
                    transform.m_Position.z);

            return true;
        }

        private void AddSpatialCandidate(
            in SpatialCandidate candidate)
        {
            m_SpatialIndex.Add(
                candidate);

            m_SpatialIndexedObjectCount++;

            if (candidate.IsBuilding)
            {
                m_SpatialBuildingCount++;
            }
            else if (candidate.IsRoad)
            {
                m_SpatialRoadCount++;
            }
            else if (candidate.IsPedestrianPath)
            {
                m_SpatialPathCount++;
            }
            else if (candidate.IsRailway)
            {
                m_SpatialRailwayCount++;
            }
            else if (candidate.IsSurfaceArea)
            {
                m_SpatialSurfaceAreaCount++;
            }
            else if (candidate.IsStaticObject)
            {
                IncrementStaticCategoryCount(
                    candidate.StaticCategory,
                    ref m_SpatialGeneralPropCount,
                    ref m_SpatialStreetLightCount,
                    ref m_SpatialQuantityObjectCount,
                    ref m_SpatialBrandingObjectCount,
                    ref m_SpatialActivityLocationCount);
            }
            else if (candidate.IsSpawnLocation)
            {
                m_SpatialSpawnLocationCount++;
            }
            else if (candidate.IsAssetLane)
            {
                m_SpatialAssetLaneCount++;
            }
        }

        private List<SpatialCandidate>
            GetSpatialCandidates(
                float2 center,
                float radius)
        {
            EnsureSpatialIndexReady();

            return m_SpatialIndex.Query(
                center,
                radius);
        }

        private bool IsCandidateInsideSelection(
            in SpatialCandidate candidate,
            float2 selectionCenter,
            float selectionSize)
        {
            float safeSize =
                math.max(
                    5f,
                    selectionSize);

            switch (CurrentSelectionShape)
            {
                case AreaBulldozerSelectionShape.Square:
                    return IsCandidateInsideSquare(
                        candidate,
                        selectionCenter,
                        safeSize,
                        SquareRotationRadians);

                case AreaBulldozerSelectionShape.Triangle:
                    return IsCandidateInsideTriangle(
                        candidate,
                        selectionCenter,
                        safeSize,
                        SquareRotationRadians);

                case AreaBulldozerSelectionShape.Polyline:
                    return IsCandidateInsidePolyline(
                        candidate,
                        CurrentLineHalfWidth);

                default:
                    return IsCandidateInsideCircle(
                        candidate,
                        selectionCenter,
                        safeSize * safeSize);
            }
        }

        private bool IsCandidateInsideCircle(
            in SpatialCandidate candidate,
            float2 circleCenter,
            float radiusSquared)
        {
            if (candidate.IsSurfaceArea)
            {
                return IsSurfaceAreaInsideCircle(
                    candidate.Entity,
                    circleCenter,
                    radiusSquared);
            }

            if (!candidate.IsSegment)
            {
                return math.distancesq(
                    candidate.Position,
                    circleCenter) <= radiusSquared;
            }

            return SelectionGeometry.IsSegmentInsideCircle(
                candidate.Position,
                candidate.EndPosition,
                circleCenter,
                radiusSquared);
        }

        private bool IsCandidateInsideSquare(
            in SpatialCandidate candidate,
            float2 squareCenter,
            float halfSize,
            float rotationRadians)
        {
            if (candidate.IsSurfaceArea)
            {
                return IsSurfaceAreaInsideSquare(
                    candidate.Entity,
                    squareCenter,
                    halfSize,
                    rotationRadians);
            }

            if (!candidate.IsSegment)
            {
                return SelectionGeometry.IsPointInsideSquare(
                    candidate.Position,
                    squareCenter,
                    halfSize,
                    rotationRadians);
            }

            return SelectionGeometry.IsSegmentInsideSquare(
                candidate.Position,
                candidate.EndPosition,
                squareCenter,
                halfSize,
                rotationRadians);
        }

        private bool IsCandidateInsideTriangle(
            in SpatialCandidate candidate,
            float2 triangleCenter,
            float cornerRadius,
            float rotationRadians)
        {
            SelectionGeometry.GetEquilateralTriangleCorners(
                triangleCenter,
                cornerRadius,
                rotationRadians,
                out float2 a,
                out float2 b,
                out float2 c);

            if (candidate.IsSurfaceArea)
            {
                return IsSurfaceAreaInsideTriangle(
                    candidate.Entity,
                    a,
                    b,
                    c);
            }

            if (!candidate.IsSegment)
            {
                return SelectionGeometry.IsPointInsideTriangle(
                    candidate.Position,
                    a,
                    b,
                    c);
            }

            return SelectionGeometry.IsSegmentInsideTriangle(
                candidate.Position,
                candidate.EndPosition,
                a,
                b,
                c);
        }

        private bool IsSurfaceAreaInsideCircle(
            Entity areaEntity,
            float2 circleCenter,
            float radiusSquared)
        {
            if (!TryGetAreaNodes(
                    areaEntity,
                    out DynamicBuffer<Game.Areas.Node> nodes))
            {
                return false;
            }

            for (int index = 0;
                 index < nodes.Length;
                 index++)
            {
                int nextIndex =
                    index + 1 < nodes.Length
                        ? index + 1
                        : 0;

                float2 start =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[index]);

                float2 end =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[nextIndex]);

                if (!math.all(math.isfinite(start)) ||
                    !math.all(math.isfinite(end)))
                {
                    return false;
                }

                if (SelectionGeometry.IsSegmentInsideCircle(
                        start,
                        end,
                        circleCenter,
                        radiusSquared))
                {
                    return true;
                }
            }

            return SelectionGeometry.IsPointInsidePolygon(
                nodes,
                circleCenter);
        }

        private bool IsSurfaceAreaInsideSquare(
            Entity areaEntity,
            float2 squareCenter,
            float halfSize,
            float rotationRadians)
        {
            if (!TryGetAreaNodes(
                    areaEntity,
                    out DynamicBuffer<Game.Areas.Node> nodes))
            {
                return false;
            }

            for (int index = 0;
                 index < nodes.Length;
                 index++)
            {
                int nextIndex =
                    index + 1 < nodes.Length
                        ? index + 1
                        : 0;

                float2 start =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[index]);

                float2 end =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[nextIndex]);

                if (!math.all(math.isfinite(start)) ||
                    !math.all(math.isfinite(end)))
                {
                    return false;
                }

                if (SelectionGeometry.IsPointInsideSquare(
                        start,
                        squareCenter,
                        halfSize,
                        rotationRadians))
                {
                    return true;
                }

                if (SelectionGeometry.IsSegmentInsideSquare(
                        start,
                        end,
                        squareCenter,
                        halfSize,
                        rotationRadians))
                {
                    return true;
                }
            }

            if (SelectionGeometry.IsPointInsidePolygon(
                    nodes,
                    squareCenter))
            {
                return true;
            }

            float2 corner0 =
                squareCenter +
                SelectionGeometry.RotateLocalToWorld(
                    new float2(-halfSize, -halfSize),
                    rotationRadians);

            float2 corner1 =
                squareCenter +
                SelectionGeometry.RotateLocalToWorld(
                    new float2(halfSize, -halfSize),
                    rotationRadians);

            float2 corner2 =
                squareCenter +
                SelectionGeometry.RotateLocalToWorld(
                    new float2(halfSize, halfSize),
                    rotationRadians);

            float2 corner3 =
                squareCenter +
                SelectionGeometry.RotateLocalToWorld(
                    new float2(-halfSize, halfSize),
                    rotationRadians);

            return
                SelectionGeometry.IsPointInsidePolygon(nodes, corner0) ||
                SelectionGeometry.IsPointInsidePolygon(nodes, corner1) ||
                SelectionGeometry.IsPointInsidePolygon(nodes, corner2) ||
                SelectionGeometry.IsPointInsidePolygon(nodes, corner3);
        }

        private bool IsSurfaceAreaInsideTriangle(
            Entity areaEntity,
            float2 a,
            float2 b,
            float2 c)
        {
            if (!TryGetAreaNodes(
                    areaEntity,
                    out DynamicBuffer<Game.Areas.Node> nodes))
            {
                return false;
            }

            for (int index = 0;
                 index < nodes.Length;
                 index++)
            {
                int nextIndex =
                    index + 1 < nodes.Length
                        ? index + 1
                        : 0;

                float2 start =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[index]);

                float2 end =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[nextIndex]);

                if (!math.all(math.isfinite(start)) ||
                    !math.all(math.isfinite(end)))
                {
                    return false;
                }

                if (SelectionGeometry.IsPointInsideTriangle(
                        start,
                        a,
                        b,
                        c) ||
                    SelectionGeometry.IsSegmentInsideTriangle(
                        start,
                        end,
                        a,
                        b,
                        c))
                {
                    return true;
                }
            }

            return
                SelectionGeometry.IsPointInsidePolygon(nodes, a) ||
                SelectionGeometry.IsPointInsidePolygon(nodes, b) ||
                SelectionGeometry.IsPointInsidePolygon(nodes, c);
        }

        private bool TryGetAreaNodes(
            Entity areaEntity,
            out DynamicBuffer<Game.Areas.Node> nodes)
        {
            nodes = default;

            if (areaEntity == Entity.Null ||
                !EntityManager.Exists(areaEntity) ||
                !EntityManager.HasBuffer<Game.Areas.Node>(
                    areaEntity))
            {
                return false;
            }

            nodes =
                EntityManager.GetBuffer<Game.Areas.Node>(
                    areaEntity,
                    true);

            return nodes.Length >= 3;
        }
    }
}
