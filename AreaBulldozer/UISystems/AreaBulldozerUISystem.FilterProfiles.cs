using AreaBulldozer.Tools;
using Colossal.UI.Binding;
using System;
using System.Collections.Generic;

namespace AreaBulldozer.UISystems
{
    public partial class AreaBulldozerUISystem
    {
        private string m_FilterProfileStorage;
        private string m_ActiveFilterProfileId = "";
        private List<FilterProfile> m_CustomFilterProfiles = new();

        private void RegisterFilterProfileBindings()
        {
            string group = AreaBulldozerUIBindingConstants.ModId;
            AddUpdateBinding(new GetterValueBinding<string>(group, "filterProfiles", GetFilterProfiles));
            AddUpdateBinding(new GetterValueBinding<string>(group, "activeFilterProfile", GetActiveFilterProfile));
            AddUpdateBinding(new GetterValueBinding<bool>(group, "filterProfileModified", IsFilterProfileModified));

            AddBinding(new TriggerBinding<string>(group, "loadFilterProfile", LoadFilterProfile));
            AddBinding(new TriggerBinding<string>(group, "createFilterProfile", CreateFilterProfile));
            AddBinding(new TriggerBinding<string>(group, "updateFilterProfile", UpdateFilterProfile));
            AddBinding(new TriggerBinding<string, string>(group, "renameFilterProfile", RenameFilterProfile));
            AddBinding(new TriggerBinding<string>(group, "deleteFilterProfile", DeleteFilterProfile));
        }

        private List<FilterProfile> CustomFilterProfiles
        {
            get
            {
                string stored = Mod.Settings?.CustomFilterProfiles ?? "";
                if (stored != m_FilterProfileStorage)
                {
                    m_CustomFilterProfiles = FilterProfile.ReadCustomProfiles(stored);
                    m_FilterProfileStorage = stored;
                }

                return m_CustomFilterProfiles;
            }
        }

        private string GetFilterProfiles()
        {
            // Return the same string while settings are unchanged.
            _ = CustomFilterProfiles;
            return m_FilterProfileStorage;
        }

        private FilterProfile FindFilterProfile(string id) =>
            Array.Find(FilterProfile.Presets, profile => profile.Id == id) ??
            CustomFilterProfiles.Find(profile => profile.Id == id);

        private string GetActiveFilterProfile() =>
            m_ActiveFilterProfileId.Length > 0 && FindFilterProfile(m_ActiveFilterProfileId) != null
                ? m_ActiveFilterProfileId
                : "";

        private bool IsFilterProfileModified() =>
            Mod.Settings is Setting setting && m_ActiveFilterProfileId.Length > 0 &&
            FindFilterProfile(m_ActiveFilterProfileId) is FilterProfile profile &&
            FilterProfile.Capture(setting) != profile.Filters;

        private void LoadFilterProfile(string id)
        {
            if (m_Tool?.IsToolActive != true || Mod.Settings == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(id))
            {
                m_ActiveFilterProfileId = "";
                return;
            }

            FilterProfile profile = FindFilterProfile(id);
            if (profile == null)
            {
                return;
            }

            Setting setting = Mod.Settings;
            bool previousCarFilter = setting.DeleteCars;
            AreaBulldozerHeightFilter previousHeightFilter = setting.HeightFilter;
            ChangeSetting(profile.Apply);
            m_ActiveFilterProfileId = id;

            if (previousCarFilter != setting.DeleteCars)
            {
                m_Tool.OnCarFilterChanged();
            }
            else
            {
                m_Tool.InvalidateSelectionGeometry();
            }

            if (previousHeightFilter != setting.HeightFilter)
            {
                m_Tool.UpdateHeightView(resetView: true);
            }
        }

        private void CreateFilterProfile(string name)
        {
            name = name?.Trim();
            if (m_Tool?.IsToolActive != true || Mod.Settings == null ||
                !IsAvailableProfileName(name))
            {
                return;
            }

            var profile = new FilterProfile(Guid.NewGuid().ToString("N"), name, FilterProfile.Capture(Mod.Settings));
            CustomFilterProfiles.Add(profile);
            m_ActiveFilterProfileId = profile.Id;
            SaveFilterProfiles();
        }

        private void UpdateFilterProfile(string id)
        {
            if (m_Tool?.IsToolActive != true || Mod.Settings == null || id != m_ActiveFilterProfileId)
            {
                return;
            }

            FilterProfile profile = CustomFilterProfiles.Find(item => item.Id == id);
            if (profile != null)
            {
                profile.Filters = FilterProfile.Capture(Mod.Settings);
                SaveFilterProfiles();
            }
        }

        private void RenameFilterProfile(string id, string name)
        {
            name = name?.Trim();
            if (m_Tool?.IsToolActive != true || Mod.Settings == null || !IsAvailableProfileName(name, id))
            {
                return;
            }

            FilterProfile profile = CustomFilterProfiles.Find(item => item.Id == id);
            if (profile != null)
            {
                profile.Name = name;
                SaveFilterProfiles();
            }
        }

        private void DeleteFilterProfile(string id)
        {
            if (m_Tool?.IsToolActive != true || Mod.Settings == null ||
                CustomFilterProfiles.RemoveAll(item => item.Id == id) == 0)
            {
                return;
            }

            if (id == m_ActiveFilterProfileId)
            {
                m_ActiveFilterProfileId = "";
            }

            SaveFilterProfiles();
        }

        private bool IsAvailableProfileName(string name, string exceptId = null) =>
            FilterProfile.IsValidName(name) &&
            !CustomFilterProfiles.Exists(profile =>
                profile.Id != exceptId && string.Equals(profile.Name, name, StringComparison.OrdinalIgnoreCase));

        private void SaveFilterProfiles()
        {
            string stored = string.Join("\n", m_CustomFilterProfiles.ConvertAll(profile => profile.Serialize()));
            ChangeSetting(setting => setting.CustomFilterProfiles = stored);
            m_FilterProfileStorage = stored;
        }
    }
}
