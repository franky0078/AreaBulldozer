namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private readonly AssetSelectionUndoHistory m_AssetSelectionUndo = new();

        internal int AssetSelectionUndoCount => m_AssetSelectionUndo.Count(Mod.Settings);

        internal void RecordAssetSelectionUndo(AssetSelectionState before) =>
            m_AssetSelectionUndo.Record(before, Mod.Settings);

        internal void ClearAssetSelectionUndo() => m_AssetSelectionUndo.Clear();

        internal void UndoAssetSelection()
        {
            if (!IsToolActive || !m_AssetSelectionUndo.TryUndo(Mod.Settings)) return;
            // Consume the click before returning to the brush. This does not
            // touch the snapshots of objects previously deleted in the world.
            SetAssetPicker(false);
            NotifyAssetFilterChanged();
            AssetFilterChanged?.Invoke();
        }
    }
}
