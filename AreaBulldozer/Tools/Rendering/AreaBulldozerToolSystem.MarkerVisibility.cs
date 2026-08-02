using Game.Rendering;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // ------------------------------------------------------------
        // Sichtbarkeit unsichtbarer Marker
        // ------------------------------------------------------------

        private RenderingSystem m_RenderingSystem;

        private bool m_ControlsMarkerVisibility;
        private bool m_RecordedMarkersVisible;

        private void InitializeMarkerVisibility()
        {
            m_RenderingSystem =
                World.GetOrCreateSystemManaged<
                    RenderingSystem>();

            m_ControlsMarkerVisibility = false;
            m_RecordedMarkersVisible = false;

            InitializeMarkerFocusOverlay();

            Mod.Log.Info(
                "Marker visibility support initialized.");
        }

        private void DisposeMarkerVisibility()
        {
            RestoreMarkerVisibility();
            DisposeMarkerFocusOverlay();

            m_RenderingSystem = null;
        }

        private void UpdateMarkerVisibility()
        {
            bool markerFilterActive =
                Mod.Settings != null &&
                Mod.Settings.DeleteStaticObjects &&
                (Mod.Settings.DeleteActivityLocations ||
                 Mod.Settings.DeleteSpawnLocations ||
                 Mod.Settings.DeleteMarkerNetworks);

            UpdateMarkerFocusOverlay(
                markerFilterActive);

            if (m_RenderingSystem == null)
            {
                return;
            }

            if (!markerFilterActive)
            {
                RestoreMarkerVisibility();
                return;
            }

            if (!m_ControlsMarkerVisibility)
            {
                m_RecordedMarkersVisible =
                    m_RenderingSystem.markersVisible;

                m_ControlsMarkerVisibility = true;

                Mod.Log.Info(
                    "Game marker view is now visible for " +
                    "activity-location, spawn-location, and " +
                    "asset-lane filters. " +
                    "The view may also display parking markers " +
                    "and other marker types. " +
                    $"Previous marker visibility: " +
                    $"{m_RecordedMarkersVisible}.");
            }

            if (!m_RenderingSystem.markersVisible)
            {
                m_RenderingSystem.markersVisible = true;
            }
        }

        private void RestoreMarkerVisibility()
        {
            RemoveMarkerFocusOverlay();

            if (!m_ControlsMarkerVisibility)
            {
                return;
            }

            if (m_RenderingSystem != null)
            {
                m_RenderingSystem.markersVisible =
                    m_RecordedMarkersVisible;
            }

            Mod.Log.Info(
                "Marker visibility restored to " +
                $"{m_RecordedMarkersVisible}.");

            m_ControlsMarkerVisibility = false;
        }
    }
}
