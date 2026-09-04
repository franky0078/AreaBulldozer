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
        public const string ModVersion = "1.5.0";

        public static readonly ILog Log = LogManager
            .GetLogger(
                $"{nameof(AreaBulldozer)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);


        public static Setting Settings { get; private set; }

        public static bool DiagnosticLoggingEnabled =>
            Settings?.EnableDiagnosticLogging ?? false;

        public static void LogDiagnosticInfo(string message)
        {
            if (!DiagnosticLoggingEnabled ||
                string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            try
            {
                Log?.Info(message);
            }
            catch
            {
                // Never let logging interfere with the tool.
            }
        }

        private ProxyAction m_ActivateToolAction;

        public const string ActivateToolActionName =
            "ActivateTool";

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Einstellungen
            Settings = new Setting(this);

            Settings.RegisterInOptionsUI();
            Settings.RegisterKeyBindings();

            GameManager.instance.localizationManager.AddSource(
                "en-US",
                new LocaleEN(Settings));

            GameManager.instance.localizationManager.AddSource(
                "en-US",
                new LocaleSelectionShapesEN(Settings));

            GameManager.instance.localizationManager.AddSource(
                "en-US",
                new LocalePolygonEN());

            GameManager.instance.localizationManager.AddSource(
                "de-DE",
                new LocaleDE(Settings));

            GameManager.instance.localizationManager.AddSource(
                "de-DE",
                new LocaleSelectionShapesDE(Settings));

            GameManager.instance.localizationManager.AddSource(
                "de-DE",
                new LocalePolygonDE());

            AssetDatabase.global.LoadSettings(
                nameof(AreaBulldozer),
                Settings,
                new Setting(this));

            if (DiagnosticLoggingEnabled)
            {
                LogDiagnosticInfo("Area Bulldozer is loading.");
                LogDiagnosticInfo(
                    "Area Bulldozer diagnostic logging is enabled.");

                if (GameManager.instance.modManager
                    .TryGetExecutableAsset(this, out var asset))
                {
                    LogDiagnosticInfo(
                        $"Current mod asset: {asset.path}");
                }
            }


            // Tastenkürzel
            m_ActivateToolAction =
                Settings.GetAction(ActivateToolActionName);

            if (m_ActivateToolAction != null)
            {
                m_ActivateToolAction.shouldBeEnabled = true;

                m_ActivateToolAction.onInteraction +=
                    OnActivateToolInteraction;

                LogDiagnosticInfo(
                    "Area Bulldozer activation action registered.");
            }
            else
            {
                Log.Warn(
                    "Area Bulldozer activation action was not found.");
            }

            // Tool-System
            updateSystem.UpdateAt<AreaBulldozerToolSystem>(
                SystemUpdatePhase.ToolUpdate);

            updateSystem.UpdateAt<AreaBulldozerPolygonOverlaySystem>(
                SystemUpdatePhase.ToolUpdate);

            updateSystem.UpdateAt<AreaBulldozerUISystem>(
                SystemUpdatePhase.UIUpdate);

            updateSystem.UpdateAt<AreaBulldozerPolygonUISystem>(
                SystemUpdatePhase.UIUpdate);

            LogDiagnosticInfo(
                "Area Bulldozer tool and UI systems registered.");

            LogDiagnosticInfo(
                "Area Bulldozer free-area polygon support registered.");

            LogDiagnosticInfo(
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
            LogDiagnosticInfo("Area Bulldozer is unloading.");

            if (m_ActivateToolAction != null)
            {
                m_ActivateToolAction.onInteraction -=
                    OnActivateToolInteraction;

                m_ActivateToolAction.shouldBeEnabled = false;
                m_ActivateToolAction = null;
            }

            LogDiagnosticInfo("Area Bulldozer unloaded.");

            if (Settings != null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }
        }
    }
}
