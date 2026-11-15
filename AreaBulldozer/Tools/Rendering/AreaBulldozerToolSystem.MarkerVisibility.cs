namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private const int kMissingAssetFocus = 1 << 0;
        private const int kActivityFocus = 1 << 1;
        private const int kSpawnFocus = 1 << 2;
        private const int kLaneFocus = 1 << 3;

        private int m_ActiveMarkerFocusFilters;

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
            int filters = settings?.DeleteMissingAssets == true ? kMissingAssetFocus : 0;
            if (settings?.DeleteStaticObjects == true)
            {
                filters |= settings.DeleteActivityLocations ? kActivityFocus : 0;
                filters |= settings.DeleteSpawnLocations ? kSpawnFocus : 0;
                filters |= settings.DeleteMarkerNetworks ? kLaneFocus : 0;
            }

            if ((filters & ~m_ActiveMarkerFocusFilters) != 0)
            {
                settings.DimMarkerBackground = true;
            }

            m_ActiveMarkerFocusFilters = filters;
            UpdateNativeMarkerPreview((filters & (kActivityFocus | kSpawnFocus | kLaneFocus)) != 0);
            UpdateMarkerFocusOverlay(filters != 0);
        }

        private void RestoreMarkerVisibility()
        {
            m_ActiveMarkerFocusFilters = 0;
            RestoreNativeMarkerPreview();
            RemoveMarkerFocusOverlay();
        }
    }
}
