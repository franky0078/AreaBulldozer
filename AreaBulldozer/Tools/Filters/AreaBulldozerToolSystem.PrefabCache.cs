using Game.Prefabs;
using System;
using System.Collections.Generic;
using Unity.Entities;

namespace AreaBulldozer.Tools
{
    // Cache for classifications that depend only on the prefab.
    //
    // The index (re)build classifies every object. Most of that work only looks
    // at the prefab (PrefabBase type, geometry flags, prefab components, prefab
    // name), but used to be repeated for each of the thousands of instances.
    // The result per prefab entity is computed once and reused. Checks that
    // depend on the instance itself (Marker, SpawnLocation, Building, ...) stay
    // in the callers.
    public partial class AreaBulldozerToolSystem
    {
        [Flags]
        private enum PrefabTraits : ushort
        {
            None = 0,
            StaticObject = 1 << 0,
            PointMarker = 1 << 1,
            ActivityLocation = 1 << 2,
            SpawnLocation = 1 << 3,
            Brand = 1 << 4,
            Missing = 1 << 5,
            PedestrianPath = 1 << 6,
            RailwayTrack = 1 << 7,
            HasPrefabBase = 1 << 8
        }

        private readonly Dictionary<Entity, PrefabTraits> m_PrefabTraitsCache = new();

        // Cleared on every full index rebuild and when a city is loaded, so
        // prefabs changed at runtime (e.g. by other mods) are picked up again.
        private void ClearPrefabTraitsCache()
        {
            m_PrefabTraitsCache.Clear();
        }

        // Only call with an existing prefab entity.
        private bool HasPrefabTrait(Entity prefab, PrefabTraits trait)
        {
            if (!m_PrefabTraitsCache.TryGetValue(prefab, out PrefabTraits traits))
            {
                traits = ComputePrefabTraits(prefab);
                m_PrefabTraitsCache.Add(prefab, traits);
            }

            return (traits & trait) != 0;
        }

        private bool TryGetExistingPrefab(Entity entity, out Entity prefab)
        {
            prefab = Entity.Null;
            if (!EntityManager.HasComponent<PrefabRef>(entity))
            {
                return false;
            }

            prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
            return prefab != Entity.Null && EntityManager.Exists(prefab);
        }

        private PrefabTraits ComputePrefabTraits(Entity prefab)
        {
            PrefabTraits traits = PrefabTraits.None;

            bool prefabFound =
                m_PrefabSystem.TryGetPrefab(prefab, out PrefabBase prefabBase);
            bool hasPrefabBase = prefabFound && prefabBase != null;

            if (hasPrefabBase)
            {
                traits |= PrefabTraits.HasPrefabBase;

                if (prefabBase is StaticObjectPrefab && !(prefabBase is BuildingPrefab))
                {
                    traits |= PrefabTraits.StaticObject;
                }

                if (IsPedestrianPathPrefab(prefabBase))
                {
                    traits |= PrefabTraits.PedestrianPath;
                }

                if (IsRailwayTrackPrefab(prefabBase))
                {
                    traits |= PrefabTraits.RailwayTrack;
                }
            }

            if (IsPointMarkerPrefab(prefab))
            {
                traits |= PrefabTraits.PointMarker;
            }

            if (EntityManager.HasComponent<Game.Prefabs.ActivityLocationData>(prefab) ||
                (EntityManager.HasBuffer<Game.Prefabs.ActivityLocationElement>(prefab) &&
                 EntityManager.GetBuffer<Game.Prefabs.ActivityLocationElement>(prefab, true).Length != 0))
            {
                traits |= PrefabTraits.ActivityLocation;
            }

            if (EntityManager.HasComponent<Game.Prefabs.SpawnLocationData>(prefab))
            {
                traits |= PrefabTraits.SpawnLocation;
            }

            if (EntityManager.HasComponent<BrandObjectData>(prefab))
            {
                traits |= PrefabTraits.Brand;
            }

            // Missing prefab: placeholder with disabled PrefabData, a negative
            // index and no PrefabBase. A disabled prefab alone is not enough.
            if (EntityManager.HasComponent<PrefabData>(prefab) &&
                !EntityManager.IsComponentEnabled<PrefabData>(prefab) &&
                EntityManager.GetComponentData<PrefabData>(prefab).m_Index < 0 &&
                !prefabFound)
            {
                traits |= PrefabTraits.Missing;
            }

            return traits;
        }

        // Prefab part of TryGetPointMarkerPrefab (unchanged rules).
        private bool IsPointMarkerPrefab(Entity prefab)
        {
            if (EntityManager.HasComponent<ObjectGeometryData>(prefab))
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
            return EntityManager.HasComponent<Game.Prefabs.SpawnLocationData>(prefab) &&
                   (!EntityManager.HasBuffer<Game.Prefabs.SubMesh>(prefab) ||
                    EntityManager.GetBuffer<Game.Prefabs.SubMesh>(prefab, true).Length == 0);
        }
    }
}
