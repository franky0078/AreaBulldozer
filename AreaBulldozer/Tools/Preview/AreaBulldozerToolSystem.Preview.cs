using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // ------------------------------------------------------------
        // Vorschau
        // ------------------------------------------------------------

        private void UpdateSelectionPreviewIfNeeded()
        {
            if (!HasValidPosition ||
                Mod.Settings == null)
            {
                ClearSelectionPreview();
                return;
            }

            bool useSquareBrush =
                Mod.Settings.UseSquareBrush;

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

            bool deleteGeneralProps =
                Mod.Settings.DeleteGeneralProps;

            bool deleteStreetLights =
                Mod.Settings.DeleteStreetLights;

            bool deleteQuantityObjects =
                Mod.Settings.DeleteQuantityObjects;

            bool deleteBrandingObjects =
                Mod.Settings.DeleteBrandingObjects;

            bool deleteActivityLocations =
                Mod.Settings.DeleteActivityLocations;

            bool deleteSpawnLocations =
                Mod.Settings.DeleteSpawnLocations;

            bool deleteMarkerNetworks =
                Mod.Settings.DeleteMarkerNetworks;

            bool deleteBuildingSubObjects =
                Mod.Settings.DeleteBuildingSubObjects;

            bool deleteNetworkSubObjects =
                Mod.Settings.DeleteNetworkSubObjects;

            bool protectOwnedObjects =
                Mod.Settings.ProtectOwnedObjects;

            if (!deleteTrees &&
                !deleteBuildings &&
                !deleteRoads &&
                !deletePaths &&
                !deleteRailways &&
                !deleteSurfaces &&
                !deleteStaticObjects)
            {
                ClearSelectionPreview();
                return;
            }

            float radius =
                CurrentRadius;

            float currentTime =
                UnityEngine.Time.unscaledTime;

            bool rotationChanged =
                math.abs(
                    math.atan2(
                        math.sin(
                            SquareRotationRadians -
                            m_LastPreviewSquareRotationRadians),
                        math.cos(
                            SquareRotationRadians -
                            m_LastPreviewSquareRotationRadians))) >
                math.radians(0.05f);

            bool radiusChanged =
                math.abs(
                    radius -
                    m_LastPreviewRadius) > 0.01f;

            bool filtersChanged =
                useSquareBrush !=
                    m_LastUseSquareBrush ||
                deleteTrees != m_LastDeleteTrees ||
                deleteBuildings !=
                    m_LastDeleteBuildings ||
                deleteRoads !=
                    m_LastDeleteRoads ||
                deletePaths !=
                    m_LastDeletePaths ||
                deleteRailways !=
                    m_LastDeleteRailways ||
                deleteSurfaces !=
                    m_LastDeleteSurfaces ||
                deleteStaticObjects !=
                    m_LastDeleteStaticObjects ||
                deleteGeneralProps !=
                    m_LastDeleteGeneralProps ||
                deleteStreetLights !=
                    m_LastDeleteStreetLights ||
                deleteQuantityObjects !=
                    m_LastDeleteQuantityObjects ||
                deleteBrandingObjects !=
                    m_LastDeleteBrandingObjects ||
                deleteActivityLocations !=
                    m_LastDeleteActivityLocations ||
                deleteSpawnLocations !=
                    m_LastDeleteSpawnLocations ||
                deleteMarkerNetworks !=
                    m_LastDeleteMarkerNetworks ||
                deleteBuildingSubObjects !=
                    m_LastDeleteBuildingSubObjects ||
                deleteNetworkSubObjects !=
                    m_LastDeleteNetworkSubObjects ||
                protectOwnedObjects !=
                    m_LastProtectOwnedObjects;

            float2 currentPosition =
                new float2(
                    CurrentPosition.x,
                    CurrentPosition.z);

            float2 previousPosition =
                new float2(
                    m_LastPreviewPosition.x,
                    m_LastPreviewPosition.z);

            float movementThresholdSquared =
                kPreviewMoveThreshold *
                kPreviewMoveThreshold;

            bool movedEnough =
                math.distancesq(
                    currentPosition,
                    previousPosition) >=
                movementThresholdSquared;

            if (!movedEnough &&
                !radiusChanged &&
                !rotationChanged &&
                !filtersChanged)
            {
                return;
            }

            if (!radiusChanged &&
                !rotationChanged &&
                !filtersChanged &&
                currentTime <
                    m_NextPreviewUpdateTime)
            {
                return;
            }

            CancelLargeSelectionConfirmation();

            UpdateSelectionPreview(
                deleteTrees,
                deleteBuildings,
                deleteRoads,
                deletePaths,
                deleteRailways,
                deleteSurfaces,
                deleteStaticObjects,
                deleteSpawnLocations,
                deleteMarkerNetworks);

            m_LastPreviewPosition =
                CurrentPosition;

            m_LastPreviewRadius =
                radius;

            m_LastUseSquareBrush =
                useSquareBrush;

            m_LastPreviewSquareRotationRadians =
                SquareRotationRadians;

            m_LastDeleteTrees =
                deleteTrees;

            m_LastDeleteBuildings =
                deleteBuildings;

            m_LastDeleteRoads =
                deleteRoads;

            m_LastDeletePaths =
                deletePaths;

            m_LastDeleteRailways =
                deleteRailways;

            m_LastDeleteSurfaces =
                deleteSurfaces;

            m_LastDeleteStaticObjects =
                deleteStaticObjects;

            m_LastDeleteGeneralProps =
                deleteGeneralProps;

            m_LastDeleteStreetLights =
                deleteStreetLights;

            m_LastDeleteQuantityObjects =
                deleteQuantityObjects;

            m_LastDeleteBrandingObjects =
                deleteBrandingObjects;

            m_LastDeleteActivityLocations =
                deleteActivityLocations;

            m_LastDeleteSpawnLocations =
                deleteSpawnLocations;

            m_LastDeleteMarkerNetworks =
                deleteMarkerNetworks;

            m_LastDeleteBuildingSubObjects =
                deleteBuildingSubObjects;

            m_LastDeleteNetworkSubObjects =
                deleteNetworkSubObjects;

            m_LastProtectOwnedObjects =
                protectOwnedObjects;

            m_NextPreviewUpdateTime =
                currentTime +
                kPreviewUpdateInterval;
        }

        private void UpdateSelectionPreview(
            bool includeVegetation,
            bool includeBuildings,
            bool includeRoads,
            bool includePaths,
            bool includeRailways,
            bool includeSurfaces,
            bool includeStaticObjects,
            bool includeSpawnLocations,
            bool includeMarkerNetworks)
        {
            if (m_HighlightedEntities == null ||
                m_NextHighlightedEntities == null)
            {
                return;
            }

            m_NextHighlightedEntities.Clear();

            float radius =
                CurrentRadius;

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

            if (includeBuildings)
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
                        (m_PendingDeletion != null &&
                         m_PendingDeletion.Contains(
                             candidate.Entity)))
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
                Entity entity =
                    candidate.Entity;

                if (candidate.IsBuilding)
                {
                    if (!includeBuildings)
                    {
                        continue;
                    }
                }
                else if (candidate.IsRoad)
                {
                    if (!includeRoads)
                    {
                        continue;
                    }
                }
                else if (candidate.IsPedestrianPath)
                {
                    if (!includePaths)
                    {
                        continue;
                    }
                }
                else if (candidate.IsRailway)
                {
                    if (!includeRailways)
                    {
                        continue;
                    }
                }
                else if (candidate.IsSurfaceArea)
                {
                    if (!includeSurfaces)
                    {
                        continue;
                    }
                }
                else if (candidate.IsStaticObject)
                {
                    if (!includeStaticObjects ||
                        !IsStaticCategoryEnabled(
                            candidate.StaticCategory))
                    {
                        continue;
                    }
                }
                else if (candidate.IsSpawnLocation)
                {
                    if (!includeStaticObjects ||
                        !includeSpawnLocations)
                    {
                        continue;
                    }
                }
                else if (candidate.IsAssetLane)
                {
                    if (!includeStaticObjects ||
                        !includeMarkerNetworks)
                    {
                        continue;
                    }
                }
                else if (!includeVegetation)
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

                if (!IsEntityUsable(entity))
                {
                    continue;
                }

                if (m_PendingDeletion != null &&
                    m_PendingDeletion.Contains(entity))
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
                    continue;
                }

                if (candidate.IsBuilding)
                {
                    AddBuildingPreviewHierarchy(entity);
                    continue;
                }

                AddEntityToNextPreview(entity);
            }

            foreach (
                Entity entity
                in m_HighlightedEntities)
            {
                if (!m_NextHighlightedEntities.Contains(
                        entity))
                {
                    RemoveOwnedHighlight(entity);
                }
            }

            HashSet<Entity> previousSet =
                m_HighlightedEntities;

            m_HighlightedEntities =
                m_NextHighlightedEntities;

            m_NextHighlightedEntities =
                previousSet;

            m_NextHighlightedEntities.Clear();
        }
    }
}
