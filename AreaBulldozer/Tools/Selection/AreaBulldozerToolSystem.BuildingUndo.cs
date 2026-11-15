using Game.Common;
using Game.Objects;
using Game.Prefabs;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Building = Game.Buildings.Building;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private struct BuildingSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public Transform Transform;
            public PseudoRandomSeed? Seed;
            public uint OptionMask;
            public Entity OriginalRoad;
            public BuildingLotAppearance LotAppearance;
        }

        private readonly List<BuildingSnapshot> m_BuildingUndo = new();

        private void RecordBuildingForUndo(Entity entity)
        {
            if (!EntityManager.HasComponent<Building>(entity) ||
                EntityManager.HasComponent<Owner>(entity) ||
                !EntityManager.HasComponent<Transform>(entity) ||
                !EntityManager.HasComponent<PrefabRef>(entity))
            {
                return;
            }

            Entity prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
            if (!IsEntityUsable(prefab) || !EntityManager.HasComponent<BuildingData>(prefab) ||
                !EntityManager.HasComponent<ObjectData>(prefab))
            {
                return;
            }

            Building data = EntityManager.GetComponentData<Building>(entity);
            m_BuildingUndo.Add(new BuildingSnapshot
            {
                Original = entity,
                Prefab = prefab,
                Transform = EntityManager.GetComponentData<Transform>(entity),
                Seed = ReadUndoComponent<PseudoRandomSeed>(entity),
                OptionMask = data.m_OptionMask,
                OriginalRoad = data.m_RoadEdge,
                LotAppearance = BuildingLotAppearance.Capture(EntityManager, entity)
            });
        }

        private HashSet<float3> GetOccupiedBuildingPositions()
        {
            var positions = new HashSet<float3>();
            using NativeArray<Transform> buildings = m_BuildingQuery.ToComponentDataArray<Transform>(Allocator.Temp);
            foreach (Transform building in buildings)
            {
                positions.Add(building.m_Position);
            }

            return positions;
        }

        private bool RestoreBuilding(BuildingSnapshot snapshot)
        {
            if (IsEntityUsable(snapshot.Original, allowOverridden: true) ||
                !IsEntityUsable(snapshot.Prefab) ||
                !EntityManager.HasComponent<BuildingData>(snapshot.Prefab) ||
                !EntityManager.HasComponent<ObjectData>(snapshot.Prefab))
            {
                return false;
            }

            EntityArchetype archetype =
                EntityManager.GetComponentData<ObjectData>(snapshot.Prefab).m_Archetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return false;
            }

            using NativeArray<Entity> result = EntityManager.CreateEntity(archetype, 1, Allocator.Temp);
            Entity building = result[0];
            if (!EntityManager.HasComponent<Building>(building) ||
                !EntityManager.HasComponent<Transform>(building) ||
                !EntityManager.HasComponent<PrefabRef>(building))
            {
                EntityManager.DestroyEntity(building);
                return false;
            }

            try
            {
                EntityManager.SetComponentData(building, new PrefabRef(snapshot.Prefab));
                EntityManager.SetComponentData(building, snapshot.Transform);
                if (snapshot.Seed is PseudoRandomSeed seed &&
                    EntityManager.HasComponent<PseudoRandomSeed>(building))
                {
                    EntityManager.SetComponentData(building, seed);
                }

                Building data = EntityManager.GetComponentData<Building>(building);
                data.m_OptionMask = snapshot.OptionMask;
                EntityManager.SetComponentData(building, data);
                snapshot.LotAppearance.RestoreFoundation(EntityManager, building);

                // Complete through the game's construction system so it generates the lot and
                // default attachments and reconnects the building without reusing stale entity IDs.
                SetUndoComponent(building, new Game.Objects.UnderConstruction
                {
                    m_NewPrefab = snapshot.Prefab,
                    m_Progress = 250,
                    m_Speed = 200
                });
                EntityManager.AddComponent<Updated>(building);
                EntityManager.AddComponent<BatchesUpdated>(building);
                World.GetOrCreateSystemManaged<AreaBulldozerBuildingConnectionSystem>().Track(building, snapshot.OriginalRoad, snapshot.LotAppearance);
                return true;
            }
            catch
            {
                EntityManager.DestroyEntity(building);
                throw;
            }
        }
    }
}
