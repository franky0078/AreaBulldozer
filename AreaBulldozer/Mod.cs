using AreaBulldozer.Localization;
using AreaBulldozer.Tools;
using AreaBulldozer.UISystems;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Input;
using Game.Modding;
using Game.SceneFlow;
using UnityEngine.InputSystem;

namespace AreaBulldozer
{
    public class Mod : IMod
    {
        public const string ModVersion = "1.1.0";

        public static readonly ILog Log = LogManager
            .GetLogger(
                $"{nameof(AreaBulldozer)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);


        public static Setting Settings { get; private set; }

        private ProxyAction m_ActivateToolAction;

        public const string ActivateToolActionName =
            "ActivateTool";

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info("Area Bulldozer is loading.");

            if (GameManager.instance.modManager
                .TryGetExecutableAsset(this, out var asset))
            {
                Log.Info($"Current mod asset: {asset.path}");
            }

            // --------------------------------------------------------
            // Einstellungen
            // --------------------------------------------------------

            Settings = new Setting(this);

            Settings.RegisterInOptionsUI();
            Settings.RegisterKeyBindings();

            // Englische Lokalisierung
            GameManager.instance.localizationManager.AddSource(
                "en-US",
                new LocaleEN(Settings));

            // Deutsche Lokalisierung
            GameManager.instance.localizationManager.AddSource(
                "de-DE",
                new LocaleDE(Settings));

            // Gespeicherte Einstellungen laden
            AssetDatabase.global.LoadSettings(
                nameof(AreaBulldozer),
                Settings,
                new Setting(this));

            // --------------------------------------------------------
            // Tastenkürzel
            // --------------------------------------------------------

            m_ActivateToolAction =
                Settings.GetAction(ActivateToolActionName);

            if (m_ActivateToolAction != null)
            {
                m_ActivateToolAction.shouldBeEnabled = true;

                m_ActivateToolAction.onInteraction +=
                    OnActivateToolInteraction;

                Log.Info(
                    "Area Bulldozer activation action registered.");
            }
            else
            {
                Log.Warn(
                    "Area Bulldozer activation action was not found.");
            }

            // --------------------------------------------------------
            // Tool-System
            // --------------------------------------------------------

            updateSystem.UpdateAt<AreaBulldozerToolSystem>(
                SystemUpdatePhase.ToolUpdate);

            updateSystem.UpdateAt<AreaBulldozerUISystem>(
                SystemUpdatePhase.UIUpdate);

            Log.Info(
                "Area Bulldozer tool and UI systems registered.");

            Log.Info(
                "Area Bulldozer loaded successfully.");
        }

        private void OnActivateToolInteraction(
            ProxyAction action,
            InputActionPhase phase)
        {
            if (phase != InputActionPhase.Performed)
            {
                return;
            }

            AreaBulldozerToolSystem tool =
                AreaBulldozerToolSystem.Instance;

            if (tool == null)
            {
                Log.Warn(
                    "The Area Bulldozer tool system is not available.");

                return;
            }

            tool.ToggleTool();
        }

        public void OnDispose()
        {
            Log.Info("Area Bulldozer is unloading.");

            if (m_ActivateToolAction != null)
            {
                m_ActivateToolAction.onInteraction -=
                    OnActivateToolInteraction;

                m_ActivateToolAction.shouldBeEnabled = false;
                m_ActivateToolAction = null;
            }

            if (Settings != null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }

            Log.Info("Area Bulldozer unloaded.");
        }
    }
}