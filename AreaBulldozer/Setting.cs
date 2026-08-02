using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;

namespace AreaBulldozer
{
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

        // ------------------------------------------------------------
        // Prop- und Markerfilter der Werkzeug-UI
        // ------------------------------------------------------------

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

        // ------------------------------------------------------------
        // Sicherheit – nur in den normalen Mod-Optionen
        // ------------------------------------------------------------

        [SettingsUISection(kSection, kSubObjectGroup)]
        public bool DeleteBuildingSubObjects { get; set; }

        [SettingsUISection(kSection, kSubObjectGroup)]
        public bool DeleteNetworkSubObjects { get; set; }

        [SettingsUISection(kSection, kSubObjectGroup)]
        public bool ProtectOwnedObjects { get; set; }

        // ------------------------------------------------------------
        // Darstellung und Bedienung
        // ------------------------------------------------------------

        [SettingsUIHidden]
        public bool UseSquareBrush { get; set; }

        [SettingsUIHidden]
        public int BrushRadius { get; set; }

        [SettingsUISlider(
            min = 75,
            max = 125,
            step = 5)]
        [SettingsUISection(kSection, kToolGroup)]
        public int UIScale { get; set; }

        [SettingsUISection(kSection, kToolGroup)]
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

                Mod.Log.Info(
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

        // ------------------------------------------------------------
        // Tastenkürzel
        // ------------------------------------------------------------

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
                Mod.Log.Info(
                    "Resetting Area Bulldozer key bindings.");

                ResetKeyBindings();
            }
        }

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

            // Riskier scopes remain disabled by default.
            DeleteBuildingSubObjects = false;
            DeleteNetworkSubObjects = false;
            ProtectOwnedObjects = true;

            UseSquareBrush = false;
            BrushRadius = 30;
            UIScale = 100;

            UseUniversalModMenu = false;
            LauncherButtonMovable = false;
            LauncherPositionX = 54;
            LauncherPositionY = 8;

            ConfirmLargeSelection = true;
            LargeSelectionThreshold = 250;
        }
    }
}
