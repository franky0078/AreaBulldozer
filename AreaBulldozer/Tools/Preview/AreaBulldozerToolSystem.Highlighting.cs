using AreaBulldozer.Diagnostics;
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
        private readonly List<Entity> m_HighlightAddBuffer =
            new();

        private readonly List<Entity> m_HighlightRemoveBuffer =
            new();

        private readonly List<Entity> m_HighlightNotOwnedBuffer =
            new();

        private readonly List<Entity> m_HighlightBatchAdd = new();
        private readonly List<Entity> m_HighlightBatchRemove = new();
        private readonly List<Entity> m_HighlightBatchUpdated = new();
        private readonly HashSet<Entity> m_HighlightBatchUpdatedSet = new();

        private readonly HashSet<Entity> m_PreviewHierarchyVisited = new();

        private readonly List<Entity> m_HighlightedAreaEntities = new();
        private bool m_HighlightedAreaEntitiesDirty = true;

        private void AddEntityToNextPreview(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                m_NextHighlightedEntities == null)
            {
                return;
            }

            m_NextHighlightedEntities.Add(entity);
        }

        private void AddBuildingPreviewHierarchy(
            Entity buildingEntity)
        {
            const int maximumDepth = 4;
            const int maximumEntities = 2048;

            m_PreviewHierarchyVisited.Clear();

            AddBuildingPreviewHierarchyRecursive(
                buildingEntity,
                0,
                maximumDepth,
                maximumEntities,
                m_PreviewHierarchyVisited);
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

        private void ApplyPreviewHighlightDiff()
        {
            if (m_HighlightedEntities == null ||
                m_NextHighlightedEntities == null)
            {
                return;
            }

            m_HighlightAddBuffer.Clear();
            m_HighlightRemoveBuffer.Clear();
            m_HighlightNotOwnedBuffer.Clear();

            foreach (Entity entity in m_NextHighlightedEntities)
            {
                if (m_HighlightedEntities.Contains(entity))
                {
                    continue;
                }

                if (!EntityManager.Exists(entity))
                {
                    m_HighlightNotOwnedBuffer.Add(entity);
                    continue;
                }

                if (EntityManager.HasComponent<Game.Areas.Area>(
                        entity))
                {
                    continue;
                }

                if (EntityManager.HasComponent<Highlighted>(
                        entity))
                {
                    m_HighlightNotOwnedBuffer.Add(entity);
                    continue;
                }

                m_HighlightAddBuffer.Add(entity);
            }

            foreach (Entity entity in m_HighlightNotOwnedBuffer)
            {
                m_NextHighlightedEntities.Remove(entity);
            }

            foreach (Entity entity in m_HighlightedEntities)
            {
                if (m_NextHighlightedEntities.Contains(entity))
                {
                    continue;
                }

                if (!EntityManager.Exists(entity) ||
                    EntityManager.HasComponent<Game.Areas.Area>(
                        entity) ||
                    !EntityManager.HasComponent<Highlighted>(
                        entity))
                {
                    continue;
                }

                m_HighlightRemoveBuffer.Add(entity);
            }

            ApplyHighlightComponentChanges(
                m_HighlightAddBuffer,
                m_HighlightRemoveBuffer);

            HashSet<Entity> previousSet =
                m_HighlightedEntities;

            m_HighlightedEntities =
                m_NextHighlightedEntities;

            m_NextHighlightedEntities =
                previousSet;

            m_NextHighlightedEntities.Clear();
            m_HighlightedAreaEntitiesDirty = true;
        }

        private void ApplyHighlightComponentChanges(
            List<Entity> entitiesToHighlight,
            List<Entity> entitiesToUnhighlight)
        {
            long applyStart = PerfTrace.Begin();

            m_HighlightBatchAdd.Clear();
            m_HighlightBatchRemove.Clear();
            m_HighlightBatchUpdated.Clear();
            m_HighlightBatchUpdatedSet.Clear();

            if (entitiesToHighlight != null)
            {
                foreach (Entity entity in entitiesToHighlight)
                {
                    if (entity == Entity.Null ||
                        !EntityManager.Exists(entity))
                    {
                        continue;
                    }

                    if (!EntityManager.HasComponent<Highlighted>(
                            entity))
                    {
                        m_HighlightBatchAdd.Add(entity);
                    }

                    QueueBatchesUpdated(entity);
                }
            }

            if (entitiesToUnhighlight != null)
            {
                foreach (Entity entity in entitiesToUnhighlight)
                {
                    if (entity == Entity.Null ||
                        !EntityManager.Exists(entity))
                    {
                        continue;
                    }

                    if (EntityManager.HasComponent<Highlighted>(
                            entity))
                    {
                        m_HighlightBatchRemove.Add(entity);
                    }

                    QueueBatchesUpdated(entity);
                }
            }

            ApplyComponentBatch(m_HighlightBatchAdd, ComponentType.ReadWrite<Highlighted>(), add: true);
            ApplyComponentBatch(m_HighlightBatchRemove, ComponentType.ReadWrite<Highlighted>(), add: false);
            ApplyComponentBatch(m_HighlightBatchUpdated, ComponentType.ReadWrite<BatchesUpdated>(), add: true);

            PerfTrace.End(
                PerfTrace.HighlightApply,
                applyStart,
                m_HighlightBatchAdd.Count + m_HighlightBatchRemove.Count);
        }

        private void QueueBatchesUpdated(Entity entity)
        {
            if (!EntityManager.HasComponent<BatchesUpdated>(entity) &&
                m_HighlightBatchUpdatedSet.Add(entity))
            {
                m_HighlightBatchUpdated.Add(entity);
            }
        }

        private void ApplyComponentBatch(
            List<Entity> entities,
            ComponentType componentType,
            bool add)
        {
            if (entities.Count == 0)
            {
                return;
            }

            if (entities.Count == 1)
            {
                if (add)
                {
                    EntityManager.AddComponent(entities[0], componentType);
                }
                else
                {
                    EntityManager.RemoveComponent(entities[0], componentType);
                }

                return;
            }

            NativeArray<Entity> batch =
                new(entities.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            try
            {
                for (int index = 0; index < entities.Count; index++)
                {
                    batch[index] = entities[index];
                }

                if (add)
                {
                    EntityManager.AddComponent(batch, componentType);
                }
                else
                {
                    EntityManager.RemoveComponent(batch, componentType);
                }
            }
            finally
            {
                batch.Dispose();
            }
        }

        private void RemoveHighlightsBeforeDeletion(
            HashSet<Entity> entities)
        {
            if (entities == null ||
                entities.Count == 0)
            {
                return;
            }

            m_HighlightRemoveBuffer.Clear();

            m_HighlightedAreaEntitiesDirty = true;

            foreach (Entity entity in entities)
            {
                m_HighlightedEntities?.Remove(entity);
                m_NextHighlightedEntities?.Remove(entity);

                if (entity == Entity.Null ||
                    !EntityManager.Exists(entity))
                {
                    continue;
                }

                if (EntityManager.HasComponent<Game.Areas.Area>(
                        entity))
                {
                    continue;
                }

                if (EntityManager.HasComponent<Highlighted>(
                        entity))
                {
                    m_HighlightRemoveBuffer.Add(entity);
                }
            }

            ApplyHighlightComponentChanges(
                null,
                m_HighlightRemoveBuffer);
        }

        private void ClearSelectionPreview(
            bool resetPreviewState = true)
        {
            if (m_HighlightedEntities == null)
            {
                return;
            }

            m_HighlightRemoveBuffer.Clear();

            foreach (Entity entity in
                     m_HighlightedEntities)
            {
                if (entity == Entity.Null ||
                    !EntityManager.Exists(entity) ||
                    EntityManager.HasComponent<Game.Areas.Area>(
                        entity) ||
                    !EntityManager.HasComponent<Highlighted>(
                        entity))
                {
                    continue;
                }

                m_HighlightRemoveBuffer.Add(entity);
            }

            ApplyHighlightComponentChanges(
                null,
                m_HighlightRemoveBuffer);

            m_HighlightedEntities.Clear();
            m_NextHighlightedEntities?.Clear();
            m_HighlightedAreaEntities.Clear();
            m_HighlightedAreaEntitiesDirty = false;

            if (resetPreviewState)
            {
                ResetPreviewState();
            }
        }

        private List<Entity> GetHighlightedAreaEntities()
        {
            if (!m_HighlightedAreaEntitiesDirty)
            {
                return m_HighlightedAreaEntities;
            }

            m_HighlightedAreaEntities.Clear();
            m_HighlightedAreaEntitiesDirty = false;

            if (m_HighlightedEntities == null)
            {
                return m_HighlightedAreaEntities;
            }

            foreach (Entity entity in m_HighlightedEntities)
            {
                if (entity != Entity.Null &&
                    EntityManager.Exists(entity) &&
                    EntityManager.HasComponent<Game.Areas.Area>(entity) &&
                    EntityManager.HasBuffer<Game.Areas.Node>(entity))
                {
                    m_HighlightedAreaEntities.Add(entity);
                }
            }

            return m_HighlightedAreaEntities;
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

            m_LastPreviewSelectionShape =
                (AreaBulldozerSelectionShape)(-1);

            m_LastPreviewLineWidth =
                -1;

            m_LastPreviewUseCurvedPolyline =
                false;

            m_LastPreviewPolylineRounding =
                -1;

            m_LastPreviewSquareRotationRadians =
                -1000f;

            m_LastFilterSnapshot =
                default;
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
