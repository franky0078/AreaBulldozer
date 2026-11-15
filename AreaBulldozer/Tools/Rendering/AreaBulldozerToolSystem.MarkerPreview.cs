using Game.Rendering;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private RenderingSystem m_RenderingSystem;
        private bool m_ControlsNativeMarkerPreview;
        private bool m_PreviousNativeMarkerVisibility;

        private void InitializeNativeMarkerPreview()
        {
            m_RenderingSystem = World.GetOrCreateSystemManaged<RenderingSystem>();
        }

        private void DisposeNativeMarkerPreview()
        {
            RestoreNativeMarkerPreview();
            m_RenderingSystem = null;
        }

        private void UpdateNativeMarkerPreview(bool showMarkers)
        {
            if (m_RenderingSystem == null)
            {
                return;
            }

            if (!showMarkers)
            {
                RestoreNativeMarkerPreview();
                return;
            }

            if (!m_ControlsNativeMarkerPreview)
            {
                m_PreviousNativeMarkerVisibility = m_RenderingSystem.markersVisible;
                m_ControlsNativeMarkerPreview = true;
            }

            // The game renders its existing marker meshes and materials together.
            // The individual tool filters are applied to the selection separately.
            m_RenderingSystem.markersVisible = true;
        }

        private void RestoreNativeMarkerPreview()
        {
            if (!m_ControlsNativeMarkerPreview)
            {
                return;
            }

            if (m_RenderingSystem != null)
            {
                m_RenderingSystem.markersVisible = m_PreviousNativeMarkerVisibility;
            }

            m_ControlsNativeMarkerPreview = false;
        }
    }
}
