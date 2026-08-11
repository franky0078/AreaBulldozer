using Game;
using Game.Common;
using Game.Net;
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

        private bool IsEntityUsable(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return false;
            }

            if (EntityManager.HasComponent<Deleted>(
                    entity) ||
                EntityManager.HasComponent<Temp>(
                    entity) ||
                EntityManager.HasComponent<Overridden>(
                    entity))
            {
                return false;
            }

            return true;
        }

        private bool IsInsideCircle(
            Entity entity,
            float2 circleCenter,
            float radiusSquared)
        {
            if (!EntityManager.HasComponent<Transform>(
                    entity))
            {
                return false;
            }

            Transform transform =
                EntityManager.GetComponentData<Transform>(
                    entity);

            float2 entityPosition =
                new float2(
                    transform.m_Position.x,
                    transform.m_Position.z);

            return math.distancesq(
                entityPosition,
                circleCenter) <= radiusSquared;
        }

        private bool IsOwnedObjectProtected(
            Entity entity,
            in FilterSnapshot filters)
        {
            OwnerScope scope =
                ResolveOwnerScope(entity);

            return scope switch
            {
                OwnerScope.None => false,
                OwnerScope.Building =>
                    !filters.DeleteBuildingSubObjects,
                OwnerScope.Network =>
                    !filters.DeleteNetworkSubObjects,
                OwnerScope.Other =>
                    filters.ProtectOwnedObjects,
                _ => true
            };
        }

        private bool IsCandidateOwnedObjectProtected(
            in SpatialCandidate candidate,
            in FilterSnapshot filters)
        {
            if (candidate.IsStaticObject &&
                candidate.StaticCategory !=
                    StaticObjectCategory.ActivityLocation)
            {
                return false;
            }

            return IsOwnedObjectProtected(
                candidate.Entity,
                in filters);
        }

        private OwnerScope ResolveOwnerScope(
            Entity entity)
        {
            Entity current = entity;
            bool hadOwner = false;

            for (int depth = 0; depth < 8; depth++)
            {
                if (current == Entity.Null ||
                    !EntityManager.Exists(current) ||
                    !EntityManager.HasComponent<Owner>(current))
                {
                    return hadOwner
                        ? OwnerScope.Other
                        : OwnerScope.None;
                }

                Owner owner =
                    EntityManager.GetComponentData<Owner>(current);

                if (owner.m_Owner == Entity.Null ||
                    !EntityManager.Exists(owner.m_Owner))
                {
                    return OwnerScope.Other;
                }

                hadOwner = true;
                current = owner.m_Owner;

                if (IsBuildingEntity(current))
                {
                    return OwnerScope.Building;
                }

                if (IsNetworkEntity(current))
                {
                    return OwnerScope.Network;
                }
            }

            return OwnerScope.Other;
        }

        private bool IsBuildingEntity(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return false;
            }

            if (EntityManager.HasComponent<
                    Game.Buildings.Building>(entity))
            {
                return true;
            }

            return TryGetPrefabBase(
                       entity,
                       out PrefabBase prefabBase) &&
                   prefabBase is BuildingPrefab;
        }

        private bool IsRootBuildingEntity(
            Entity entity)
        {
            if (!IsBuildingEntity(entity))
            {
                return false;
            }

            if (!EntityManager.HasComponent<Owner>(entity))
            {
                return true;
            }

            Owner owner =
                EntityManager.GetComponentData<Owner>(entity);

            if (owner.m_Owner == Entity.Null ||
                !EntityManager.Exists(owner.m_Owner))
            {
                return true;
            }

            return !IsBuildingEntity(owner.m_Owner);
        }

        private bool TryGetOwningRootBuilding(
            Entity entity,
            out Entity buildingEntity)
        {
            buildingEntity = Entity.Null;
            Entity current = entity;

            for (int depth = 0; depth < 8; depth++)
            {
                if (current == Entity.Null ||
                    !EntityManager.Exists(current) ||
                    !EntityManager.HasComponent<Owner>(current))
                {
                    return false;
                }

                Owner owner =
                    EntityManager.GetComponentData<Owner>(current);

                current = owner.m_Owner;

                if (current == Entity.Null ||
                    !EntityManager.Exists(current))
                {
                    return false;
                }

                if (IsRootBuildingEntity(current))
                {
                    buildingEntity = current;
                    return true;
                }
            }

            return false;
        }

        private bool IsNetworkEntity(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return false;
            }

            if (EntityManager.HasComponent<
                    Game.Net.Edge>(entity) ||
                EntityManager.HasComponent<
                    Game.Net.Node>(entity))
            {
                return true;
            }

            return TryGetPrefabBase(
                       entity,
                       out PrefabBase prefabBase) &&
                   (prefabBase is NetPrefab ||
                    prefabBase is RoadPrefab);
        }

        private bool TryGetPrefabBase(
            Entity entity,
            out PrefabBase prefabBase)
        {
            prefabBase = null;

            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                !EntityManager.HasComponent<PrefabRef>(entity))
            {
                return false;
            }

            PrefabRef prefabRef =
                EntityManager.GetComponentData<PrefabRef>(entity);

            return prefabRef.m_Prefab != Entity.Null &&
                   m_PrefabSystem.TryGetPrefab(
                       prefabRef.m_Prefab,
                       out prefabBase) &&
                   prefabBase != null;
        }


        private bool IsMainRoadEdgeEntity(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                !EntityManager.HasComponent<Edge>(entity) ||
                !EntityManager.HasComponent<Road>(entity))
            {
                return false;
            }

            if (TryGetPrefabBase(
                    entity,
                    out PrefabBase prefabBase) &&
                (IsPedestrianPathPrefab(prefabBase) ||
                 IsRailwayTrackPrefab(prefabBase)))
            {
                return false;
            }

            if (EntityManager.HasComponent<Owner>(entity))
            {
                return IsAggregateOwner(entity);
            }

            return prefabBase is RoadPrefab;
        }

        private bool IsMainPedestrianPathEdgeEntity(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                !EntityManager.HasComponent<Edge>(entity) ||
                !TryGetPrefabBase(
                    entity,
                    out PrefabBase prefabBase) ||
                !IsPedestrianPathPrefab(prefabBase))
            {
                return false;
            }

            return !EntityManager.HasComponent<Owner>(entity) ||
                   IsAggregateOwner(entity);
        }

        private bool IsMainRailwayEdgeEntity(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity) ||
                !EntityManager.HasComponent<Edge>(entity) ||
                !TryGetPrefabBase(
                    entity,
                    out PrefabBase prefabBase) ||
                !IsRailwayTrackPrefab(prefabBase))
            {
                return false;
            }

            return !EntityManager.HasComponent<Owner>(entity) ||
                   IsAggregateOwner(entity);
        }

        private bool IsAggregateOwner(
            Entity entity)
        {
            if (!EntityManager.HasComponent<Owner>(entity))
            {
                return false;
            }

            Owner owner =
                EntityManager.GetComponentData<Owner>(entity);

            return owner.m_Owner != Entity.Null &&
                   EntityManager.Exists(owner.m_Owner) &&
                   EntityManager.HasComponent<Aggregate>(
                       owner.m_Owner) &&
                   EntityManager.HasBuffer<AggregateElement>(
                       owner.m_Owner);
        }

        private static bool IsPrefabDerivedFrom(
            PrefabBase prefabBase,
            string typeName)
        {
            for (System.Type type = prefabBase?.GetType();
                 type != null;
                 type = type.BaseType)
            {
                if (type.Name == typeName)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPedestrianPathPrefab(
            PrefabBase prefabBase)
        {
            if (prefabBase == null)
            {
                return false;
            }

            if (IsPrefabDerivedFrom(
                    prefabBase,
                    "PathwayPrefab"))
            {
                return true;
            }

            if (!(prefabBase is NetPrefab) &&
                !(prefabBase is RoadPrefab))
            {
                return false;
            }

            string prefabName =
                (prefabBase.name ?? string.Empty)
                    .Trim()
                    .ToLowerInvariant();

            string typeName =
                prefabBase
                    .GetType()
                    .Name
                    .ToLowerInvariant();

            bool pathLike =
                typeName.Contains("pedestrian") ||
                typeName.Contains("footpath") ||
                typeName.Contains("pathway") ||
                prefabName.Contains("pedestrian") ||
                prefabName.Contains("footpath") ||
                prefabName.Contains("foot path") ||
                prefabName.Contains("walkway") ||
                prefabName.Contains("walking path") ||
                prefabName.Contains("pathway") ||
                prefabName.Contains("gravel path") ||
                prefabName.Contains("paved path") ||
                prefabName.Contains("invisible path") ||
                prefabName.EndsWith(" path") ||
                prefabName.StartsWith("path ");

            if (!pathLike)
            {
                return false;
            }

            return
                !prefabName.Contains("track") &&
                !prefabName.Contains("rail") &&
                !prefabName.Contains("train") &&
                !prefabName.Contains("subway") &&
                !prefabName.Contains("metro") &&
                !prefabName.Contains("tram") &&
                !prefabName.Contains("water pipe") &&
                !prefabName.Contains("sewage") &&
                !prefabName.Contains("power line") &&
                !prefabName.Contains("electricity") &&
                !prefabName.Contains("marker");
        }

        private static bool IsRailwayTrackPrefab(
            PrefabBase prefabBase)
        {
            if (prefabBase == null)
            {
                return false;
            }

            if (IsPedestrianPathPrefab(prefabBase))
            {
                return false;
            }

            if (IsPrefabDerivedFrom(
                    prefabBase,
                    "TrackPrefab"))
            {
                return true;
            }

            if (!(prefabBase is NetPrefab) &&
                !(prefabBase is RoadPrefab))
            {
                return false;
            }

            string prefabName =
                (prefabBase.name ?? string.Empty)
                    .Trim()
                    .ToLowerInvariant();

            string typeName =
                prefabBase
                    .GetType()
                    .Name
                    .ToLowerInvariant();

            bool railwayLike =
                typeName.Contains("track") ||
                typeName.Contains("rail") ||
                prefabName.Contains("train track") ||
                prefabName.Contains("rail track") ||
                prefabName.Contains("railway") ||
                prefabName.Contains("rail road") ||
                prefabName.Contains("railroad") ||
                prefabName.Contains("subway track") ||
                prefabName.Contains("metro track") ||
                prefabName.Contains("tram track") ||
                prefabName.Contains("streetcar track") ||
                prefabName.Contains("train rail") ||
                prefabName.EndsWith(" track") ||
                prefabName.StartsWith("track ");

            if (!railwayLike)
            {
                return false;
            }

            return
                !prefabName.Contains("water pipe") &&
                !prefabName.Contains("sewage") &&
                !prefabName.Contains("power line") &&
                !prefabName.Contains("electricity") &&
                !prefabName.Contains("marker") &&
                !prefabName.Contains("parking") &&
                !prefabName.Contains("fence") &&
                !prefabName.Contains("hedge");
        }


        private bool IsStaticObjectEntity(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return false;
            }

            // First protection layer
            if (EntityManager.HasComponent<
                    Game.Buildings.Building>(entity))
            {
                return false;
            }

            if (!TryGetPrefabBase(
                    entity,
                    out PrefabBase prefabBase))
            {
                return false;
            }

            // Second protection layer
            if (prefabBase is BuildingPrefab)
            {
                return false;
            }

            return prefabBase is StaticObjectPrefab;
        }
    }
}
