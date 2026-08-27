using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Area Bulldozer" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "Safety" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "Display and launcher" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "Key bindings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "Information" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "Remove trees and vegetation" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "Targets trees, bushes, and other vegetation inside the selection area." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "Remove buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "Targets complete root buildings inside the selection area. Building-owned props, markers, and lanes are not counted as separate buildings." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "Remove roads" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "Targets complete main road edges whose curve intersects the selection area. Paths, railways, marker networks, and building-owned networks remain excluded." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "Remove pedestrian paths" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "Targets complete standalone pedestrian-path edges whose route intersects the selection area. Roads, railways, marker networks, and building-owned SubNets remain excluded." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "Remove railway tracks" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "Targets complete standalone train, tram, subway, and metro track edges whose route intersects the selection area. Roads, pedestrian paths, marker networks, and building-owned SubNets remain excluded." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "Remove surfaces and spaces" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "Targets complete polygonal surfaces and spaces when the selection area intersects their border or its center lies inside the area. Asset-owned areas also require the matching sub-object scope and may regenerate after an asset update." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "Remove static objects" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "Enables the selected prop and marker categories below. Visible props are controlled directly by their category; sensitive activity, spawn, and lane markers still respect the sub-object safety options." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "General props and decorations" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "Targets static props that do not belong to one of the more specific categories." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "Street lights" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "Targets objects with the street-light component, including lights owned by buildings or networks." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "Trash bins and quantity objects" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "Targets quantity objects, which can include trash bins and similar props." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "Branding and advertisements" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "Targets brand objects, advertisements, and similar decorations." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "Activity locations" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "Targets asset activity locations. These can affect building behavior and are disabled by default." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "Spawn locations – blue" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "Targets the blue SpawnLocation markers where vehicles or other asset objects can appear. Owned spawn locations also require the matching sub-object scope." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Asset lanes / SubLanes – green and blue" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Targets green and blue dashed vehicle, pedestrian, and parking lanes inside building assets. Only SubLanes using NetLanePrefab or NetLaneGeometryPrefab are selected; normal road edges remain excluded." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "Dim marker background" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "Dims the game view while activity locations, spawn locations, or asset lanes are enabled. Menus remain interactive." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Background darkness" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Sets the game-view dimming strength in percent." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "Selection line thickness" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "Sets a fixed outline thickness for the circular and square selection shapes. The thickness no longer changes with the selection size." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Include building sub-objects" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Allows the selected object types when they belong to a building. The matching object filter must also be enabled. Sub-objects may regenerate after building updates." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Include network sub-objects" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Allows the selected object types when they belong to a road or another network. The matching object filter must also be enabled. Sub-objects may regenerate later." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "Protect owned objects" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "Protects owned objects whose owner is not recognized as a building or network. This safety option should remain enabled." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "Use square selection" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "Disabled: circular selection. Enabled: freely rotatable square. Hold the right mouse button and move the mouse horizontally to rotate it. The configured size is the circle radius or the square half-side length." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "Default selection size" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "Sets the size in metres: circle radius or square half-side length." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "Tool window scale" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Scales the Area Bulldozer window from 75 to 125 percent. Lower values provide more space; higher values improve readability on high-resolution displays." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseUniversalModMenu)), "Show button in the new mod menu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseUniversalModMenu)), "Moves the Area Bulldozer launcher into the game's universal mod menu. The separate floating button is hidden while this option is enabled." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "Make floating button movable" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Allows the separate launcher to be moved with Ctrl + left mouse button. Releasing the mouse button saves the position. A normal left click only toggles the tool. This option has no effect when the new mod menu is used." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "Reset button position" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "Moves the draggable launcher back to its default position in the top-left corner." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "Confirm large selections" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "Requires a second click within five seconds when the configured number of objects is selected. The selection shape turns yellow while confirmation is pending." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "Confirmation threshold" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "Sets the number of selected objects at which a second confirmation is required." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "Activate Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "Activates or deactivates the tool." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset key bindings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Resets the key binding to its default value." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "Shows the currently installed version of Area Bulldozer." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "Activate Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.ToggleTool", "Toggle Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "Hold Ctrl + left mouse button to move; release to save" },
                { "AreaBulldozer.UI.ModMenuOpen", "Open tool" },
                { "AreaBulldozer.UI.Close", "Close tool" },
                { "AreaBulldozer.UI.Active", "Active" },
                { "AreaBulldozer.UI.Subtitle", "Remove multiple objects within one area" },
                { "AreaBulldozer.UI.Selection", "Selection" },
                { "AreaBulldozer.UI.Circle", "Circle" },
                { "AreaBulldozer.UI.Square", "Square" },
                { "AreaBulldozer.UI.CircleTooltip", "Round selection area around the cursor." },
                { "AreaBulldozer.UI.SquareTooltip", "Square selection area, rotatable by holding the right mouse button." },
                { "AreaBulldozer.UI.DecreaseSize", "Shrink selection" },
                { "AreaBulldozer.UI.IncreaseSize", "Grow selection" },
                { "AreaBulldozer.UI.RotateLeft", "Rotate counter-clockwise" },
                { "AreaBulldozer.UI.RotateRight", "Rotate clockwise" },
                { "AreaBulldozer.UI.VegetationTooltip", "Delete trees, bushes and plants." },
                { "AreaBulldozer.UI.BuildingsTooltip", "Delete buildings in the area." },
                { "AreaBulldozer.UI.RoadsTooltip", "Delete roads in the area." },
                { "AreaBulldozer.UI.PathsTooltip", "Delete pedestrian and cycle paths." },
                { "AreaBulldozer.UI.RailwaysTooltip", "Delete railway tracks." },
                { "AreaBulldozer.UI.SurfacesTooltip", "Delete surfaces and areas." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "Master switch for props and markers. Enables the detail filters below." },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "Props / Markers" },
                { "AreaBulldozer.UI.Radius", "Radius" },
                { "AreaBulldozer.UI.HalfSide", "Square size" },
                { "AreaBulldozer.UI.Rotation", "Rotation" },
                { "AreaBulldozer.UI.Degrees", "deg" },
                { "AreaBulldozer.UI.MainFilters", "Object filters" },
                { "AreaBulldozer.UI.ChooseFilters", "Choose filters" },
                { "AreaBulldozer.UI.Vegetation", "Trees and vegetation" },
                { "AreaBulldozer.UI.VegetationShort", "Vegetation" },
                { "AreaBulldozer.UI.Buildings", "Buildings" },
                { "AreaBulldozer.UI.Roads", "Roads" },
                { "AreaBulldozer.UI.Paths", "Pedestrian paths" },
                { "AreaBulldozer.UI.Railways", "Railway tracks" },
                { "AreaBulldozer.UI.Surfaces", "Surfaces and spaces" },
                { "AreaBulldozer.UI.SurfacesShort", "Surfaces" },
                { "AreaBulldozer.UI.StaticObjects", "Props and markers" },
                { "AreaBulldozer.UI.StaticObjectsShort", "Props / markers" },
                { "AreaBulldozer.UI.AdvancedFilters", "Advanced prop and marker filters" },
                { "AreaBulldozer.UI.StaticMasterNotice", "Enable Props and markers above to use these categories." },
                { "AreaBulldozer.UI.GeneralProps", "General props" },
                { "AreaBulldozer.UI.StreetLights", "Street lights" },
                { "AreaBulldozer.UI.QuantityObjects", "Bins and quantity objects" },
                { "AreaBulldozer.UI.Branding", "Branding and advertisements" },
                { "AreaBulldozer.UI.ActivityLocations", "Activity locations" },
                { "AreaBulldozer.UI.SpawnLocations", "Spawn locations" },
                { "AreaBulldozer.UI.AssetLanes", "Asset lanes / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "Dim marker background" },
                { "AreaBulldozer.UI.BackgroundDarkness", "Background darkness" },
                { "AreaBulldozer.UI.Safety", "Safety" },
                { "AreaBulldozer.UI.BuildingSubObjects", "Building sub-objects" },
                { "AreaBulldozer.UI.NetworkSubObjects", "Network sub-objects" },
                { "AreaBulldozer.UI.ProtectOwned", "Protect owned objects" },
                { "AreaBulldozer.UI.ConfirmLarge", "Confirm large selections" },
                { "AreaBulldozer.UI.Threshold", "Confirmation threshold" },
                { "AreaBulldozer.UI.Objects", "objects" },
                { "AreaBulldozer.UI.ApplyHelp", "Delete selected objects" },
                { "AreaBulldozer.UI.RotateHelp", "Hold and drag to rotate" },
                { "AreaBulldozer.UI.ShortcutHelp", "Toggle tool" }
            };
        }

        public void Unload()
        {
        }
    }
}
