using AreaBulldozer.Tools;
using Colossal.UI.Binding;
using Game;
using Game.UI;

namespace AreaBulldozer.UISystems
{
    public partial class AreaBulldozerPolygonUISystem :
        UISystemBase
    {
        private const string kModId =
            "AreaBulldozer";

        private const string kFreeAreaPolygon =
            "freeAreaPolygon";

        private const string kFreeAreaPolygonPointCount =
            "freeAreaPolygonPointCount";

        private const string kSetFreeAreaPolygon =
            "setFreeAreaPolygon";

        private AreaBulldozerToolSystem m_Tool;

        private bool m_HasSavedState;
        private AreaBulldozerSelectionShape m_PreviousShape;
        private bool m_PreviousUseSquareBrush;
        private int m_PreviousLineWidth;
        private bool m_PreviousCurvedPolyline;

        public override GameMode gameMode =>
            GameMode.GameOrEditor;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_Tool =
                World.GetOrCreateSystemManaged<
                    AreaBulldozerToolSystem>();

            AddUpdateBinding(
                new GetterValueBinding<bool>(
                    kModId,
                    kFreeAreaPolygon,
                    () =>
                        FreeAreaPolygonModeState.Active));

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    kModId,
                    kFreeAreaPolygonPointCount,
                    () =>
                        m_Tool?.
                            CurrentFreeAreaPolygonPointCount ??
                        0));

            AddBinding(
                new TriggerBinding<bool>(
                    kModId,
                    kSetFreeAreaPolygon,
                    SetFreeAreaPolygonMode));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (!FreeAreaPolygonModeState.Active ||
                Mod.Settings == null)
            {
                return;
            }

            // A normal shape button was selected while polygon mode was
            // active. Keep the newly selected shape and only restore the
            // temporary polygon rendering settings.
            if (Mod.Settings.SelectionShape !=
                AreaBulldozerSelectionShape.Polyline)
            {
                DeactivateBecauseShapeChanged();
            }
        }

        private void SetFreeAreaPolygonMode(
            bool enabled)
        {
            if (enabled)
            {
                EnableFreeAreaPolygon();
            }
            else
            {
                DisableFreeAreaPolygon(
                    restorePreviousShape: true);
            }
        }

        private void EnableFreeAreaPolygon()
        {
            Setting setting =
                Mod.Settings;

            if (setting == null)
            {
                return;
            }

            if (!FreeAreaPolygonModeState.Active)
            {
                m_PreviousShape =
                    setting.SelectionShape;

                m_PreviousUseSquareBrush =
                    setting.UseSquareBrush;

                m_PreviousLineWidth =
                    setting.LineWidth;

                m_PreviousCurvedPolyline =
                    setting.UseCurvedPolyline;

                m_HasSavedState = true;
            }

            FreeAreaPolygonModeState.Active =
                true;

            // Reuse the stable polyline selection path internally.
            setting.SelectionShape =
                AreaBulldozerSelectionShape.Polyline;

            setting.UseSquareBrush =
                false;

            // Keep the inherited corridor rendering as unobtrusive as
            // possible. The dedicated polygon overlay draws the real shape.
            setting.LineWidth = 2;
            setting.UseCurvedPolyline = false;

            m_Tool?.
                NotifySelectionShapeChanged();

            m_Tool?.
                ResetFreeAreaPolygonRuntimeState();

            Mod.LogDiagnosticInfo(
                "Free-area polygon mode enabled.");
        }

        private void DisableFreeAreaPolygon(
            bool restorePreviousShape)
        {
            Setting setting =
                Mod.Settings;

            bool wasActive =
                FreeAreaPolygonModeState.Active;

            FreeAreaPolygonModeState.Active =
                false;

            if (setting != null &&
                m_HasSavedState)
            {
                setting.LineWidth =
                    m_PreviousLineWidth;

                setting.UseCurvedPolyline =
                    m_PreviousCurvedPolyline;

                if (restorePreviousShape)
                {
                    setting.SelectionShape =
                        m_PreviousShape;

                    setting.UseSquareBrush =
                        m_PreviousUseSquareBrush;
                }
            }

            m_HasSavedState = false;

            if (wasActive)
            {
                m_Tool?.
                    ResetFreeAreaPolygonRuntimeState();

                m_Tool?.
                    NotifySelectionShapeChanged();

                Mod.LogDiagnosticInfo(
                    "Free-area polygon mode disabled.");
            }
        }

        private void DeactivateBecauseShapeChanged()
        {
            Setting setting =
                Mod.Settings;

            if (setting == null)
            {
                FreeAreaPolygonModeState.Active =
                    false;
                return;
            }

            AreaBulldozerSelectionShape selectedShape =
                setting.SelectionShape;

            bool selectedUseSquare =
                setting.UseSquareBrush;

            DisableFreeAreaPolygon(
                restorePreviousShape: false);

            // DisableFreeAreaPolygon restores only temporary rendering
            // settings in this path. Preserve the user's newly chosen shape.
            setting.SelectionShape =
                selectedShape;

            setting.UseSquareBrush =
                selectedUseSquare;

            m_Tool?.
                NotifySelectionShapeChanged();
        }

        protected override void OnDestroy()
        {
            DisableFreeAreaPolygon(
                restorePreviousShape: true);

            m_Tool = null;

            base.OnDestroy();
        }
    }
}
