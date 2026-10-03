using Game.Prefabs;
using Unity.Entities;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // Check actual missing-prefab state, never mesh colour or asset names.
        // A disabled prefab alone is not sufficient: valid assets can be disabled.
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

            Entity prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
            if (prefab == Entity.Null || !EntityManager.Exists(prefab))
            {
                return true;
            }

            // A live target without prefab metadata is ambiguous; leave it alone.
            if (!EntityManager.HasComponent<PrefabData>(prefab) ||
                EntityManager.IsComponentEnabled<PrefabData>(prefab) ||
                EntityManager.GetComponentData<PrefabData>(prefab).m_Index >= 0)
            {
                return false;
            }

            return !m_PrefabSystem.TryGetPrefab(prefab, out PrefabBase _);
        }
    }
}
