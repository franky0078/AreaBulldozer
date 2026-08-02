using Game.Common;
using Game.Net;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // ------------------------------------------------------------
        // Auswahl und Löschen
        // ------------------------------------------------------------

        private void DeleteSelectedObjects()
        {
            if (!HasValidPosition)
            {
                return;
            }

            if (Mod.Settings == null)
            {
                Mod.Log.Warn(
                    "Area Bulldozer settings are not available.");

                return;
            }

            bool deleteTrees =
                Mod.Settings.DeleteTrees;

            bool deleteBuildings =
                Mod.Settings.DeleteBuildings;

            bool deleteRoads =
                Mod.Settings.DeleteRoads;

            bool deletePaths =
                Mod.Settings.DeletePaths;

            bool deleteRailways =
                Mod.Settings.DeleteRailways;

            bool deleteSurfaces =
                Mod.Settings.DeleteSurfaces;

            bool deleteStaticObjects =
                Mod.Settings.DeleteStaticObjects;

            bool deleteSpawnLocations =
                Mod.Settings.DeleteSpawnLocations;

            bool deleteMarkerNetworks =
                Mod.Settings.DeleteMarkerNetworks;

            if (!deleteTrees &&
                !deleteBuildings &&
                !deleteRoads &&
                !deletePaths &&
                !deleteRailways &&
                !deleteSurfaces &&
                !deleteStaticObjects)
            {
                Mod.Log.Info(
                    "Area Bulldozer: all object filters are disabled.");

                return;
            }

            m_PendingDeletion ??=
                new();

            if (m_ToolOutputBarrier == null)
            {
                Mod.Log.Error(
                    "ToolOutputBarrier is not available. " +
                    "Deletion was cancelled.");

                return;
            }

            HashSet<Entity> entitiesToDelete =
                new();

            HashSet<Entity> assetLaneEntitiesToDelete =
                new();

            HashSet<Entity> networkEdgesToDelete =
                new();

            int vegetationCount = 0;
            int buildingCount = 0;
            int roadCount = 0;
            int pathCount = 0;
            int railwayCount = 0;
            int surfaceCount = 0;
            int staticObjectCount = 0;
            int spawnLocationCount = 0;
            int markerNetworkCount = 0;

            int generalPropCount = 0;
            int streetLightCount = 0;
            int quantityObjectCount = 0;
            int brandingObjectCount = 0;
            int activityLocationCount = 0;

            int protectedVegetationCount = 0;
            int protectedSurfaceCount = 0;
            int protectedStaticObjectCount = 0;
            int protectedSpawnLocationCount = 0;
            int protectedMarkerNetworkCount = 0;

            float radius =
                CurrentRadius;

            string selectionDescription =
                UseSquareBrush
                    ? $"square with {radius:0} m half-side at " +
                      $"{SquareRotationDegrees:0} degrees"
                    : $"circle with {radius:0} m radius";

            float2 selectionCenter =
                new float2(
                    CurrentPosition.x,
                    CurrentPosition.z);

            List<SpatialCandidate> candidates =
                GetSpatialCandidates(
                    selectionCenter,
                    CurrentSpatialQueryRadius);

            HashSet<Entity> selectedBuildings =
                new();

            if (deleteBuildings)
            {
                foreach (
                    SpatialCandidate candidate
                    in candidates)
                {
                    if (!candidate.IsBuilding ||
                        !IsCandidateInsideSelection(
                            candidate,
                            selectionCenter,
                            radius) ||
                        !IsEntityUsable(candidate.Entity) ||
                        m_PendingDeletion.Contains(
                            candidate.Entity))
                    {
                        continue;
                    }

                    selectedBuildings.Add(
                        candidate.Entity);
                }
            }

            foreach (
                SpatialCandidate candidate
                in candidates)
            {
                if (candidate.IsBuilding)
                {
                    if (!deleteBuildings)
                    {
                        continue;
                    }
                }
                else if (candidate.IsRoad)
                {
                    if (!deleteRoads)
                    {
                        continue;
                    }
                }
                else if (candidate.IsPedestrianPath)
                {
                    if (!deletePaths)
                    {
                        continue;
                    }
                }
                else if (candidate.IsRailway)
                {
                    if (!deleteRailways)
                    {
                        continue;
                    }
                }
                else if (candidate.IsSurfaceArea)
                {
                    if (!deleteSurfaces)
                    {
                        continue;
                    }
                }
                else if (candidate.IsStaticObject)
                {
                    if (!deleteStaticObjects ||
                        !IsStaticCategoryEnabled(
                            candidate.StaticCategory))
                    {
                        continue;
                    }
                }
                else if (candidate.IsSpawnLocation)
                {
                    if (!deleteStaticObjects ||
                        !deleteSpawnLocations)
                    {
                        continue;
                    }
                }
                else if (candidate.IsAssetLane)
                {
                    if (!deleteStaticObjects ||
                        !deleteMarkerNetworks)
                    {
                        continue;
                    }
                }
                else if (!deleteTrees)
                {
                    continue;
                }

                if (!IsCandidateInsideSelection(
                        candidate,
                        selectionCenter,
                        radius))
                {
                    continue;
                }

                Entity entity =
                    candidate.Entity;

                if (!IsEntityUsable(entity))
                {
                    continue;
                }

                if (m_PendingDeletion.Contains(
                        entity))
                {
                    continue;
                }

                if (!candidate.IsBuilding &&
                    !candidate.IsRoad &&
                    !candidate.IsPedestrianPath &&
                    !candidate.IsRailway &&
                    TryGetOwningRootBuilding(
                        entity,
                        out Entity owningBuilding) &&
                    selectedBuildings.Contains(
                        owningBuilding))
                {
                    continue;
                }

                if (!candidate.IsBuilding &&
                    !candidate.IsRoad &&
                    !candidate.IsPedestrianPath &&
                    !candidate.IsRailway &&
                    IsCandidateOwnedObjectProtected(candidate))
                {
                    if (candidate.IsSurfaceArea)
                    {
                        protectedSurfaceCount++;
                    }
                    else if (candidate.IsStaticObject)
                    {
                        protectedStaticObjectCount++;
                    }
                    else if (candidate.IsSpawnLocation)
                    {
                        protectedSpawnLocationCount++;
                    }
                    else if (candidate.IsAssetLane)
                    {
                        protectedMarkerNetworkCount++;
                    }
                    else
                    {
                        protectedVegetationCount++;
                    }

                    continue;
                }

                if (!entitiesToDelete.Add(entity))
                {
                    continue;
                }

                if (candidate.IsBuilding)
                {
                    buildingCount++;
                }
                else if (candidate.IsRoad)
                {
                    roadCount++;
                    networkEdgesToDelete.Add(entity);
                }
                else if (candidate.IsPedestrianPath)
                {
                    pathCount++;
                    networkEdgesToDelete.Add(entity);
                }
                else if (candidate.IsRailway)
                {
                    railwayCount++;
                    networkEdgesToDelete.Add(entity);
                }
                else if (candidate.IsSurfaceArea)
                {
                    surfaceCount++;
                }
                else if (candidate.IsStaticObject)
                {
                    staticObjectCount++;

                    IncrementStaticCategoryCount(
                        candidate.StaticCategory,
                        ref generalPropCount,
                        ref streetLightCount,
                        ref quantityObjectCount,
                        ref brandingObjectCount,
                        ref activityLocationCount);
                }
                else if (candidate.IsSpawnLocation)
                {
                    spawnLocationCount++;
                }
                else if (candidate.IsAssetLane)
                {
                    markerNetworkCount++;
                    assetLaneEntitiesToDelete.Add(entity);
                }
                else
                {
                    vegetationCount++;
                }
            }

            int protectedTotal =
                protectedVegetationCount +
                protectedSurfaceCount +
                protectedStaticObjectCount +
                protectedSpawnLocationCount +
                protectedMarkerNetworkCount;

            if (entitiesToDelete.Count == 0)
            {
                CancelLargeSelectionConfirmation();

                Mod.Log.Info(
                    $"Area Bulldozer: no removable objects " +
                    $"inside {selectionDescription}. " +
                    $"{protectedTotal} protected " +
                    $"sub-objects ignored.");

                return;
            }

            bool confirmationEnabled =
                Mod.Settings.ConfirmLargeSelection;

            bool confirmationAlreadyPending =
                m_LargeSelectionConfirmationPending;

            bool reachesConfiguredThreshold =
                entitiesToDelete.Count >=
                    CurrentLargeSelectionThreshold;

            bool requiresConfirmation =
                confirmationEnabled &&
                (confirmationAlreadyPending ||
                 reachesConfiguredThreshold);

            if (requiresConfirmation)
            {
                bool confirmed =
                    confirmationAlreadyPending &&
                    IsMatchingLargeSelectionConfirmation(
                        entitiesToDelete,
                        vegetationCount,
                        buildingCount,
                        roadCount,
                        pathCount,
                        railwayCount,
                        surfaceCount,
                        staticObjectCount,
                        spawnLocationCount,
                        markerNetworkCount);

                if (!confirmed)
                {
                    BeginLargeSelectionConfirmation(
                        entitiesToDelete,
                        vegetationCount,
                        buildingCount,
                        roadCount,
                        pathCount,
                        railwayCount,
                        surfaceCount,
                        staticObjectCount,
                        spawnLocationCount,
                        markerNetworkCount,
                        protectedTotal);

                    return;
                }

                Mod.Log.Info(
                    $"Area Bulldozer: large selection " +
                    $"confirmed. " +
                    $"{entitiesToDelete.Count} objects " +
                    $"will be deleted.");

                CancelLargeSelectionConfirmation();
            }
            else
            {
                CancelLargeSelectionConfirmation();
            }

            foreach (
                Entity entity
                in entitiesToDelete)
            {
                m_PendingDeletion.Add(entity);

                RemoveHighlightBeforeDeletion(
                    entity);
            }

            ClearSelectionPreview(
                resetPreviewState: false);

            m_LastPreviewPosition =
                CurrentPosition;

            m_LastPreviewRadius =
                radius;

            m_NextPreviewUpdateTime =
                UnityEngine.Time.unscaledTime +
                kPreviewUpdateInterval;

            EntityCommandBuffer commandBuffer =
                m_ToolOutputBarrier.CreateCommandBuffer();

            HashSet<Entity> queuedDeletedEntities =
                new();

            HashSet<Entity> queuedUpdatedEntities =
                new();

            int queuedCount = 0;

            HashSet<Entity> deletedRoadNodes =
                new();

            HashSet<Entity> updatedRoadNodes =
                new();

            HashSet<Entity> updatedConnectedRoadEdges =
                new();

            foreach (
                Entity entity
                in entitiesToDelete)
            {
                if (entity == Entity.Null ||
                    !EntityManager.Exists(entity))
                {
                    m_PendingDeletion.Remove(entity);
                    continue;
                }

                if (EntityManager.HasComponent<Deleted>(
                        entity))
                {
                    m_PendingDeletion.Remove(entity);
                    continue;
                }

                if (networkEdgesToDelete.Contains(
                        entity))
                {
                    QueueRoadDependencyUpdates(
                        entity,
                        networkEdgesToDelete,
                        queuedDeletedEntities,
                        queuedUpdatedEntities,
                        deletedRoadNodes,
                        updatedRoadNodes,
                        updatedConnectedRoadEdges,
                        commandBuffer);
                }

                if (assetLaneEntitiesToDelete.Contains(
                        entity))
                {
                    QueueAssetLaneOwnerUpdate(
                        entity,
                        queuedDeletedEntities,
                        queuedUpdatedEntities,
                        commandBuffer);
                }

                QueueDeletedComponent(
                    entity,
                    queuedDeletedEntities,
                    commandBuffer);

                queuedCount++;
            }

            Mod.Log.Info(
                $"Area Bulldozer: marked {queuedCount} " +
                $"objects for deletion inside " +
                $"{selectionDescription}. " +
                $"Vegetation: {vegetationCount}, " +
                $"buildings: {buildingCount}, " +
                $"roads: {roadCount}, " +
                $"pedestrian paths: {pathCount}, " +
                $"railway tracks: {railwayCount}, " +
                $"surfaces and spaces: {surfaceCount}, " +
                $"static objects: {staticObjectCount} " +
                $"(general props: {generalPropCount}, " +
                $"street lights: {streetLightCount}, " +
                $"quantity objects: {quantityObjectCount}, " +
                $"branding: {brandingObjectCount}, " +
                $"activity locations: {activityLocationCount}), " +
                $"spawn locations: {spawnLocationCount}, " +
                $"asset lanes: {markerNetworkCount}, " +
                $"network endpoint nodes deleted: " +
                $"{deletedRoadNodes.Count}, " +
                $"network nodes updated: " +
                $"{updatedRoadNodes.Count}, " +
                $"connected network edges updated: " +
                $"{updatedConnectedRoadEdges.Count}, " +
                $"protected sub-objects ignored: " +
                $"{protectedTotal}.");
        }

        private void QueueRoadDependencyUpdates(
            Entity roadEntity,
            HashSet<Entity> selectedRoadEdges,
            HashSet<Entity> queuedDeletedEntities,
            HashSet<Entity> queuedUpdatedEntities,
            HashSet<Entity> deletedRoadNodes,
            HashSet<Entity> updatedRoadNodes,
            HashSet<Entity> updatedConnectedRoadEdges,
            EntityCommandBuffer commandBuffer)
        {
            if (roadEntity == Entity.Null ||
                !EntityManager.Exists(roadEntity) ||
                !EntityManager.HasComponent<Edge>(roadEntity))
            {
                return;
            }

            Edge edge =
                EntityManager.GetComponentData<Edge>(
                    roadEntity);

            QueueRoadAggregateUpdate(
                roadEntity,
                queuedDeletedEntities,
                queuedUpdatedEntities,
                commandBuffer);

            QueueRoadEndpointDependencies(
                roadEntity,
                edge.m_Start,
                selectedRoadEdges,
                queuedDeletedEntities,
                queuedUpdatedEntities,
                deletedRoadNodes,
                updatedRoadNodes,
                updatedConnectedRoadEdges,
                commandBuffer);

            QueueRoadEndpointDependencies(
                roadEntity,
                edge.m_End,
                selectedRoadEdges,
                queuedDeletedEntities,
                queuedUpdatedEntities,
                deletedRoadNodes,
                updatedRoadNodes,
                updatedConnectedRoadEdges,
                commandBuffer);
        }
        private void QueueRoadAggregateUpdate(
            Entity roadEntity,
            HashSet<Entity> queuedDeletedEntities,
            HashSet<Entity> queuedUpdatedEntities,
            EntityCommandBuffer commandBuffer)
        {
            if (!EntityManager.HasComponent<Owner>(roadEntity))
            {
                return;
            }

            Owner owner =
                EntityManager.GetComponentData<Owner>(roadEntity);

            if (owner.m_Owner == Entity.Null ||
                !EntityManager.Exists(owner.m_Owner) ||
                !EntityManager.HasComponent<Aggregate>(
                    owner.m_Owner))
            {
                return;
            }

            QueueUpdatedComponent(
                owner.m_Owner,
                queuedUpdatedEntities,
                queuedDeletedEntities,
                commandBuffer);
        }

        private void QueueRoadEndpointDependencies(
            Entity roadEntity,
            Entity nodeEntity,
            HashSet<Entity> selectedRoadEdges,
            HashSet<Entity> queuedDeletedEntities,
            HashSet<Entity> queuedUpdatedEntities,
            HashSet<Entity> deletedRoadNodes,
            HashSet<Entity> updatedRoadNodes,
            HashSet<Entity> updatedConnectedRoadEdges,
            EntityCommandBuffer commandBuffer)
        {
            if (nodeEntity == Entity.Null ||
                !EntityManager.Exists(nodeEntity))
            {
                return;
            }

            int remainingConnectedEdges = 0;

            if (EntityManager.HasBuffer<ConnectedEdge>(
                    nodeEntity))
            {
                DynamicBuffer<ConnectedEdge> connectedEdges =
                    EntityManager.GetBuffer<ConnectedEdge>(
                        nodeEntity,
                        true);

                foreach (ConnectedEdge connectedEdge
                         in connectedEdges)
                {
                    Entity connectedEntity =
                        connectedEdge.m_Edge;

                    if (connectedEntity == Entity.Null ||
                        connectedEntity == roadEntity ||
                        selectedRoadEdges.Contains(
                            connectedEntity) ||
                        !EntityManager.Exists(
                            connectedEntity) ||
                        EntityManager.HasComponent<Deleted>(
                            connectedEntity))
                    {
                        continue;
                    }

                    remainingConnectedEdges++;

                    int updatedCountBefore =
                        queuedUpdatedEntities.Count;

                    QueueUpdatedComponent(
                        connectedEntity,
                        queuedUpdatedEntities,
                        queuedDeletedEntities,
                        commandBuffer);

                    if (queuedUpdatedEntities.Count >
                        updatedCountBefore)
                    {
                        updatedConnectedRoadEdges.Add(
                            connectedEntity);
                    }

                    if (!EntityManager.HasComponent<Edge>(
                            connectedEntity))
                    {
                        continue;
                    }

                    Edge neighboringEdge =
                        EntityManager.GetComponentData<Edge>(
                            connectedEntity);

                    QueueRoadNodeUpdate(
                        neighboringEdge.m_Start,
                        queuedUpdatedEntities,
                        queuedDeletedEntities,
                        updatedRoadNodes,
                        commandBuffer);

                    QueueRoadNodeUpdate(
                        neighboringEdge.m_End,
                        queuedUpdatedEntities,
                        queuedDeletedEntities,
                        updatedRoadNodes,
                        commandBuffer);
                }
            }

            if (remainingConnectedEdges == 0)
            {
                int deletedCountBefore =
                    queuedDeletedEntities.Count;

                QueueDeletedComponent(
                    nodeEntity,
                    queuedDeletedEntities,
                    commandBuffer);

                if (queuedDeletedEntities.Count >
                    deletedCountBefore)
                {
                    deletedRoadNodes.Add(
                        nodeEntity);
                }

                return;
            }

            QueueRoadNodeUpdate(
                nodeEntity,
                queuedUpdatedEntities,
                queuedDeletedEntities,
                updatedRoadNodes,
                commandBuffer);
        }

        private void QueueRoadNodeUpdate(
            Entity nodeEntity,
            HashSet<Entity> queuedUpdatedEntities,
            HashSet<Entity> queuedDeletedEntities,
            HashSet<Entity> updatedRoadNodes,
            EntityCommandBuffer commandBuffer)
        {
            int updatedCountBefore =
                queuedUpdatedEntities.Count;

            QueueUpdatedComponent(
                nodeEntity,
                queuedUpdatedEntities,
                queuedDeletedEntities,
                commandBuffer);

            if (queuedUpdatedEntities.Count >
                updatedCountBefore)
            {
                updatedRoadNodes.Add(
                    nodeEntity);
            }
        }

        private void QueueAssetLaneOwnerUpdate(
            Entity laneEntity,
            HashSet<Entity> queuedDeletedEntities,
            HashSet<Entity> queuedUpdatedEntities,
            EntityCommandBuffer commandBuffer)
        {
            if (laneEntity == Entity.Null ||
                !EntityManager.Exists(laneEntity) ||
                !EntityManager.HasComponent<Game.Common.Owner>(
                    laneEntity))
            {
                return;
            }

            Game.Common.Owner owner =
                EntityManager.GetComponentData<Game.Common.Owner>(
                    laneEntity);

            if (owner.m_Owner == Entity.Null ||
                !EntityManager.Exists(owner.m_Owner))
            {
                return;
            }

            QueueUpdatedComponent(
                owner.m_Owner,
                queuedUpdatedEntities,
                queuedDeletedEntities,
                commandBuffer);
        }

        private void QueueDeletedComponent(
            Entity entity,
            HashSet<Entity> queuedDeletedEntities,
            EntityCommandBuffer commandBuffer)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                EntityManager.HasComponent<Deleted>(entity) ||
                !queuedDeletedEntities.Add(entity))
            {
                return;
            }

            commandBuffer.AddComponent<Deleted>(
                entity);
        }

        private void QueueUpdatedComponent(
            Entity entity,
            HashSet<Entity> queuedUpdatedEntities,
            HashSet<Entity> queuedDeletedEntities,
            EntityCommandBuffer commandBuffer)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                EntityManager.HasComponent<Deleted>(entity) ||
                queuedDeletedEntities.Contains(entity) ||
                EntityManager.HasComponent<Updated>(entity) ||
                !queuedUpdatedEntities.Add(entity))
            {
                return;
            }

            commandBuffer.AddComponent<Updated>(
                entity);
        }
    }
}
