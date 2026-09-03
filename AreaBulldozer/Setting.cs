using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;

namespace AreaBulldozer
{
    public enum AreaBulldozerLauncherMode
    {
        Standalone = 0,
        VanillaBulldozer = 1,
        UniversalModMenu = 2
    }

    public enum AreaBulldozerSelectionShape
    {
        Circle = 0,
        Square = 1,
        Triangle = 2,

        LegacyLine = 3,
        Polyline = 4
    }

    [FileLocation(nameof(AreaBulldozer))]
    [SettingsUIGroupOrder(
        kToolGroup,
        kSubObjectGroup,
        kKeybindingGroup,
        kAboutGroup)]
    [SettingsUIShowGroupName(
        kToolGroup,
        kSubObjectGroup,
        kKeybindingGroup,
        kAboutGroup)]
    [SettingsUIKeyboardAction(
        Mod.ActivateToolActionName,
        ActionType.Button,
        usages: new string[]
        {
            Usages.kMenuUsage,
            "AreaBulldozerUsage"
        },
        interactions: new string[]
        {
            "UIButton"
        })]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kFilterGroup = "Filters";
        public const string kStaticCategoryGroup = "StaticCategories";
        public const string kSubObjectGroup = "SubObjects";
        public const string kToolGroup = "Tool";
        public const string kKeybindingGroup = "KeyBinding";
        public const string kAboutGroup = "About";

        private AreaBulldozerLauncherMode m_LauncherMode;

        public Setting(IMod mod)
            : base(mod)
        {
            SetDefaults();
        }

        [SettingsUIHidden]
        public bool DeleteTrees { get; set; }

        [SettingsUIHidden]
        public bool DeleteBuildings { get; set; }

        [SettingsUIHidden]
        public bool DeleteRoads { get; set; }

        [SettingsUIHidden]
        public bool DeletePaths { get; set; }

        [SettingsUIHidden]
        public bool DeleteRailways { get; set; }

        [SettingsUIHidden]
        public bool DeleteSurfaces { get; set; }

        [SettingsUIHidden]
        public bool DeleteStaticObjects { get; set; }

        [SettingsUIHidden]
        public bool DeleteGeneralProps { get; set; }

        [SettingsUIHidden]
        public bool DeleteStreetLights { get; set; }

        [SettingsUIHidden]
        public bool DeleteQuantityObjects { get; set; }

        [SettingsUIHidden]
        public bool DeleteBrandingObjects { get; set; }

        [SettingsUIHidden]
        public bool DeleteActivityLocations { get; set; }

        [SettingsUIHidden]
        public bool DeleteSpawnLocations { get; set; }

        [SettingsUIHidden]
        public bool DeleteMarkerNetworks { get; set; }

        [SettingsUIHidden]
        public bool DimMarkerBackground { get; set; }

        [SettingsUISlider(
            min = 10,
            max = 70,
            step = 5)]
        [SettingsUISection(kSection, kToolGroup)]
        public int MarkerBackgroundDarkness { get; set; }

        [SettingsUISection(kSection, kSubObjectGroup)]
        public bool DeleteBuildingSubObjects { get; set; }

        [SettingsUISection(kSection, kSubObjectGroup)]
        public bool DeleteNetworkSubObjects { get; set; }

        [SettingsUISection(kSection, kSubObjectGroup)]
        public bool ProtectOwnedObjects { get; set; }

        [SettingsUIHidden]
        public AreaBulldozerSelectionShape SelectionShape { get; set; }

        [SettingsUIHidden]
        public bool UseSquareBrush { get; set; }

        [SettingsUIHidden]
        public int BrushRadius { get; set; }

        [SettingsUIHidden]
        public int LineWidth { get; set; }

        [SettingsUIHidden]
        public bool UseCurvedPolyline { get; set; }

        [SettingsUIHidden]
        public int PolylineRounding { get; set; }

