using Colossal.Mathematics;
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
        private readonly Dictionary<Entity, Entity> m_SpatialAssetVegetation = new();
        private bool m_SpatialAssetVegetationIndexed;
        private readonly HashSet<Entity> m_SpatialLinkedProps = new();
        private bool m_SpatialLinkedPropsIndexed;

        private bool m_SpatialIndexReady;
        private int m_SpatialIndexedObjectCount;
        private int m_SpatialMissingAssetCount;
        private int m_SpatialSourceEntityCount = -1;
        private float m_NextSpatialRefreshCheckTime;

        private int m_SpatialBuildingCount;
        private int m_SpatialCarCount;
        private bool m_SpatialCarsEnabled;
        private bool m_SpatialAssetLanesEnabled;
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
        private int m_SpatialSubLaneOwnerScanCount;
        private int m_SpatialSubLaneEntryCount;
        private int m_SpatialRejectedNetworkLaneOwnerCount;

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
            m_SpatialMissingAssetsEnabled = Mod.Settings?.DeleteMissingAssets ?? false;
            m_SpatialCarsEnabled = Mod.Settings?.DeleteCars ?? false;
            m_SpatialAssetLanesEnabled = NeedsAssetLaneIndex;
            m_SpatialAssetVegetationIndexed = NeedsAssetVegetationIndex;
            m_SpatialLinkedPropsIndexed = NeedsLinkedPropIndex;

            System.Diagnostics.Stopwatch stopwatch =
                System.Diagnostics.Stopwatch.StartNew();

            if (m_SpatialAssetVegetationIndexed || m_SpatialLinkedPropsIndexed)
            {
                IndexAssetSubObjects();
            }

            AddQueryToSpatialIndex(
                m_PlantQuery,
                SpatialCandidateKind.Vegetation);

            AddQueryToSpatialIndex(
                m_BuildingQuery,
                SpatialCandidateKind.Building);

            if (m_SpatialCarsEnabled)
            {
                AddQueryToSpatialIndex(m_CarQuery, SpatialCandidateKind.Car);
            }

            AddRoadEdgesToSpatialIndex();
            AddPathAndRailwayEdgesToSpatialIndex();
            AddSurfaceAreasToSpatialIndex();

            AddQueryToSpatialIndex(
                m_StaticObjectQuery,
                SpatialCandidateKind.StaticObject);

            AddQueryToSpatialIndex(
                m_SpawnLocationQuery,
                SpatialCandidateKind.SpawnLocation);

            if (m_SpatialAssetLanesEnabled)
            {
                AddAssetLanesToSpatialIndex();
            }

            if (m_SpatialMissingAssetsEnabled)
            {
                AddQueryToSpatialIndex(
                    m_MissingAssetObjectQuery,
                    SpatialCandidateKind.MissingAsset);
            }

            if (m_SpatialMissingAssetsEnabled)
            {
                Mod.LogDiagnosticInfo(
                    $"Missing-assets filter: {m_SpatialMissingAssetCount} confirmed missing object instances indexed.");
            }

            stopwatch.Stop();

            m_SpatialIndexReady = true;

            m_SpatialSourceEntityCount =
                ComputeSpatialSourceEntityCount();

            m_NextSpatialRefreshCheckTime =
                UnityEngine.Time.unscaledTime +
                kSpatialRefreshCheckInterval;

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
                $"cars: {m_SpatialCarCount}, " +
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
                $"{m_SpatialAssetLaneOwnerCount}. " +
                $"SubLane diagnostics - owners scanned: " +
                $"{m_SpatialSubLaneOwnerScanCount}, " +
                $"entries scanned: {m_SpatialSubLaneEntryCount}, " +
                $"network owners excluded: " +
                $"{m_SpatialRejectedNetworkLaneOwnerCount}.");

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

        private bool m_SpatialMissingAssetsEnabled;

        private bool NeedsAssetVegetationIndex =>
            Mod.Settings?.DeleteTrees ?? false;

        private bool NeedsLinkedPropIndex =>
            Mod.Settings is Setting setting && setting.DeleteStaticObjects &&
            setting.DeleteGeneralProps && setting.LooseGeneralPropsOnly;

        private bool NeedsAssetLaneIndex =>
            Mod.Settings is Setting setting && setting.DeleteStaticObjects && setting.DeleteMarkerNetworks;

        private void EnsureSpatialIndexReady()
        {
            if (!m_SpatialIndexReady ||
                m_SpatialMissingAssetsEnabled != (Mod.Settings?.DeleteMissingAssets ?? false) ||
                m_SpatialCarsEnabled != (Mod.Settings?.DeleteCars ?? false) ||
                m_SpatialAssetLanesEnabled != NeedsAssetLaneIndex ||
                m_SpatialAssetVegetationIndexed != NeedsAssetVegetationIndex ||
                m_SpatialLinkedPropsIndexed != NeedsLinkedPropIndex)
            {
                RebuildSpatialIndex();
            }
        }

        private int ComputeSpatialSourceEntityCount()
        {
            return
                (m_SpatialMissingAssetsEnabled ? m_MissingAssetObjectQuery.CalculateEntityCount() : 0) +
                m_PlantQuery.CalculateEntityCount() +
                ((m_SpatialAssetVegetationIndexed || m_SpatialLinkedPropsIndexed) ? m_SubObjectOwnerQuery.CalculateEntityCount() : 0) +
                m_BuildingQuery.CalculateEntityCount() +
                (m_SpatialCarsEnabled ? m_CarQuery.CalculateEntityCount() : 0) +
                m_RoadEdgeQuery.CalculateEntityCount() +
                m_NetEdgeQuery.CalculateEntityCount() +
                m_SurfaceAreaQuery.CalculateEntityCount() +
                m_StaticObjectQuery.CalculateEntityCount() +
                m_SpawnLocationQuery.CalculateEntityCount() +
                (m_SpatialAssetLanesEnabled ? m_SubLaneOwnerQuery.CalculateEntityCount() : 0);
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

            if (currentCount == m_SpatialSourceEntityCount &&
                !m_SpatialMissingAssetsEnabled)
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
            m_SpatialAssetVegetation.Clear();
            m_SpatialLinkedProps.Clear();
            m_SpatialMissingAssetCount = 0;

            ResetSpatialIndexCounters();
            m_SpatialIndexReady = false;
        }

        private void ResetSpatialIndexCounters()
        {
            m_SpatialIndexedObjectCount = 0;

            m_SpatialBuildingCount = 0;
            m_SpatialCarCount = 0;
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
            m_SpatialSubLaneOwnerScanCount = 0;
            m_SpatialSubLaneEntryCount = 0;
            m_SpatialRejectedNetworkLaneOwnerCount = 0;
        }

        private void IndexAssetSubObjects()
        {
            using NativeArray<Entity> parents = m_SubObjectOwnerQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity parent in parents)
            {
                DynamicBuffer<Game.Objects.SubObject> children =
                    EntityManager.GetBuffer<Game.Objects.SubObject>(parent, true);

                for (int index = 0; index < children.Length; index++)
                {
                    Entity child = children[index].m_SubObject;
                    if (child == Entity.Null || !EntityManager.Exists(child) ||
                        EntityManager.HasComponent<Owner>(child))
                    {
                        continue;
                    }

                    if (m_SpatialAssetVegetationIndexed && EntityManager.HasComponent<Plant>(child))
                    {
                        m_SpatialAssetVegetation[child] =
                            m_SpatialAssetVegetation.TryGetValue(child, out Entity indexedParent) &&
                            indexedParent != parent
                                ? Entity.Null
                                : parent;
                    }

                    if (m_SpatialLinkedPropsIndexed)
                    {
                        m_SpatialLinkedProps.Add(child);
                    }
                }
            }
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
                    if (!IsEntityUsable(
                            entity,
                            allowOverridden:
                                kind == SpatialCandidateKind.Vegetation ||
                                kind == SpatialCandidateKind.MissingAsset))
                    {
                        continue;
                    }

                    if (m_PendingDeletion != null &&
                        m_PendingDeletion.Contains(entity))
                    {
                        continue;
                    }

                    bool missingAsset = m_SpatialMissingAssetsEnabled && IsMissingAssetObject(entity);
                    if (kind == SpatialCandidateKind.MissingAsset ? !missingAsset : missingAsset)
                    {
                        continue;
                    }

                    if (kind == SpatialCandidateKind.Building &&
                        !IsRootBuildingEntity(entity))
                    {
                        continue;
                    }

                    if (kind == SpatialCandidateKind.StaticObject &&
                        !IsActivityLocationTarget(entity) &&
                        (TryGetPointMarkerPrefab(entity, out _) || !IsStaticObjectEntity(entity)))
                    {
                        continue;
                    }

                    if (kind == SpatialCandidateKind.SpawnLocation && !IsSpawnPointMarker(entity))
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
                    if (!IsEntityUsable(entity, allowOverridden: m_SpatialAssetLanesEnabled) ||
                        (m_PendingDeletion != null &&
                         m_PendingDeletion.Contains(entity)))
                    {
                        continue;
                    }

                    if (IsAssetMarkerNetwork(entity))
                    {
                        if (m_SpatialAssetLanesEnabled)
                        {
                            AddAssetLaneCurveToSpatialIndex(entity);
                        }

                        continue;
                    }

                    if (EntityManager.HasComponent<Overridden>(entity))
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
                    if (!IsEntityUsable(ownerEntity, allowOverridden: true) ||
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

                    if (IsStandaloneAssetLane(ownerEntity))
                    {
                        if (indexedLanes.Add(ownerEntity) && AddAssetLaneCurveToSpatialIndex(ownerEntity))
                        {
                            m_SpatialAssetLaneOwnerCount++;
                        }
                        continue;
                    }

                    DynamicBuffer<Game.Net.SubLane> subLanes =
                        EntityManager.GetBuffer<Game.Net.SubLane>(ownerEntity, true);

                    bool ownerContainsAssetLane = false;
                    for (int index = 0; index < subLanes.Length; index++)
                    {
                        Entity lane = subLanes[index].m_SubLane;
                        m_SpatialSubLaneEntryCount++;
                        if (!IsEntityUsable(lane, allowOverridden: true) ||
                            (m_PendingDeletion != null && m_PendingDeletion.Contains(lane)) ||
                            !indexedLanes.Add(lane))
                        {
                            continue;
                        }

                        if (IsAssetLaneMarker(lane) &&
                            AddAssetLaneCurveToSpatialIndex(lane))
                        {
                            ownerContainsAssetLane = true;
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

            return IsBuildingEntity(ownerEntity) || IsStandaloneAssetLane(ownerEntity) ||
                   TryGetOwningRootBuilding(ownerEntity, out _);
        }

        private bool IsAssetMarkerNetwork(Entity entity)
        {
            if (!EntityManager.HasComponent<Edge>(entity) ||
                !EntityManager.HasComponent<PrefabRef>(entity) ||
                !EntityManager.HasComponent<Curve>(entity))
            {
                return false;
            }

            Entity prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
            if (!EntityManager.Exists(prefab) || !EntityManager.HasComponent<NetData>(prefab))
            {
                return false;
            }

            NetData net = EntityManager.GetComponentData<NetData>(prefab);
            Game.Net.Layer layers = net.m_RequiredLayers;
            if (EntityManager.HasComponent<Game.Net.Marker>(entity) ||
                EntityManager.HasComponent<MarkerNetData>(prefab))
            {
                layers |= net.m_ConnectLayers | net.m_LocalConnectLayers;
            }

            return (layers & (Game.Net.Layer.MarkerPathway | Game.Net.Layer.MarkerTaxiway)) != 0 &&
                   (net.m_RequiredLayers & (Game.Net.Layer.Fence | Game.Net.Layer.NetFence)) == 0 &&
                   TryGetOwningRootBuilding(entity, out _);
        }

        private bool IsStandaloneAssetLane(Entity entity)
        {
            if (!EntityManager.HasComponent<Game.Tools.EditorContainer>(entity) ||
                !EntityManager.HasComponent<Edge>(entity) ||
                EntityManager.HasComponent<Owner>(entity))
            {
                return false;
            }

            Entity prefab = EntityManager.GetComponentData<Game.Tools.EditorContainer>(entity).m_Prefab;
            if (!IsMarkerLanePrefab(prefab) || !EntityManager.HasBuffer<Game.Net.SubLane>(entity))
            {
                return false;
            }

            DynamicBuffer<Game.Net.SubLane> subLanes = EntityManager.GetBuffer<Game.Net.SubLane>(entity, true);
            bool hasMarkerGeometry = false;
            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity child = subLanes[i].m_SubLane;
                if (!EntityManager.Exists(child) || !EntityManager.HasComponent<Game.Net.LaneGeometry>(child))
                {
                    continue;
                }

                if (!IsAssetLaneMarker(child))
                {
                    return false;
                }

                hasMarkerGeometry = true;
            }

            return hasMarkerGeometry;
        }

        private bool IsAssetLaneMarker(Entity lane)
        {
            if (!EntityManager.HasComponent<Game.Net.Lane>(lane) ||
                !EntityManager.HasComponent<PrefabRef>(lane) ||
                !EntityManager.HasComponent<Curve>(lane))
            {
                return false;
            }

            Entity prefab = EntityManager.GetComponentData<PrefabRef>(lane).m_Prefab;
            if (!EntityManager.Exists(prefab) || IsFenceLanePrefab(prefab))
            {
                return false;
            }

            return EntityManager.HasComponent<Game.Net.CarLane>(lane) ||
                   EntityManager.HasComponent<Game.Net.PedestrianLane>(lane) ||
                   EntityManager.HasComponent<Game.Net.ParkingLane>(lane) ||
                   IsMarkerLanePrefab(prefab);
        }

        private bool IsMarkerLanePrefab(Entity prefab)
        {
            if (!EntityManager.Exists(prefab) || !EntityManager.HasComponent<NetLaneData>(prefab))
            {
                return false;
            }

            if (IsFenceLanePrefab(prefab))
            {
                return false;
            }

            var flags = EntityManager.GetComponentData<NetLaneData>(prefab).m_Flags;
            if ((flags & (Game.Prefabs.LaneFlags.Road |
                          Game.Prefabs.LaneFlags.Pedestrian |
                          Game.Prefabs.LaneFlags.Parking)) != 0)
            {
                return true;
            }

            if (!EntityManager.HasComponent<NetLaneGeometryData>(prefab))
            {
                return false;
            }

            NetLaneGeometryData geometry = EntityManager.GetComponentData<NetLaneGeometryData>(prefab);
            return (geometry.m_GameLayers & MeshLayer.Default) == 0 &&
                   ((geometry.m_GameLayers | geometry.m_EditorLayers) & MeshLayer.Marker) != 0;
        }

        private bool IsFenceLanePrefab(Entity prefab) =>
            EntityManager.HasComponent<UtilityLaneData>(prefab) &&
            (EntityManager.GetComponentData<UtilityLaneData>(prefab).m_UtilityTypes & UtilityTypes.Fence) != 0;

        private bool AddAssetLaneCurveToSpatialIndex(Entity entity)
        {
            if (!EntityManager.HasComponent<Curve>(entity))
            {
                return false;
            }

            Curve curve = EntityManager.GetComponentData<Curve>(entity);
            if (!math.isfinite(curve.m_Length) ||
                !math.all(math.isfinite(curve.m_Bezier.a)) || !math.all(math.isfinite(curve.m_Bezier.b)) ||
                !math.all(math.isfinite(curve.m_Bezier.c)) || !math.all(math.isfinite(curve.m_Bezier.d)))
            {
                return false;
            }

            int segments = math.clamp((int)math.ceil(curve.m_Length / 4f), 4, 64);
            float2 start = curve.m_Bezier.a.xz;
            for (int i = 1; i <= segments; i++)
            {
                float2 end = MathUtils.Position(curve.m_Bezier, (float)i / segments).xz;
                m_SpatialIndex.Add(new SpatialCandidate(entity, start, end,
                    SpatialCandidateKind.AssetLane, StaticObjectCategory.None, true));
                start = end;
            }

            m_SpatialIndexedObjectCount++;
            m_SpatialAssetLaneCount++;
            return true;
        }

        private bool TryGetVehicleSelectionPosition(Entity entity, out float3 position)
        {
            position = default;
            if (entity == Entity.Null || !EntityManager.Exists(entity))
            {
                return false;
            }

            if (EntityManager.HasComponent<Game.Rendering.InterpolatedTransform>(entity))
            {
                position = EntityManager.GetComponentData<Game.Rendering.InterpolatedTransform>(entity).m_Position;
                if (math.all(math.isfinite(position)))
                {
                    return true;
                }
            }

            if (!EntityManager.HasComponent<Transform>(entity))
            {
                return false;
            }

            position = EntityManager.GetComponentData<Transform>(entity).m_Position;
            return math.all(math.isfinite(position));
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

            if (candidate.IsMissingAsset)
            {
                m_SpatialMissingAssetCount++;
            }

            m_SpatialIndexedObjectCount++;

            if (candidate.IsBuilding)
            {
                m_SpatialBuildingCount++;
            }
            else if (candidate.IsCar)
            {
                m_SpatialCarCount++;
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
        }

        private List<SpatialCandidate>
            GetSpatialCandidates(
                float2 center,
                float radius)
        {
            EnsureSpatialIndexReady();
            PrepareHeightFilter();

            List<SpatialCandidate> candidates = m_SpatialIndex.Query(center, radius);
            if (Mod.Settings?.DeleteCars ?? false)
            {
                candidates.RemoveAll(candidate => candidate.IsCar);
                NativeArray<Entity> cars = m_CarQuery.ToEntityArray(Allocator.Temp);
                try
                {
                    float radiusSquared = radius * radius;
                    foreach (Entity car in cars)
                    {
                        if (!IsEntityUsable(car) ||
                            (m_PendingDeletion != null && m_PendingDeletion.Contains(car)) ||
                            (m_SpatialMissingAssetsEnabled && IsMissingAssetObject(car)) ||
                            !TryGetVehicleSelectionPosition(car, out float3 visiblePosition) ||
                            math.distancesq(new float2(visiblePosition.x, visiblePosition.z), center) > radiusSquared)
                        {
                            continue;
                        }

                        candidates.Add(new SpatialCandidate(car,
                            new float2(visiblePosition.x, visiblePosition.z), SpatialCandidateKind.Car, StaticObjectCategory.None));
                    }
                }
                finally
                {
                    cars.Dispose();
                }
            }

            return candidates;
        }

        private bool IsCandidateInsideSelection(
            in SpatialCandidate indexedCandidate,
            float2 selectionCenter,
            float selectionSize)
        {
            if (!MatchesHeightFilter(indexedCandidate.Entity))
            {
                return false;
            }

            SpatialCandidate candidate = indexedCandidate;

            if (candidate.IsCar)
            {
                if (!TryGetVehicleSelectionPosition(candidate.Entity, out float3 visiblePosition))
                {
                    return false;
                }
                candidate = new SpatialCandidate(candidate.Entity,
                    new float2(visiblePosition.x, visiblePosition.z),
                    candidate.Kind, candidate.StaticCategory);
            }
            else if (candidate.IsMissingAsset)
            {
                if (!TryGetTransformPosition(candidate.Entity, out float2 currentPosition) ||
                    !math.all(math.isfinite(currentPosition)))
                {
                    return false;
                }
                candidate = new SpatialCandidate(candidate.Entity, currentPosition,
                    candidate.Kind, candidate.StaticCategory);
            }

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
