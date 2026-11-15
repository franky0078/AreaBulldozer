using AreaBulldozer.Tools;
using Colossal.UI.Binding;
using Game;
using Game.UI;
using Unity.Mathematics;

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

        internal FilterProfile.BrushSettings CaptureProfileBrush(Setting setting)
        {
            bool polygon = m_Tool.UseFreeAreaPolygon;
            bool hasPreviousBrush = polygon && m_HasSavedState;
            AreaBulldozerSelectionShape shape = hasPreviousBrush ? m_PreviousShape : m_Tool.CurrentSelectionShape;
            shape = shape switch
            {
                AreaBulldozerSelectionShape.Circle when hasPreviousBrush && m_PreviousUseSquareBrush
                    => AreaBulldozerSelectionShape.Square,
                AreaBulldozerSelectionShape.Square => shape,
                AreaBulldozerSelectionShape.Triangle => shape,
                AreaBulldozerSelectionShape.Polyline or AreaBulldozerSelectionShape.LegacyLine
                    => AreaBulldozerSelectionShape.Polyline,
                _ => AreaBulldozerSelectionShape.Circle
            };

            float degrees = m_Tool.SquareRotationDegrees;
            int rotation = math.isfinite(degrees) ? ((int)math.round(degrees * 10f) % 3600 + 3600) % 3600 : 0;
            return new FilterProfile.BrushSettings(shape, math.clamp(setting.BrushRadius, 5, 200),
                math.clamp(hasPreviousBrush ? m_PreviousLineWidth : setting.LineWidth, 2, 100),
                m_Tool.CurrentPolylineRounding, rotation,
                hasPreviousBrush ? m_PreviousCurvedPolyline : setting.UseCurvedPolyline, polygon);
        }

        internal void ApplyProfileBrush(Setting setting, FilterProfile.BrushSettings brush)
        {
            // Restore temporary polygon values before applying the new profile.
            DisableFreeAreaPolygon(restorePreviousShape: true);
            setting.SelectionShape = brush.Shape;
            setting.UseSquareBrush = brush.Shape == AreaBulldozerSelectionShape.Square;
            setting.BrushRadius = brush.Radius;
            setting.LineWidth = brush.LineWidth;
            setting.UseCurvedPolyline = brush.Curved;
            setting.PolylineRounding = brush.Rounding;
            m_Tool.SetSquareRotationDegrees(brush.RotationTenths / 10f);

            if (brush.Polygon)
            {
                EnableFreeAreaPolygon();
            }
            else
            {
                m_Tool.NotifySelectionShapeChanged();
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
