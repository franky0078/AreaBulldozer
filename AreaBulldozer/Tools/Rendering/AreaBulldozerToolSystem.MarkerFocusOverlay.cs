using Game.SceneFlow;
using System;
using System.Reflection;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // ------------------------------------------------------------
        // Abgedunkelte Marker-Fokusansicht
        // ------------------------------------------------------------

        private const string kMarkerFocusOverlayId =
            "area-bulldozer-marker-focus-overlay";

        private bool m_MarkerFocusOverlayVisible;
        private bool m_MarkerFocusOverlayFailureLogged;
        private int m_AppliedMarkerBackgroundDarkness;

        private void InitializeMarkerFocusOverlay()
        {
            m_MarkerFocusOverlayVisible = false;
            m_MarkerFocusOverlayFailureLogged = false;
            m_AppliedMarkerBackgroundDarkness = -1;

            RemoveMarkerFocusOverlay();
        }

        private void DisposeMarkerFocusOverlay()
        {
            RemoveMarkerFocusOverlay();
        }

        private void UpdateMarkerFocusOverlay(
            bool markerFilterActive)
        {
            bool shouldDimBackground =
                markerFilterActive &&
                Mod.Settings != null &&
                Mod.Settings.DimMarkerBackground;

            if (!shouldDimBackground)
            {
                RemoveMarkerFocusOverlay();
                return;
            }

            int darkness =
                Unity.Mathematics.math.clamp(
                    Mod.Settings.MarkerBackgroundDarkness,
                    10,
                    70);

            if (m_MarkerFocusOverlayVisible &&
                m_AppliedMarkerBackgroundDarkness ==
                    darkness)
            {
                return;
            }

            ShowMarkerFocusOverlay(
                darkness);
        }

        private void ShowMarkerFocusOverlay(
            int darknessPercent)
        {
            float alpha =
                darknessPercent / 100f;

            string alphaText =
                alpha.ToString(
                    "0.00",
                    System.Globalization.CultureInfo.InvariantCulture);

            string script =
                "(function(){" +
                "var id='" + kMarkerFocusOverlayId + "';" +
                "var e=document.getElementById(id);" +
                "if(!e){" +
                    "e=document.createElement('div');" +
                    "e.id=id;" +
                    "e.setAttribute('aria-hidden','true');" +
                    "e.style.position='fixed';" +
                    "e.style.left='0';" +
                    "e.style.top='0';" +
                    "e.style.width='100vw';" +
                    "e.style.height='100vh';" +
                    "e.style.pointerEvents='none';" +
                    "e.style.zIndex='0';" +
                    "e.style.transition='background 100ms linear';" +
                    "if(document.body.firstChild){" +
                        "document.body.insertBefore(" +
                            "e,document.body.firstChild);" +
                    "}else{" +
                        "document.body.appendChild(e);" +
                    "}" +
                "}" +
                "e.style.display='block';" +
                "e.style.background='rgba(0,0,0," +
                    alphaText + ")';" +
                "})();";

            if (!TryExecuteUiScript(script))
            {
                return;
            }

            m_MarkerFocusOverlayVisible = true;
            m_AppliedMarkerBackgroundDarkness =
                darknessPercent;

            Mod.LogDiagnosticInfo(
                $"Marker background dimming enabled at " +
                $"{darknessPercent}%.");
        }

        private void RemoveMarkerFocusOverlay()
        {
            if (!m_MarkerFocusOverlayVisible &&
                m_AppliedMarkerBackgroundDarkness < 0)
            {
                return;
            }

            string script =
                "(function(){" +
                "var e=document.getElementById('" +
                    kMarkerFocusOverlayId + "');" +
                "if(e){e.remove();}" +
                "})();";

            TryExecuteUiScript(script);

            if (m_MarkerFocusOverlayVisible)
            {
                Mod.LogDiagnosticInfo(
                    "Marker background dimming removed.");
            }

            m_MarkerFocusOverlayVisible = false;
            m_AppliedMarkerBackgroundDarkness = -1;
        }

        private bool TryExecuteUiScript(
            string script)
        {
            try
            {
                object gameManager =
                    GameManager.instance;

                object userInterface =
                    GetMemberValue(
                        gameManager,
                        "userInterface");

                object viewSystem =
                    GetMemberValue(
                        userInterface,
                        "view");

                object coherentView =
                    GetMemberValue(
                        viewSystem,
                        "View");

                if (coherentView == null)
                {
                    return false;
                }

                MethodInfo executeScriptMethod =
                    coherentView
                        .GetType()
                        .GetMethod(
                            "ExecuteScript",
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic,
                            binder: null,
                            types: new[]
                            {
                                typeof(string)
                            },
                            modifiers: null);

                if (executeScriptMethod == null)
                {
                    throw new MissingMethodException(
                        coherentView.GetType().FullName,
                        "ExecuteScript(string)");
                }

                executeScriptMethod.Invoke(
                    coherentView,
                    new object[]
                    {
                        script
                    });

                m_MarkerFocusOverlayFailureLogged = false;
                return true;
            }
            catch (TargetInvocationException exception)
            {
                Exception actualException =
                    exception.InnerException ??
                    exception;

                LogMarkerFocusOverlayFailure(
                    actualException);

                return false;
            }
            catch (Exception exception)
            {
                LogMarkerFocusOverlayFailure(
                    exception);

                return false;
            }
        }

        private static object GetMemberValue(
            object source,
            string memberName)
        {
            if (source == null ||
                string.IsNullOrWhiteSpace(memberName))
            {
                return null;
            }

            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.IgnoreCase;

            Type sourceType =
                source.GetType();

            return sourceType
                       .GetProperty(
                           memberName,
                           flags)?
                       .GetValue(source) ??
                   sourceType
                       .GetField(
                           memberName,
                           flags)?
                       .GetValue(source);
        }

        private void LogMarkerFocusOverlayFailure(
            Exception exception)
        {
            if (m_MarkerFocusOverlayFailureLogged)
            {
                return;
            }

            // when diagnostic logging is disabled.
            Mod.Log.Warn(
                "Marker background dimming could not be " +
                $"applied: {exception.Message}");

            m_MarkerFocusOverlayFailureLogged = true;
        }
    }
}
