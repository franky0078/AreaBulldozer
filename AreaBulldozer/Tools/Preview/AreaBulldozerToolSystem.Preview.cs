using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {

        private void UpdateSelectionPreviewIfNeeded()
        {
            if (!HasValidPosition ||
                Mod.Settings == null)
            {
                ClearSelectionPreview();
                return;
            }

            FilterSnapshot filters =
                FilterSnapshot.FromSettings(
                    Mod.Settings);

            bool useSquareBrush =
                Mod.Settings.UseSquareBrush;

            if (!filters.HasAnyPrimaryFilter)
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
                filters !=
                    m_LastFilterSnapshot;

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
                in filters);

            m_LastPreviewPosition =
                CurrentPosition;

            m_LastPreviewRadius =
                radius;

            m_LastUseSquareBrush =
                useSquareBrush;

            m_LastPreviewSquareRotationRadians =
                SquareRotationRadians;

            m_LastFilterSnapshot =
                filters;

            m_NextPreviewUpdateTime =
                currentTime +
                kPreviewUpdateInterval;
        }

        private void UpdateSelectionPreview(
            in FilterSnapshot filters)
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

            if (filters.DeleteBuildings)
            {
                foreach (
                    SpatialCandidate candidate
                    in candidates)
                {
                    if (!candidate.IsBuilding ||
                        !IsCandidateInsideSelection(
                            in candidate,
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
                    if (!filters.DeleteBuildings)
                    {
                        continue;
                    }
                }
                else if (candidate.IsRoad)
                {
                    if (!filters.DeleteRoads)
                    {
                        continue;
                    }
                }
                else if (candidate.IsPedestrianPath)
                {
                    if (!filters.DeletePaths)
                    {
                        continue;
                    }
                }
                else if (candidate.IsRailway)
                {
                    if (!filters.DeleteRailways)
                    {
                        continue;
                    }
                }
                else if (candidate.IsSurfaceArea)
                {
                    if (!filters.DeleteSurfaces)
                    {
                        continue;
                    }
                }
                else if (candidate.IsStaticObject)
                {
                    if (!IsStaticCategoryEnabled(
                            candidate.StaticCategory,
                            in filters))
                    {
                        continue;
                    }
                }
                else if (candidate.IsSpawnLocation)
                {
                    if (!filters.DeleteStaticObjects ||
                        !filters.DeleteSpawnLocations)
                    {
                        continue;
                    }
                }
                else if (candidate.IsAssetLane)
                {
                    if (!filters.DeleteStaticObjects ||
                        !filters.DeleteMarkerNetworks)
                    {
                        continue;
                    }
                }
                else if (!filters.DeleteTrees)
                {
                    continue;
                }

                if (!IsCandidateInsideSelection(
                        in candidate,
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
                    IsCandidateOwnedObjectProtected(
                        in candidate,
                        in filters))
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

            ApplyPreviewHighlightDiff();
        }
    }
}
