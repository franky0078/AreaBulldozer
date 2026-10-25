using Colossal.Mathematics;
using Game.Net;
using Game.Simulation;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // Slightly sunken props and normal surface networks still count as on-ground.
        private const float kBelowGroundTolerance = 0.5f;
        private TerrainSystem m_HeightTerrainSystem;
        private TerrainHeightData m_HeightTerrainData;
        private bool m_HeightTerrainReady;
        private readonly Dictionary<Entity, int> m_HeightClassifications = new();
        private AreaBulldozerHeightFilter m_ActiveHeightFilter;
        private AreaBulldozerHeightFilter m_LastHeightViewFilter;

        public bool UndergroundView { get; private set; }
        // Hide the vanilla underground row; our own controls set requireUnderground.
        public override bool allowUnderground => false;

        public override void SetUnderground(bool isUnderground)
        {
            UndergroundView = isUnderground;
            requireUnderground = IsToolActive && isUnderground;
        }

        public override void ElevationUp() => SetUnderground(false);
        public override void ElevationDown() => SetUnderground(true);
        public override void ElevationScroll() => SetUnderground(!UndergroundView);

        public void UpdateHeightView(bool resetView = false)
        {
            AreaBulldozerHeightFilter filter = Mod.Settings?.HeightFilter ??
                AreaBulldozerHeightFilter.All;
            if (resetView || filter != m_LastHeightViewFilter)
            {
                m_LastHeightViewFilter = filter;
                UndergroundView = filter == AreaBulldozerHeightFilter.BelowGround;
            }
            requireUnderground = IsToolActive && UndergroundView;
        }

        private void PrepareHeightFilter()
        {
            m_HeightClassifications.Clear();
            m_HeightTerrainReady = false;
            m_ActiveHeightFilter = Mod.Settings?.HeightFilter ?? AreaBulldozerHeightFilter.All;
            if (m_ActiveHeightFilter == AreaBulldozerHeightFilter.All)
                return;

            m_HeightTerrainSystem ??= World.GetOrCreateSystemManaged<TerrainSystem>();
            // GetHeightData supplies the CPU terrain data for synchronous sampling.
            m_HeightTerrainData = m_HeightTerrainSystem.GetHeightData();
            m_HeightTerrainReady = m_HeightTerrainData.heights.IsCreated &&
                m_HeightTerrainData.heights.Length > 0;
        }

        private bool MatchesHeightFilter(Entity entity)
        {
            if (m_ActiveHeightFilter == AreaBulldozerHeightFilter.All)
                return true;
            if (!m_HeightTerrainReady || entity == Entity.Null || !EntityManager.Exists(entity))
                return false;

            if (!m_HeightClassifications.TryGetValue(entity, out int classification))
            {
                classification = ClassifyEntityHeight(entity);
                m_HeightClassifications.Add(entity, classification);
            }
            // Mixed segments and unknown positions are excluded in both restricted modes.
            return classification == (m_ActiveHeightFilter == AreaBulldozerHeightFilter.AboveGround ? 1 : 2);
        }

        private int ClassifyHeight(float3 position)
        {
            if (!math.all(math.isfinite(position)))
                return 0;
            float height = TerrainUtils.SampleHeight(ref m_HeightTerrainData, position);
            if (!math.isfinite(height))
                return 0;
            return position.y < height - kBelowGroundTolerance ? 2 : 1;
        }

        private int ClassifyEntityHeight(Entity entity)
        {
            if (EntityManager.HasComponent<Curve>(entity))
            {
                Curve curve = EntityManager.GetComponentData<Curve>(entity);
                if (!math.isfinite(curve.m_Length))
                    return 0;
                int samples = math.clamp((int)math.ceil(curve.m_Length / 4f), 8, 512);
                int result = 0;
                for (int i = 0; i <= samples; i++)
                {
                    int sample = ClassifyHeight(MathUtils.Position(curve.m_Bezier, (float)i / samples));
                    if (sample == 0)
                        return 0;
                    result |= sample;
                    if (result == 3)
                        break;
                }
                return result;
            }

            if (TryGetAreaNodes(entity, out DynamicBuffer<Game.Areas.Node> nodes))
            {
                int result = 0;
                for (int i = 0; i < nodes.Length; i++)
                {
                    float3 start = nodes[i].m_Position;
                    float3 end = nodes[(i + 1) % nodes.Length].m_Position;
                    if (!math.all(math.isfinite(start)) || !math.all(math.isfinite(end)))
                        return 0;
                    int samples = math.clamp((int)math.ceil(math.distance(start, end) / 4f), 1, 512);
                    for (int j = 0; j < samples; j++)
                    {
                        int sample = ClassifyHeight(math.lerp(start, end, (float)j / samples));
                        if (sample == 0)
                            return 0;
                        result |= sample;
                        if (result == 3)
                            return result;
                    }
                }
                return result;
            }

            if (EntityManager.HasComponent<Game.Objects.Transform>(entity))
                return ClassifyHeight(EntityManager.GetComponentData<Game.Objects.Transform>(entity).m_Position);
            if (EntityManager.HasComponent<Game.Net.Node>(entity))
                return ClassifyHeight(EntityManager.GetComponentData<Game.Net.Node>(entity).m_Position);
            return 0;
        }
    }
}
