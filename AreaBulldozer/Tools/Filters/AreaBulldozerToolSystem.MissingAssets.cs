using Game.Prefabs;
using Unity.Entities;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private bool IsMissingAssetObject(Entity entity)
        {
            if (entity == Entity.Null || !EntityManager.Exists(entity) ||
                !EntityManager.HasComponent<PrefabRef>(entity) ||
                EntityManager.HasComponent<PrefabData>(entity) ||
                EntityManager.HasComponent<Game.Buildings.Building>(entity) ||
                EntityManager.HasComponent<Game.Net.Edge>(entity) ||
                EntityManager.HasComponent<Game.Net.Node>(entity) ||
                EntityManager.HasComponent<Game.Effects.EffectInstance>(entity) ||
                EntityManager.HasComponent<Game.Routes.LivePath>(entity))
            {
                return false;
            }

            return IsMissingPrefab(
                EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab);
        }

        private bool IsMissingPrefab(Entity prefab)
        {
            return prefab == Entity.Null ||
                   !EntityManager.Exists(prefab) ||
                   HasPrefabTrait(prefab, PrefabTraits.Missing);
        }
    }
}
