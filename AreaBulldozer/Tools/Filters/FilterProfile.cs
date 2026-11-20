using System;
using System.Collections.Generic;
using System.Globalization;

namespace AreaBulldozer.Tools
{
    internal sealed class FilterProfile
    {
        internal const int MaxNameLength = 40;

        private const int HeightShift = 22;
        private const int TreeAgeShift = 24;

        // Keep the order stable: each entry is one bit in the saved profile format.
        private static readonly (Func<Setting, bool> Read, Action<Setting, bool> Write)[] Flags =
        {
            (s => s.DeleteTrees, (s, value) => s.DeleteTrees = value),
            (s => s.DeleteMissingAssets, (s, value) => s.DeleteMissingAssets = value),
            (s => s.IncludeOverriddenVegetation, (s, value) => s.IncludeOverriddenVegetation = value),
            (s => s.DeleteBuildings, (s, value) => s.DeleteBuildings = value),
            (s => s.DeleteCars, (s, value) => s.DeleteCars = value),
            (s => s.DeleteRoads, (s, value) => s.DeleteRoads = value),
            (s => s.DeletePaths, (s, value) => s.DeletePaths = value),
            (s => s.DeleteRailways, (s, value) => s.DeleteRailways = value),
            (s => s.DeleteSurfaces, (s, value) => s.DeleteSurfaces = value),
            (s => s.DeleteStaticObjects, (s, value) => s.DeleteStaticObjects = value),
            (s => s.DeleteGeneralProps, (s, value) => s.DeleteGeneralProps = value),
            (s => s.DeleteStreetLights, (s, value) => s.DeleteStreetLights = value),
            (s => s.DeleteQuantityObjects, (s, value) => s.DeleteQuantityObjects = value),
            (s => s.DeleteBrandingObjects, (s, value) => s.DeleteBrandingObjects = value),
            (s => s.DeleteActivityLocations, (s, value) => s.DeleteActivityLocations = value),
            (s => s.DeleteSpawnLocations, (s, value) => s.DeleteSpawnLocations = value),
            (s => s.DeleteMarkerNetworks, (s, value) => s.DeleteMarkerNetworks = value),
            (s => s.DeleteBuildingSubObjects, (s, value) => s.DeleteBuildingSubObjects = value),
            (s => s.DeleteNetworkSubObjects, (s, value) => s.DeleteNetworkSubObjects = value),
            (s => s.ProtectOwnedObjects, (s, value) => s.ProtectOwnedObjects = value),
            (s => s.ProtectAssetVegetation, (s, value) => s.ProtectAssetVegetation = value),
            (s => s.LooseGeneralPropsOnly, (s, value) => s.LooseGeneralPropsOnly = value)
        };

        private const int Trees = 1 << 0;
        private const int MissingAssets = 1 << 1;
        private const int Railways = 1 << 7;
        private const int LooseProps = (1 << 9) | (1 << 10) | (1 << 21);
        private const int DefaultProtection = (1 << 19) | (1 << 20);

        private static readonly BrushSettings DefaultPresetBrush =
            new(AreaBulldozerSelectionShape.Circle, 30, 10, 50, 0, curved: false, polygon: false);

        internal static readonly FilterProfile[] Presets =
        {
            CreatePreset("deadTrees", DefaultProtection | Trees | ((int)AreaBulldozerTreeAgeFilter.Dead << TreeAgeShift)),
            CreatePreset("vegetation", DefaultProtection | Trees),
            CreatePreset("missingAssets", DefaultProtection | MissingAssets),
            CreatePreset("looseProps", DefaultProtection | LooseProps),
            CreatePreset("undergroundRail", DefaultProtection | Railways | ((int)AreaBulldozerHeightFilter.BelowGround << HeightShift))
        };

        private static FilterProfile CreatePreset(string id, int filters) =>
            new(id, "", filters) { Brush = DefaultPresetBrush };

        internal string Id { get; }
        internal string Name { get; set; }
        internal int Filters { get; set; }

        internal BrushSettings? Brush { get; set; }
        internal string AssetSelection { get; set; } = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("0\n"));

        internal FilterProfile(string id, string name, int filters)
        {
            Id = id;
            Name = name;
            Filters = filters;
        }

        internal static int Capture(Setting setting)
        {
            int filters = ((int)setting.HeightFilter << HeightShift) |
                ((int)setting.TreeAgeFilter << TreeAgeShift);

            for (int index = 0; index < Flags.Length; index++)
            {
                if (Flags[index].Read(setting))
                {
                    filters |= 1 << index;
                }
            }

            return filters;
        }

