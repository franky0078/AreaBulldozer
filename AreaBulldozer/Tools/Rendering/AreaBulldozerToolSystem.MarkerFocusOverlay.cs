using Game.SceneFlow;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private const string kMarkerFocusOverlayId =
            "area-bulldozer-marker-focus-overlay";
        private const string kOutlinePassTypeName =
            "Game.Rendering.OutlinesWorldUIPass";

        private AreaBulldozerMarkerFocusPass m_MarkerFocusPass;
        private CustomPassVolume m_MarkerFocusVolume;
        private bool m_MarkerFocusOverlayVisible;
        private bool m_MarkerFocusOverlayFailureLogged;
        private int m_AppliedMarkerBackgroundDarkness;
        private float m_NextMarkerFocusAttachAttempt;

        private void InitializeMarkerFocusOverlay()
        {
            m_MarkerFocusOverlayVisible = false;
            m_MarkerFocusOverlayFailureLogged = false;
            m_AppliedMarkerBackgroundDarkness = -1;
            m_NextMarkerFocusAttachAttempt = 0f;

            TryExecuteUiScript(
                "(function(){var e=document.getElementById('" +
                kMarkerFocusOverlayId + "');if(e){e.remove();}})();");
        }

        private void DisposeMarkerFocusOverlay()
        {
            RemoveMarkerFocusOverlay();
        }

        private void UpdateMarkerFocusOverlay(bool focusFilterActive)
        {
            bool shouldDimBackground = focusFilterActive &&
                Mod.Settings != null && Mod.Settings.DimMarkerBackground;

            if (!shouldDimBackground)
            {
                RemoveMarkerFocusOverlay();
                return;
            }

            int darkness = Unity.Mathematics.math.clamp(
                Mod.Settings.MarkerBackgroundDarkness, 10, 70);

            ShowMarkerFocusOverlay(darkness);
        }

        private void ShowMarkerFocusOverlay(int darknessPercent)
        {
            // Reattach if a scene change replaced the game's render volume.
            bool isAttached = m_MarkerFocusPass != null &&
                m_MarkerFocusVolume != null &&
                m_MarkerFocusVolume.isActiveAndEnabled &&
                m_MarkerFocusVolume.customPasses.Contains(m_MarkerFocusPass);

            if (!isAttached)
            {
                if (UnityEngine.Time.realtimeSinceStartup < m_NextMarkerFocusAttachAttempt)
                {
                    return;
                }

                m_NextMarkerFocusAttachAttempt = UnityEngine.Time.realtimeSinceStartup + 2f;
                if (!TryAttachMarkerFocusPass())
                {
                    return;
                }
            }

            m_MarkerFocusPass.TargetCamera = Camera.main;
            float remainingBrightness = 1f - darknessPercent / 100f;
            m_MarkerFocusPass.BrightnessMultiplier =
                Unity.Mathematics.math.pow(remainingBrightness, 2.2f);
            m_MarkerFocusPass.enabled = true;

            if (!m_MarkerFocusOverlayVisible ||
                m_AppliedMarkerBackgroundDarkness != darknessPercent)
            {
                Mod.LogDiagnosticInfo(
                    $"Native marker background dimming enabled at {darknessPercent}%. " +
                    "The background pass precedes the game's outlines and world UI.");
            }

            m_MarkerFocusOverlayVisible = true;
            m_AppliedMarkerBackgroundDarkness = darknessPercent;
        }

        private bool TryAttachMarkerFocusPass()
        {
            if (m_MarkerFocusPass != null)
            {
                m_MarkerFocusPass.enabled = false;
                if (m_MarkerFocusVolume != null)
                {
                    m_MarkerFocusVolume.customPasses.Remove(m_MarkerFocusPass);
                }
            }
            m_MarkerFocusVolume = null;

            CustomPassVolume[] volumes =
                UnityEngine.Object.FindObjectsOfType<CustomPassVolume>();

            foreach (CustomPassVolume volume in volumes)
            {
                if (!volume.isActiveAndEnabled)
                {
                    continue;
                }

                for (int index = 0; index < volume.customPasses.Count; index++)
                {
                    CustomPass pass = volume.customPasses[index];
                    if (pass == null || !pass.enabled ||
                        pass.GetType().FullName != kOutlinePassTypeName)
                    {
                        continue;
                    }

                    if (m_MarkerFocusPass == null)
                    {
                        m_MarkerFocusPass = new AreaBulldozerMarkerFocusPass();
                    }

                    if (!m_MarkerFocusPass.TryInitialize())
                    {
                        LogMarkerFocusOverlayFailure(new InvalidOperationException(
                            "The HDRP/Unlit shader or its ForwardOnly pass is unavailable. " +
                            "Background dimming has been left off."));
                        return false;
                    }

                    volume.customPasses.Insert(index, m_MarkerFocusPass);
                    m_MarkerFocusVolume = volume;
                    m_MarkerFocusOverlayFailureLogged = false;
                    Mod.LogDiagnosticInfo(
                        "Native background pass attached before OutlinesWorldUIPass " +
                        $"at {volume.injectionPoint} in volume {volume.name}.");
                    return true;
                }
            }

            LogMarkerFocusOverlayFailure(new InvalidOperationException(
                "No active OutlinesWorldUIPass was found. " +
                "Background dimming is off; attachment will be retried."));
            return false;
        }

        private void RemoveMarkerFocusOverlay()
        {
            if (m_MarkerFocusPass != null)
            {
                // Disable first so a queued render cannot draw another dim frame.
                m_MarkerFocusPass.enabled = false;
                m_MarkerFocusPass.BrightnessMultiplier = 1f;
                m_MarkerFocusPass.TargetCamera = null;
                if (m_MarkerFocusVolume != null)
                {
                    m_MarkerFocusVolume.customPasses.Remove(m_MarkerFocusPass);
                }
                m_MarkerFocusPass.ReleaseResources();
                m_MarkerFocusPass = null;
            }
            m_MarkerFocusVolume = null;

            if (m_MarkerFocusOverlayVisible)
            {
                Mod.LogDiagnosticInfo("Native marker background dimming removed.");
            }

            m_MarkerFocusOverlayVisible = false;
            m_AppliedMarkerBackgroundDarkness = -1;
            m_NextMarkerFocusAttachAttempt = 0f;
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
