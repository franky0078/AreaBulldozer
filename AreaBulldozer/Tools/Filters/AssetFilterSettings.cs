using System;
using System.Collections.Generic;
using System.Text;

namespace AreaBulldozer.Tools
{
    // Store full PrefabIDs, including custom-asset hashes, rather than world-specific entities.
    internal static class AssetFilterSettings
    {
        internal const int MaximumAssets = 128;

        internal static List<string> Read(string stored)
        {
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (string id in (stored ?? "").Split('\n'))
            {
                string[] parts = id.Split('/');
                if (result.Count >= MaximumAssets) break;
                if ((parts.Length != 2 && parts.Length != 3) || id.Length > 4096 ||
                    string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1])) continue;
                bool valid = true;
                foreach (char c in id) valid &= !char.IsControl(c);
                if (valid && seen.Add(id)) result.Add(id);
            }
            return result;
        }

        internal static string Capture(Setting setting) => Convert.ToBase64String(Encoding.UTF8.GetBytes(
            (setting.AssetFilterEnabled ? "1\n" : "0\n") + string.Join("\n", Read(setting.AssetFilterPrefabs))));

        internal static bool TryApply(string encoded, Setting setting)
        {
            if (string.IsNullOrEmpty(encoded))
            {
                setting.AssetFilterEnabled = false;
                setting.AssetFilterPrefabs = "";
                return true;
            }
            if (encoded.Length > 750000) return false;
            try
            {
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                if (!decoded.StartsWith("0\n", StringComparison.Ordinal) &&
                    !decoded.StartsWith("1\n", StringComparison.Ordinal)) return false;
                setting.AssetFilterEnabled = decoded[0] == '1';
                setting.AssetFilterPrefabs = string.Join("\n", Read(decoded.Substring(2)));
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
