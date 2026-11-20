using AreaBulldozer.Tools;
using Colossal.UI.Binding;

namespace AreaBulldozer.UISystems
{
    public partial class AreaBulldozerUISystem
    {
        private string m_AssetProfileStorage;
        private bool m_AssetProfileEnabled;
        private string m_AssetProfileSnapshot;

        private string GetAssetProfileSelection(Setting setting)
        {
            string stored = setting.AssetFilterPrefabs ?? "";
            if (m_AssetProfileSnapshot == null || m_AssetProfileStorage != stored ||
                m_AssetProfileEnabled != setting.AssetFilterEnabled)
            {
                m_AssetProfileStorage = stored;
                m_AssetProfileEnabled = setting.AssetFilterEnabled;
                m_AssetProfileSnapshot = AssetFilterSettings.Capture(setting);
            }
            return m_AssetProfileSnapshot;
        }

        private void RegisterAssetFilterBindings()
        {
            string group = AreaBulldozerUIBindingConstants.ModId;
            AddUpdateBinding(new GetterValueBinding<string>(group, "assetFilterItems", () => m_Tool?.GetAssetFilterItems() ?? "[]"));
            AddUpdateBinding(new GetterValueBinding<bool>(group, "assetPickerActive", () => m_Tool?.AssetPickerActive ?? false));
            AddUpdateBinding(new GetterValueBinding<bool>(group, "assetFilterEnabled", () => Mod.Settings?.AssetFilterEnabled ?? false));
            AddUpdateBinding(new GetterValueBinding<int>(group, "assetSelectionUndoCount", () => m_Tool?.AssetSelectionUndoCount ?? 0));
            AddBinding(new TriggerBinding(group, "undoAssetSelection", () => m_Tool?.UndoAssetSelection()));
            AddBinding(new TriggerBinding<bool>(group, "setAssetPicker", active => m_Tool?.SetAssetPicker(active)));
            AddBinding(new TriggerBinding<bool>(group, "setAssetFilterEnabled", enabled =>
            {
                if (Mod.Settings == null) return;
                var before = new AssetSelectionState(Mod.Settings);
                ChangeSetting(setting => setting.AssetFilterEnabled = enabled);
                m_Tool?.RecordAssetSelectionUndo(before);
                m_Tool?.NotifyAssetFilterChanged();
            }));
            AddBinding(new TriggerBinding<string>(group, "removeAssetFilterItem", id =>
            {
                if (Mod.Settings == null) return;
                var items = AssetFilterSettings.Read(Mod.Settings.AssetFilterPrefabs);
                if (!items.Remove(id)) return;
                var before = new AssetSelectionState(Mod.Settings);
                ChangeSetting(setting => setting.AssetFilterPrefabs = string.Join("\n", items));
                m_Tool?.RecordAssetSelectionUndo(before);
                m_Tool?.NotifyAssetFilterChanged();
            }));
            AddBinding(new TriggerBinding(group, "clearAssetFilter", () =>
            {
                if (Mod.Settings == null) return;
                var before = new AssetSelectionState(Mod.Settings);
                ChangeSetting(setting => setting.AssetFilterPrefabs = "");
                m_Tool?.RecordAssetSelectionUndo(before);
                m_Tool?.NotifyAssetFilterChanged();
            }));
        }

        private void OnAssetFilterChanged()
        {
            // Picking happens in the tool update; use the existing delayed save.
            ChangeSetting(_ => { });
        }
    }
}
