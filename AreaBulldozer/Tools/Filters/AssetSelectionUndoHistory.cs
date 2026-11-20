using System;
using System.Collections.Generic;

namespace AreaBulldozer.Tools
{
    // Only the working selection is restored. Brush options, category filters,
    // saved profiles and the separate deletion undo are deliberately independent.
    internal readonly struct AssetSelectionState
    {
        internal readonly bool Enabled;
        internal readonly string Prefabs;

        internal AssetSelectionState(Setting setting)
        {
            Enabled = setting.AssetFilterEnabled;
            Prefabs = setting.AssetFilterPrefabs ?? "";
        }

        internal bool Matches(Setting setting) =>
            Enabled == setting.AssetFilterEnabled &&
            string.Equals(Prefabs, setting.AssetFilterPrefabs ?? "", StringComparison.Ordinal);

        internal bool Matches(AssetSelectionState other) =>
            Enabled == other.Enabled && string.Equals(Prefabs, other.Prefabs, StringComparison.Ordinal);

        internal void Apply(Setting setting)
        {
            setting.AssetFilterEnabled = Enabled;
            setting.AssetFilterPrefabs = Prefabs;
        }
    }

    internal sealed class AssetSelectionUndoHistory
    {
        internal const int MaximumSteps = 32;
        private readonly List<(AssetSelectionState Before, AssetSelectionState After)> m_Changes = new();

        internal void Clear() => m_Changes.Clear();

        internal int Count(Setting setting)
        {
            // A profile load or settings reset must not resurrect an unrelated list.
            if (setting == null || (m_Changes.Count > 0 && !m_Changes[m_Changes.Count - 1].After.Matches(setting)))
            {
                Clear();
            }
            return m_Changes.Count;
        }

        internal void Record(AssetSelectionState before, Setting setting)
        {
            if (setting == null || before.Matches(setting)) return;
            if (m_Changes.Count > 0 && !m_Changes[m_Changes.Count - 1].After.Matches(before)) Clear();
            if (m_Changes.Count == MaximumSteps) m_Changes.RemoveAt(0);
            m_Changes.Add((before, new AssetSelectionState(setting)));
        }

        internal bool TryUndo(Setting setting)
        {
            if (Count(setting) == 0) return false;
            int last = m_Changes.Count - 1;
            AssetSelectionState previous = m_Changes[last].Before;
            m_Changes.RemoveAt(last);
            previous.Apply(setting);
            return true;
        }
    }
}