        internal void Apply(Setting setting)
        {
            if (!AssetFilterSettings.TryApply(AssetSelection, setting))
            {
                // A damaged asset list must not turn into an unrestricted brush.
                setting.AssetFilterEnabled = true;
                setting.AssetFilterPrefabs = "";
            }
            setting.HeightFilter = (AreaBulldozerHeightFilter)((Filters >> HeightShift) & 3);
            setting.TreeAgeFilter = (AreaBulldozerTreeAgeFilter)((Filters >> TreeAgeShift) & 7);

            for (int index = 0; index < Flags.Length; index++)
            {
                Flags[index].Write(setting, (Filters & (1 << index)) != 0);
            }
        }

        internal static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MaxNameLength)
            {
                return false;
            }

            foreach (char character in name)
            {
                if (char.IsControl(character))
                {
                    return false;
                }
            }

            return true;
        }

        internal static List<FilterProfile> ReadCustomProfiles(string stored)
        {
            var profiles = new List<FilterProfile>();
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string row in (stored ?? "").Split('\n'))
            {
                string[] columns = row.Split('\t');
                if ((columns.Length < 3 || columns.Length > 5) ||
                    !Guid.TryParseExact(columns[0], "N", out _) ||
                    !IsValidName(columns[1]) ||
                    !int.TryParse(columns[2], NumberStyles.None, CultureInfo.InvariantCulture, out int filters) ||
                    filters >= 1 << (TreeAgeShift + 3) ||
                    ((filters >> HeightShift) & 3) > (int)AreaBulldozerHeightFilter.BelowGround ||
                    ((filters >> TreeAgeShift) & 7) > (int)AreaBulldozerTreeAgeFilter.Dead ||
                    ids.Contains(columns[0]) || names.Contains(columns[1]))
                {
                    continue;
                }

                ids.Add(columns[0]);
                names.Add(columns[1]);
                var profile = new FilterProfile(columns[0], columns[1], filters);
                if (columns.Length >= 4 && BrushSettings.TryParse(columns[3], out BrushSettings brush))
                {
                    profile.Brush = brush;
                }

                if (columns.Length == 5) profile.AssetSelection = columns[4];
                profiles.Add(profile);
            }

            return profiles;
        }

        internal string Serialize()
        {
            string row = Id + "\t" + Name + "\t" + Filters.ToString(CultureInfo.InvariantCulture);
            return row + "\t" + (Brush.HasValue ? Brush.Value.Serialize() : "") + "\t" + AssetSelection;
        }

        internal readonly struct BrushSettings
        {
            internal AreaBulldozerSelectionShape Shape { get; }
            internal int Radius { get; }
            internal int LineWidth { get; }
            internal int Rounding { get; }
            internal int RotationTenths { get; }
            internal bool Curved { get; }
            internal bool Polygon { get; }

            internal BrushSettings(AreaBulldozerSelectionShape shape, int radius, int lineWidth,
                int rounding, int rotationTenths, bool curved, bool polygon)
            {
                Shape = shape;
                Radius = radius;
                LineWidth = lineWidth;
                Rounding = rounding;
                RotationTenths = rotationTenths;
                Curved = curved;
                Polygon = polygon;
            }

            internal bool Matches(BrushSettings other) =>
                Shape == other.Shape && Radius == other.Radius && LineWidth == other.LineWidth &&
                Rounding == other.Rounding && RotationTenths == other.RotationTenths &&
                Curved == other.Curved && Polygon == other.Polygon;

            internal string Serialize() => FormattableString.Invariant(
                $"{(int)Shape},{Radius},{LineWidth},{Rounding},{RotationTenths},{(Curved ? 1 : 0) | (Polygon ? 2 : 0)}");

            internal static bool TryParse(string value, out BrushSettings brush)
            {
                brush = default;
                string[] columns = value.Split(',');
                if (columns.Length != 6)
                {
                    return false;
                }

                var numbers = new int[6];
                for (int i = 0; i < columns.Length; i++)
                {
                    if (!int.TryParse(columns[i], NumberStyles.None, CultureInfo.InvariantCulture, out numbers[i]))
                    {
                        return false;
                    }
                }

                var shape = (AreaBulldozerSelectionShape)numbers[0];
                if ((shape != AreaBulldozerSelectionShape.Circle && shape != AreaBulldozerSelectionShape.Square &&
                     shape != AreaBulldozerSelectionShape.Triangle && shape != AreaBulldozerSelectionShape.Polyline) ||
                    numbers[1] < 5 || numbers[1] > 200 || numbers[2] < 2 || numbers[2] > 100 ||
                    numbers[3] < 10 || numbers[3] > 100 || numbers[4] > 3599 || numbers[5] > 3)
                {
                    return false;
                }

                brush = new BrushSettings(shape, numbers[1], numbers[2], numbers[3], numbers[4],
                    (numbers[5] & 1) != 0, (numbers[5] & 2) != 0);
                return true;
            }
        }
    }
}
