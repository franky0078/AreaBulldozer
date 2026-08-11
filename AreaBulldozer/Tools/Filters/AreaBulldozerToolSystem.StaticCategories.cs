using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {

        private StaticObjectCategory GetStaticObjectCategory(
            Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return StaticObjectCategory.None;
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

            if (EntityManager.HasComponent<
                    Game.Objects.ActivityLocation>(
                    entity))
            {
                return StaticObjectCategory.ActivityLocation;
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
