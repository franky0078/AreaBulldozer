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

            if (TryGetExistingPrefab(entity, out Entity prefab) &&
                HasPrefabTrait(prefab, PrefabTraits.Brand))
            {
                return StaticObjectCategory.BrandingObject;
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

            // Geometry and spawn-location rules depend only on the prefab (cached).
            return prefab != Entity.Null &&
                   EntityManager.Exists(prefab) &&
                   HasPrefabTrait(prefab, PrefabTraits.PointMarker);
        }

        private bool IsActivityLocationTarget(Entity entity)
        {
            bool hasActivity = EntityManager.HasComponent<Game.Objects.ActivityLocation>(entity);
            if (!hasActivity && TryGetExistingPrefab(entity, out Entity prefab))
            {
                hasActivity = HasPrefabTrait(prefab, PrefabTraits.ActivityLocation);
            }

            return hasActivity && (IsStaticObjectEntity(entity) || TryGetPointMarkerPrefab(entity, out _));
        }

        private bool IsSpawnPointMarker(Entity entity) =>
            TryGetPointMarkerPrefab(entity, out Entity prefab) &&
            (EntityManager.HasComponent<Game.Objects.SpawnLocation>(entity) ||
             (prefab != Entity.Null && EntityManager.Exists(prefab) &&
              HasPrefabTrait(prefab, PrefabTraits.SpawnLocation)));

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
