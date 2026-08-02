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
        // ------------------------------------------------------------
        // Hervorhebung
        // ------------------------------------------------------------

        private bool TryAddHighlight(
            Entity entity)
        {
            if (!IsEntityUsable(entity))
            {
                return false;
            }

            if (EntityManager.HasComponent<Game.Areas.Area>(
                    entity))
            {
                return true;
            }

            if (EntityManager.HasComponent<Highlighted>(
                    entity))
            {
                return false;
            }

            EntityManager.AddComponent<Highlighted>(
                entity);

            MarkHighlightVisualsUpdated(entity);

            return true;
        }

        private void AddEntityToNextPreview(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                m_NextHighlightedEntities == null)
            {
                return;
            }

            if (m_HighlightedEntities != null &&
                m_HighlightedEntities.Contains(entity))
            {
                m_NextHighlightedEntities.Add(entity);
                return;
            }

            if (TryAddHighlight(entity))
            {
                m_NextHighlightedEntities.Add(entity);
            }
        }

        private void AddBuildingPreviewHierarchy(
            Entity buildingEntity)
        {
            const int maximumDepth = 4;
            const int maximumEntities = 2048;

            HashSet<Entity> visited =
                new();

            AddBuildingPreviewHierarchyRecursive(
                buildingEntity,
                0,
                maximumDepth,
                maximumEntities,
                visited);
        }

        private void AddBuildingPreviewHierarchyRecursive(
            Entity entity,
            int depth,
            int maximumDepth,
            int maximumEntities,
            HashSet<Entity> visited)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                visited == null ||
                visited.Count >= maximumEntities ||
                !visited.Add(entity))
            {
                return;
            }

            AddEntityToNextPreview(entity);

            if (depth >= maximumDepth ||
                !EntityManager.HasBuffer<Game.Objects.SubObject>(
                    entity))
            {
                return;
            }

            DynamicBuffer<Game.Objects.SubObject> subObjects =
                EntityManager.GetBuffer<Game.Objects.SubObject>(
                    entity,
                    true);

            for (int index = 0;
                 index < subObjects.Length &&
                 visited.Count < maximumEntities;
                 index++)
            {
                Entity subObjectEntity =
                    subObjects[index].m_SubObject;

                AddBuildingPreviewHierarchyRecursive(
                    subObjectEntity,
                    depth + 1,
                    maximumDepth,
                    maximumEntities,
                    visited);
            }
        }

        private void RemoveOwnedHighlight(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return;
            }

            if (EntityManager.HasComponent<Game.Areas.Area>(
                    entity))
            {
                return;
            }

            if (!EntityManager.HasComponent<Highlighted>(
                    entity))
            {
                return;
            }

            EntityManager.RemoveComponent<Highlighted>(
                entity);

            MarkHighlightVisualsUpdated(entity);
        }

        private void RemoveHighlightBeforeDeletion(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return;
            }

            if (EntityManager.HasComponent<Game.Areas.Area>(
                    entity))
            {
                m_HighlightedEntities?.Remove(entity);
                m_NextHighlightedEntities?.Remove(entity);
                return;
            }

            if (EntityManager.HasComponent<Highlighted>(
                    entity))
            {
                EntityManager.RemoveComponent<Highlighted>(
                    entity);

                MarkHighlightVisualsUpdated(entity);
            }

            m_HighlightedEntities?.Remove(entity);
            m_NextHighlightedEntities?.Remove(entity);
        }


        private void MarkHighlightVisualsUpdated(
            Entity entity)
        {
            if (!EntityManager.Exists(entity))
            {
                return;
            }

            if (EntityManager.HasComponent<Game.Areas.Area>(
                    entity))
            {
                if (!EntityManager.HasComponent<Updated>(
                        entity))
                {
                    EntityManager.AddComponent<Updated>(
                        entity);
                }

                return;
            }

            MarkBatchesUpdated(entity);
        }

        private void MarkBatchesUpdated(
            Entity entity)
        {
            if (!EntityManager.Exists(entity))
            {
                return;
            }

            if (!EntityManager.HasComponent<BatchesUpdated>(
                    entity))
            {
                EntityManager.AddComponent<BatchesUpdated>(
                    entity);
            }
        }

        private void ClearSelectionPreview(
            bool resetPreviewState = true)
        {
            if (m_HighlightedEntities == null)
            {
                return;
            }

            foreach (Entity entity in
                     m_HighlightedEntities)
            {
                RemoveOwnedHighlight(entity);
            }

            m_HighlightedEntities.Clear();
            m_NextHighlightedEntities?.Clear();

            if (resetPreviewState)
            {
                ResetPreviewState();
            }
        }

        private void ResetPreviewState()
        {
            CancelLargeSelectionConfirmation();

            m_LastPreviewPosition =
                float3.zero;

            m_LastPreviewRadius =
                -1f;

            m_NextPreviewUpdateTime =
                0f;

            m_LastUseSquareBrush =
                false;

            m_LastPreviewSquareRotationRadians =
                -1000f;

            m_LastDeleteTrees =
                false;

            m_LastDeleteBuildings =
                false;

            m_LastDeleteRoads =
                false;

            m_LastDeletePaths =
                false;

            m_LastDeleteRailways =
                false;

            m_LastDeleteSurfaces =
                false;

            m_LastDeleteStaticObjects =
                false;

            m_LastDeleteGeneralProps =
                false;

            m_LastDeleteStreetLights =
                false;

            m_LastDeleteQuantityObjects =
                false;

            m_LastDeleteBrandingObjects =
                false;

            m_LastDeleteActivityLocations =
                false;

            m_LastDeleteSpawnLocations =
                false;

            m_LastDeleteMarkerNetworks =
                false;

            m_LastDeleteBuildingSubObjects =
                false;

            m_LastDeleteNetworkSubObjects =
                false;

            m_LastProtectOwnedObjects =
                false;
        }

        private void CleanupPendingDeletions()
        {
            if (m_PendingDeletion == null ||
                m_PendingDeletion.Count == 0)
            {
                return;
            }

            m_PendingDeletion.RemoveWhere(
                entity =>
                    entity == Entity.Null ||
                    !EntityManager.Exists(entity) ||
                    EntityManager.HasComponent<Deleted>(
                        entity));
        }
    }
}