        [SettingsUISlider(
            min = 25,
            max = 150,
            step = 5)]
        [SettingsUISection(kSection, kToolGroup)]
        public int SelectionLineThickness { get; set; }

        [SettingsUISlider(
            min = 75,
            max = 125,
            step = 5)]
        [SettingsUISection(kSection, kToolGroup)]
        public int UIScale { get; set; }

        [SettingsUISection(kSection, kToolGroup)]
        public AreaBulldozerLauncherMode LauncherMode
        {
            get =>
                UseUniversalModMenu
                    ? AreaBulldozerLauncherMode.UniversalModMenu
                    : m_LauncherMode;
            set
            {
                m_LauncherMode = value;

                // Compatibility with the former boolean option.
                UseUniversalModMenu = false;
            }
        }

        // Old setting from previous versions.
        [SettingsUIHidden]
        public bool UseUniversalModMenu { get; set; }

        [SettingsUISection(kSection, kToolGroup)]
        public bool LauncherButtonMovable { get; set; }

        [SettingsUIHidden]
        public int LauncherPositionX { get; set; }

        [SettingsUIHidden]
        public int LauncherPositionY { get; set; }

        [SettingsUISection(kSection, kToolGroup)]
        public bool ResetLauncherPosition
        {
            set
            {
                LauncherPositionX = 54;
                LauncherPositionY = 8;

                Mod.LogDiagnosticInfo(
                    "Resetting Area Bulldozer launcher position.");
            }
        }

        [SettingsUISection(kSection, kSubObjectGroup)]
        public bool ConfirmLargeSelection { get; set; }

        [SettingsUISlider(
            min = 50,
            max = 2000,
            step = 50)]
        [SettingsUISection(kSection, kSubObjectGroup)]
        public int LargeSelectionThreshold { get; set; }

        [SettingsUIKeyboardBinding(
            BindingKeyboard.B,
            Mod.ActivateToolActionName,
            shift: true)]
        [SettingsUISection(kSection, kKeybindingGroup)]
        public ProxyBinding ActivateToolBinding { get; set; }

        [SettingsUISection(kSection, kKeybindingGroup)]
        public bool ResetBindings
        {
            set
            {
                Mod.LogDiagnosticInfo(
                    "Resetting Area Bulldozer key bindings.");

                ResetKeyBindings();
            }
        }

        [SettingsUISection(kSection, kAboutGroup)]
        public bool EnableDiagnosticLogging { get; set; }

        [SettingsUISection(kSection, kAboutGroup)]
        public string Version => Mod.ModVersion;

        public override void SetDefaults()
        {
            DeleteTrees = true;
            DeleteBuildings = false;
            DeleteRoads = false;
            DeletePaths = false;
            DeleteRailways = false;
            DeleteSurfaces = false;
            DeleteStaticObjects = false;

            DeleteGeneralProps = true;
            DeleteStreetLights = true;
            DeleteQuantityObjects = true;
            DeleteBrandingObjects = true;

            DeleteActivityLocations = false;
            DeleteSpawnLocations = false;
            DeleteMarkerNetworks = false;

            DimMarkerBackground = true;
            MarkerBackgroundDarkness = 40;

            DeleteBuildingSubObjects = false;
            DeleteNetworkSubObjects = false;
            ProtectOwnedObjects = true;

            SelectionShape = AreaBulldozerSelectionShape.Circle;
            UseSquareBrush = false;
            BrushRadius = 30;
            LineWidth = 10;
            UseCurvedPolyline = false;
            PolylineRounding = 50;

            SelectionLineThickness = 65;

            UIScale = 100;

            LauncherMode = AreaBulldozerLauncherMode.Standalone;
            UseUniversalModMenu = false;

            LauncherButtonMovable = false;
            LauncherPositionX = 54;
            LauncherPositionY = 8;

            ConfirmLargeSelection = true;
            LargeSelectionThreshold = 250;

            // Info/diagnostic logging is deliberately disabled by default.
            // Warnings and errors are always written.
            EnableDiagnosticLogging = false;
        }
    }
}
