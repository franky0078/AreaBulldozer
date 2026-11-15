using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {

        // Statische Objektkategorien
        private StaticObjectCategory GetStaticObjectCategory(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return StaticObjectCategory.None;
            }

            if (IsActivityLocationTarget(entity))
            {
                return StaticObjectCategory.ActivityLocation;
            }

            if (EntityManager.HasComponent<StreetLight>(
                    entity))
            {
                return StaticObjectCategory.StreetLight;
            }

            if (EntityManager.HasComponent<Quantity>(
                    entity))
            {
                return StaticObjectCategory.QuantityObject;
            }

            if (EntityManager.HasComponent<PrefabRef>(
                    entity))
            {
                PrefabRef prefabRef =
                    EntityManager.GetComponentData<PrefabRef>(
                        entity);

                if (prefabRef.m_Prefab != Entity.Null &&
                    EntityManager.HasComponent<BrandObjectData>(
                        prefabRef.m_Prefab))
                {
                    return StaticObjectCategory.BrandingObject;
                }
            }

            return StaticObjectCategory.GeneralProp;
        }

        private bool TryGetPointMarkerPrefab(Entity entity, out Entity prefab)
        {
            prefab = Entity.Null;
            if (EntityManager.HasComponent<PrefabRef>(entity))
            {
                prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
            }

            if (EntityManager.HasComponent<Game.Objects.Marker>(entity))
            {
                return true;
            }

            if (EntityManager.Exists(prefab) && EntityManager.HasComponent<ObjectGeometryData>(prefab))
            {
                ObjectGeometryData geometry = EntityManager.GetComponentData<ObjectGeometryData>(prefab);
                if ((geometry.m_Flags & Game.Objects.GeometryFlags.Marker) != 0)
                {
                    return true;
                }

                if ((geometry.m_Layers & MeshLayer.Default) != 0 ||
                    (geometry.m_Flags & Game.Objects.GeometryFlags.Physical) != 0)
                {
                    return false;
                }

                if ((geometry.m_Layers & MeshLayer.Marker) != 0)
                {
                    return true;
                }
            }

            // Meshless position objects can define a real point without a marker mesh.
            return EntityManager.Exists(prefab) &&
                   EntityManager.HasComponent<Game.Prefabs.SpawnLocationData>(prefab) &&
                   (!EntityManager.HasBuffer<Game.Prefabs.SubMesh>(prefab) ||
                    EntityManager.GetBuffer<Game.Prefabs.SubMesh>(prefab, true).Length == 0);
        }

        private bool IsActivityLocationTarget(Entity entity)
        {
            bool hasActivity = EntityManager.HasComponent<Game.Objects.ActivityLocation>(entity);
            if (!hasActivity && EntityManager.HasComponent<PrefabRef>(entity))
            {
                Entity prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
                hasActivity = EntityManager.Exists(prefab) &&
                    (EntityManager.HasComponent<Game.Prefabs.ActivityLocationData>(prefab) ||
                     (EntityManager.HasBuffer<Game.Prefabs.ActivityLocationElement>(prefab) &&
                      EntityManager.GetBuffer<Game.Prefabs.ActivityLocationElement>(prefab, true).Length != 0));
            }

            return hasActivity && (IsStaticObjectEntity(entity) || TryGetPointMarkerPrefab(entity, out _));
        }

        private bool IsSpawnPointMarker(Entity entity) =>
            TryGetPointMarkerPrefab(entity, out Entity prefab) &&
            (EntityManager.HasComponent<Game.Objects.SpawnLocation>(entity) ||
             (EntityManager.Exists(prefab) && EntityManager.HasComponent<Game.Prefabs.SpawnLocationData>(prefab)));

        private bool MatchesCurrentMarkerType(in SpatialCandidate candidate)
        {
            if (candidate.IsAssetLane)
            {
                return IsAssetLaneMarker(candidate.Entity) || IsAssetMarkerNetwork(candidate.Entity) ||
                       IsStandaloneAssetLane(candidate.Entity);
            }

            if (candidate.IsSpawnLocation)
            {
                return IsSpawnPointMarker(candidate.Entity);
            }

            return candidate.StaticCategory != StaticObjectCategory.ActivityLocation ||
                   IsActivityLocationTarget(candidate.Entity);
        }

        private static bool IsMarkerCandidate(in SpatialCandidate candidate) =>
            candidate.IsAssetLane || candidate.IsSpawnLocation ||
            (candidate.IsStaticObject && candidate.StaticCategory == StaticObjectCategory.ActivityLocation);

        private static bool IsStaticCategoryEnabled(
            StaticObjectCategory category,
            in FilterSnapshot filters)
        {
            if (!filters.DeleteStaticObjects)
            {
                return false;
            }

            return category switch
            {
                StaticObjectCategory.GeneralProp =>
                    filters.DeleteGeneralProps,

                StaticObjectCategory.StreetLight =>
                    filters.DeleteStreetLights,

                StaticObjectCategory.QuantityObject =>
                    filters.DeleteQuantityObjects,

                StaticObjectCategory.BrandingObject =>
                    filters.DeleteBrandingObjects,

                StaticObjectCategory.ActivityLocation =>
                    filters.DeleteActivityLocations,

                _ => false
            };
        }

        private static void IncrementStaticCategoryCount(
            StaticObjectCategory category,
            ref int generalPropCount,
            ref int streetLightCount,
            ref int quantityObjectCount,
            ref int brandingObjectCount,
            ref int activityLocationCount)
        {
            switch (category)
            {
                case StaticObjectCategory.GeneralProp:
                    generalPropCount++;
                    break;

                case StaticObjectCategory.StreetLight:
                    streetLightCount++;
                    break;

                case StaticObjectCategory.QuantityObject:
                    quantityObjectCount++;
                    break;

                case StaticObjectCategory.BrandingObject:
                    brandingObjectCount++;
                    break;

                case StaticObjectCategory.ActivityLocation:
                    activityLocationCount++;
                    break;
            }
        }
    }
}
