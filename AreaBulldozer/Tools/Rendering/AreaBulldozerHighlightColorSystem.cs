using Game;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerHighlightColorSystem : GameSystemBase
    {
        private EntityQuery m_RenderSettingsQuery;
        private Color m_PreviousHoveredColor;
        private Color m_LastAppliedColor;
        private bool m_HasPreviousColor;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_RenderSettingsQuery =
                GetEntityQuery(
                    ComponentType.ReadWrite<RenderingSettingsData>());
        }

        protected override void OnUpdate()
        {
            AreaBulldozerToolSystem tool =
                AreaBulldozerToolSystem.Instance;

            bool toolActive =
                tool != null &&
                tool.IsToolActive;

            if (!toolActive)
            {
                RestorePreviousColor();
                return;
            }

            if (m_RenderSettingsQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            Entity entity =
                m_RenderSettingsQuery.GetSingletonEntity();

            RenderingSettingsData data =
                EntityManager.GetComponentData<RenderingSettingsData>(
                    entity);

            if (!m_HasPreviousColor)
            {
                m_PreviousHoveredColor =
                    data.m_HoveredColor;

                m_HasPreviousColor = true;
            }

            Setting settings = Mod.Settings;

            Color desiredColor =
                new(
                    math.clamp(
                        settings?.SurfaceColorRed ?? 20,
                        0,
                        255) / 255f,
                    math.clamp(
                        settings?.SurfaceColorGreen ?? 219,
                        0,
                        255) / 255f,
                    math.clamp(
                        settings?.SurfaceColorBlue ?? 255,
                        0,
                        255) / 255f,
                    m_PreviousHoveredColor.a);

            if (ColorsEqual(
                    data.m_HoveredColor,
                    desiredColor))
            {
                m_LastAppliedColor = desiredColor;
                return;
            }

            data.m_HoveredColor = desiredColor;

            EntityManager.SetComponentData(
                entity,
                data);

            m_LastAppliedColor = desiredColor;
        }

        protected override void OnDestroy()
        {
            RestorePreviousColor();
            base.OnDestroy();
        }

        private void RestorePreviousColor()
        {
            if (!m_HasPreviousColor)
            {
                return;
            }

            if (!m_RenderSettingsQuery.IsEmptyIgnoreFilter)
            {
                Entity entity =
                    m_RenderSettingsQuery.GetSingletonEntity();

                RenderingSettingsData data =
                    EntityManager.GetComponentData<RenderingSettingsData>(
                        entity);

                if (ColorsEqual(
                        data.m_HoveredColor,
                        m_LastAppliedColor))
                {
                    data.m_HoveredColor =
                        m_PreviousHoveredColor;

                    EntityManager.SetComponentData(
                        entity,
                        data);
                }
            }

            m_HasPreviousColor = false;
        }

        private static bool ColorsEqual(
            Color left,
            Color right)
        {
            const float tolerance = 0.0001f;

            return
                math.abs(left.r - right.r) <= tolerance &&
                math.abs(left.g - right.g) <= tolerance &&
                math.abs(left.b - right.b) <= tolerance &&
                math.abs(left.a - right.a) <= tolerance;
        }
    }
}
