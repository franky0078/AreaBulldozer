using Game;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {

        private void BeginLargeSelectionConfirmation(
            HashSet<Entity> selectedEntities,
            int vegetationCount,
            int buildingCount,
            int roadCount,
            int pathCount,
            int railwayCount,
            int surfaceCount,
            int staticObjectCount,
            int spawnLocationCount,
            int markerNetworkCount,
            int protectedTotal)
        {
            m_LargeSelectionConfirmationPending =
                true;

            m_LargeSelectionConfirmationPosition =
                CurrentPosition;

            m_LargeSelectionConfirmationRadius =
                CurrentRadius;

            m_LargeSelectionConfirmationUseSquareBrush =
                UseSquareBrush;

            m_LargeSelectionConfirmationSquareRotationRadians =
                SquareRotationRadians;

            m_LargeSelectionConfirmationExpiresAt =
                UnityEngine.Time.unscaledTime +
                kLargeSelectionConfirmationTimeout;

            m_ConfirmationFilterSnapshot =
                FilterSnapshot.FromSettings(
                    Mod.Settings);

            m_ConfirmationEntities ??=
                new();

            m_ConfirmationEntities.Clear();

            foreach (Entity entity in selectedEntities)
            {
                m_ConfirmationEntities.Add(entity);
            }

            m_ConfirmationObjectCount =
                selectedEntities.Count;

            m_ConfirmationThreshold =
                CurrentLargeSelectionThreshold;

            m_ConfirmationVegetationCount =
                vegetationCount;

            m_ConfirmationBuildingCount =
                buildingCount;

            m_ConfirmationRoadCount =
                roadCount;

            m_ConfirmationPathCount =
                pathCount;

            m_ConfirmationRailwayCount =
                railwayCount;

            m_ConfirmationSurfaceCount =
                surfaceCount;

            m_ConfirmationStaticObjectCount =
                staticObjectCount;

            m_ConfirmationSpawnLocationCount =
                spawnLocationCount;

            m_ConfirmationMarkerNetworkCount =
                markerNetworkCount;

            SafeLogWarn(
                $"Area Bulldozer: {selectedEntities.Count} objects selected. " +
                $"Click again within " +
                $"{kLargeSelectionConfirmationTimeout:0} seconds " +
                $"without moving the brush to confirm deletion. " +
                $"Vegetation: {vegetationCount}, " +
                $"buildings: {buildingCount}, " +
                $"roads: {roadCount}, " +
                $"pedestrian paths: {pathCount}, " +
                $"railway tracks: {railwayCount}, " +
                $"surfaces and spaces: {surfaceCount}, " +
                $"static objects: {staticObjectCount}, " +
                $"spawn locations: {spawnLocationCount}, " +
                $"asset lanes: {markerNetworkCount}, " +
                $"protected sub-objects ignored: {protectedTotal}.");
        }

        private bool IsMatchingLargeSelectionConfirmation(
            HashSet<Entity> selectedEntities,
            int vegetationCount,
            int buildingCount,
            int roadCount,
            int pathCount,
            int railwayCount,
            int surfaceCount,
            int staticObjectCount,
            int spawnLocationCount,
            int markerNetworkCount)
        {
            if (!m_LargeSelectionConfirmationPending ||
                Mod.Settings == null)
            {
                return false;
            }

            float currentTime =
                UnityEngine.Time.unscaledTime;

            if (currentTime >
                m_LargeSelectionConfirmationExpiresAt)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation expired.");

                return false;
            }

            float2 currentPosition =
                new float2(
                    CurrentPosition.x,
                    CurrentPosition.z);

            float2 confirmationPosition =
                new float2(
                    m_LargeSelectionConfirmationPosition.x,
                    m_LargeSelectionConfirmationPosition.z);

            float moveToleranceSquared =
                kLargeSelectionMoveTolerance *
                kLargeSelectionMoveTolerance;

            if (math.distancesq(
                    currentPosition,
                    confirmationPosition) >
                moveToleranceSquared)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the brush moved.");

                return false;
            }

            if (math.abs(
                    CurrentRadius -
                    m_LargeSelectionConfirmationRadius) > 0.01f)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the size changed.");

                return false;
            }

            if (UseSquareBrush !=
                m_LargeSelectionConfirmationUseSquareBrush)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the shape changed.");

                return false;
            }

            if (UseSquareBrush &&
                math.abs(
                    math.atan2(
                        math.sin(
                            SquareRotationRadians -
                            m_LargeSelectionConfirmationSquareRotationRadians),
                        math.cos(
                            SquareRotationRadians -
                            m_LargeSelectionConfirmationSquareRotationRadians))) >
                math.radians(0.05f))
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the square rotated.");

                return false;
            }

            bool filtersMatch =
                FilterSnapshot.FromSettings(
                    Mod.Settings) ==
                m_ConfirmationFilterSnapshot;

            if (!filtersMatch)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the filters changed.");

                return false;
            }

            if (CurrentLargeSelectionThreshold !=
                m_ConfirmationThreshold)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the threshold changed.");

                return false;
            }

            bool exactSelectionMatches =
                m_ConfirmationEntities != null &&
                selectedEntities.Count ==
                    m_ConfirmationObjectCount &&
                m_ConfirmationEntities.SetEquals(
                    selectedEntities);

            bool categoryCountsMatch =
                vegetationCount ==
                    m_ConfirmationVegetationCount &&
                buildingCount ==
                    m_ConfirmationBuildingCount &&
                roadCount ==
                    m_ConfirmationRoadCount &&
                pathCount ==
                    m_ConfirmationPathCount &&
                railwayCount ==
                    m_ConfirmationRailwayCount &&
                surfaceCount ==
                    m_ConfirmationSurfaceCount &&
                staticObjectCount ==
                    m_ConfirmationStaticObjectCount &&
                spawnLocationCount ==
                    m_ConfirmationSpawnLocationCount &&
                markerNetworkCount ==
                    m_ConfirmationMarkerNetworkCount;

            if (!exactSelectionMatches ||
                !categoryCountsMatch)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the exact selection changed.");

                return false;
            }

            return true;
        }

        private void UpdateLargeSelectionConfirmationState()
        {
            if (!m_LargeSelectionConfirmationPending)
            {
                return;
            }

            if (Mod.Settings == null ||
                !Mod.Settings.ConfirmLargeSelection)
            {
                CancelLargeSelectionConfirmation();
                return;
            }

            if (CurrentLargeSelectionThreshold !=
                m_ConfirmationThreshold)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the threshold changed.");

                return;
            }

            if (UnityEngine.Time.unscaledTime >
                m_LargeSelectionConfirmationExpiresAt)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation expired.");
            }
        }

        private void CancelLargeSelectionConfirmation(
            string logReason = null)
        {
            if (!m_LargeSelectionConfirmationPending)
            {
                return;
            }

            m_LargeSelectionConfirmationPending =
                false;

            m_LargeSelectionConfirmationPosition =
                float3.zero;

            m_LargeSelectionConfirmationRadius =
                0f;

            m_LargeSelectionConfirmationUseSquareBrush =
                false;

            m_LargeSelectionConfirmationSquareRotationRadians =
                0f;

            m_LargeSelectionConfirmationExpiresAt =
                0f;

            m_ConfirmationFilterSnapshot =
                default;

            m_ConfirmationObjectCount =
                0;

            m_ConfirmationVegetationCount =
                0;

            m_ConfirmationBuildingCount =
                0;

            m_ConfirmationRoadCount =
                0;

            m_ConfirmationPathCount =
                0;

            m_ConfirmationRailwayCount =
                0;

            m_ConfirmationSurfaceCount =
                0;

            m_ConfirmationStaticObjectCount =
                0;

            m_ConfirmationSpawnLocationCount =
                0;

            m_ConfirmationMarkerNetworkCount =
                0;

            m_ConfirmationThreshold =
                0;

            m_ConfirmationEntities?.Clear();

            if (!string.IsNullOrWhiteSpace(
                    logReason))
            {
                SafeLogInfo(logReason);
            }
        }
    }
}
