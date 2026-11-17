namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private const int kMissingAssetFocus = 1 << 0;
        private const int kActivityFocus = 1 << 1;
        private const int kSpawnFocus = 1 << 2;
        private const int kLaneFocus = 1 << 3;

        private int m_ActiveMarkerFocusFilters;
        private bool m_ManualPropsDimming;

        internal bool IsMarkerBackgroundDimmed =>
            IsToolActive && m_MarkerFocusOverlayVisible && m_AppliedMarkerBackgroundDarkness > 0;

        private void InitializeMarkerVisibility()
        {
            m_ActiveMarkerFocusFilters = 0;
            InitializeNativeMarkerPreview();
            InitializeMarkerFocusOverlay();
        }

        private void DisposeMarkerVisibility()
        {
            m_ActiveMarkerFocusFilters = 0;
            DisposeNativeMarkerPreview();
            DisposeMarkerFocusOverlay();
        }

        private void UpdateMarkerVisibility()
        {
            Setting settings = Mod.Settings;
            int filters = GetMarkerFocusFilters(settings);

            // A newly enabled focus filter starts dimmed. Manual dimming for
            // ordinary props does not carry over into or out of a focus mode.
            if ((filters & ~m_ActiveMarkerFocusFilters) != 0)
            {
                settings.DimMarkerBackground = true;
            }

            if (filters != m_ActiveMarkerFocusFilters || settings?.DeleteStaticObjects != true)
            {
                m_ManualPropsDimming = false;
            }

            m_ActiveMarkerFocusFilters = filters;
            bool dimBackground = filters != 0 ? settings.DimMarkerBackground : m_ManualPropsDimming;
            UpdateNativeMarkerPreview((filters & (kActivityFocus | kSpawnFocus | kLaneFocus)) != 0);
            UpdateMarkerFocusOverlay(dimBackground, (filters & kMissingAssetFocus) != 0);
        }

        internal void SetManualPropsDimming(bool dim)
        {
            if (IsToolActive && Mod.Settings is Setting settings)
            {
                m_ManualPropsDimming = dim && settings.DeleteStaticObjects &&
                    GetMarkerFocusFilters(settings) == 0;
            }
        }

        private static int GetMarkerFocusFilters(Setting settings)
        {
            int filters = settings?.DeleteMissingAssets == true ? kMissingAssetFocus : 0;
            if (settings?.DeleteStaticObjects == true)
            {
                filters |= settings.DeleteActivityLocations ? kActivityFocus : 0;
                filters |= settings.DeleteSpawnLocations ? kSpawnFocus : 0;
                filters |= settings.DeleteMarkerNetworks ? kLaneFocus : 0;
            }

            return filters;
        }

        private void RestoreMarkerVisibility()
        {
            m_ActiveMarkerFocusFilters = 0;
            m_ManualPropsDimming = false;
            RestoreNativeMarkerPreview();
            RemoveMarkerFocusOverlay();
        }
    }
}
