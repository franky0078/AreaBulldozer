using AreaBulldozer.Tools;
using Colossal.IO.AssetDatabase;
using Colossal.UI.Binding;
using Game;
using Game.UI;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace AreaBulldozer.UISystems
{
    public partial class AreaBulldozerUISystem : UISystemBase
    {
        private const float kSaveDelaySeconds = 0.5f;

        private AreaBulldozerToolSystem m_Tool;
        private bool m_SettingsDirty;
        private float m_SaveSettingsAt;

        public override GameMode gameMode =>
            GameMode.GameOrEditor;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_Tool =
                World.GetOrCreateSystemManaged<
                    AreaBulldozerToolSystem>();

            RegisterValueBindings();
            RegisterTriggerBindings();

            Mod.LogDiagnosticInfo(
                "Area Bulldozer UI bindings initialized.");
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (!m_SettingsDirty ||
                UnityEngine.Time.unscaledTime < m_SaveSettingsAt)
            {
                return;
            }

            m_SettingsDirty = false;

            try
            {
                _ = AssetDatabase.global.SaveSettings();
            }
            catch (Exception exception)
            {
                Mod.Log.Warn(
                    $"Area Bulldozer settings could not be saved: " +
                    $"{exception.Message}");
            }
        }

        protected override void OnDestroy()
        {
            if (m_SettingsDirty)
            {
                try
                {
                    _ = AssetDatabase.global.SaveSettings();
                }
                catch (Exception exception)
                {
                    Mod.Log.Warn(
                        $"Area Bulldozer settings could not be saved " +
                        $"during shutdown: {exception.Message}");
                }
            }

            m_Tool?.SetPointerOverUI(false);
            m_Tool = null;

            base.OnDestroy();
        }

        private void RegisterValueBindings()
        {
            string group =
                AreaBulldozerUIBindingConstants.ModId;

            AddUpdateBinding(
                new GetterValueBinding<bool>(
                    group,
                    AreaBulldozerUIBindingConstants.IsToolActive,
                    () => m_Tool != null && m_Tool.IsToolActive));

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.BrushRadius,
                    () => Mod.Settings?.BrushRadius ?? 30));

            AddUpdateBinding(
                new GetterValueBinding<bool>(
                    group,
                    AreaBulldozerUIBindingConstants.UseSquareBrush,
                    () => m_Tool?.UseSquareBrush ??
                          (Mod.Settings?.UseSquareBrush ?? false)));

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SelectionShape,
                    () => (int)(
                        m_Tool?.CurrentSelectionShape ??
                        AreaBulldozerSelectionShape.Circle)));

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.LineWidth,
                    () => math.clamp(
                        Mod.Settings?.LineWidth ?? 10,
                        2,
                        100)));

            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.UseCurvedPolyline,
                () => Mod.Settings?.UseCurvedPolyline ?? false);

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.PolylineRounding,
                    () => math.clamp(
                        Mod.Settings?.PolylineRounding ?? 50,
                        10,
                        100)));

            AddUpdateBinding(
                new GetterValueBinding<float>(
                    group,
                    AreaBulldozerUIBindingConstants.SquareRotationDegrees,
                    () => m_Tool?.SquareRotationDegrees ?? 0f));

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.UIScale,
                    () => math.clamp(Mod.Settings?.UIScale ?? 100, 75, 125)));

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.LauncherMode,
                    () => (int)(
                        Mod.Settings?.LauncherMode ??
                        AreaBulldozerLauncherMode.Standalone)));

            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.UseUniversalModMenu,
                () =>
                    (Mod.Settings?.LauncherMode ??
                     AreaBulldozerLauncherMode.Standalone) ==
                    AreaBulldozerLauncherMode.UniversalModMenu);

            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.LauncherButtonMovable,
                () => Mod.Settings?.LauncherButtonMovable ?? false);

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.LauncherPositionX,
                    () => math.clamp(
                        Mod.Settings?.LauncherPositionX ?? 54,
                        0,
                        8192)));

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.LauncherPositionY,
                    () => math.clamp(
                        Mod.Settings?.LauncherPositionY ?? 8,
                        0,
                        8192)));

            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteTrees,
                () => Mod.Settings?.DeleteTrees ?? true);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteBuildings,
                () => Mod.Settings?.DeleteBuildings ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteRoads,
                () => Mod.Settings?.DeleteRoads ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeletePaths,
                () => Mod.Settings?.DeletePaths ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteRailways,
                () => Mod.Settings?.DeleteRailways ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteSurfaces,
                () => Mod.Settings?.DeleteSurfaces ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteStaticObjects,
                () => Mod.Settings?.DeleteStaticObjects ?? false);

            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteGeneralProps,
                () => Mod.Settings?.DeleteGeneralProps ?? true);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteStreetLights,
                () => Mod.Settings?.DeleteStreetLights ?? true);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteQuantityObjects,
                () => Mod.Settings?.DeleteQuantityObjects ?? true);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteBrandingObjects,
                () => Mod.Settings?.DeleteBrandingObjects ?? true);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteActivityLocations,
                () => Mod.Settings?.DeleteActivityLocations ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteSpawnLocations,
                () => Mod.Settings?.DeleteSpawnLocations ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteMarkerNetworks,
                () => Mod.Settings?.DeleteMarkerNetworks ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DimMarkerBackground,
                () => Mod.Settings?.DimMarkerBackground ?? true);

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.MarkerBackgroundDarkness,
                    () => Mod.Settings?.MarkerBackgroundDarkness ?? 40));

            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteBuildingSubObjects,
                () => Mod.Settings?.DeleteBuildingSubObjects ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.DeleteNetworkSubObjects,
                () => Mod.Settings?.DeleteNetworkSubObjects ?? false);
            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.ProtectOwnedObjects,
                () => Mod.Settings?.ProtectOwnedObjects ?? true);

            AddBooleanValueBinding(
                AreaBulldozerUIBindingConstants.ConfirmLargeSelection,
                () => Mod.Settings?.ConfirmLargeSelection ?? true);

            AddUpdateBinding(
                new GetterValueBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.LargeSelectionThreshold,
                    () => Mod.Settings?.LargeSelectionThreshold ?? 250));
        }

        private void RegisterTriggerBindings()
        {
            string group =
                AreaBulldozerUIBindingConstants.ModId;

            AddBinding(
                new TriggerBinding(
                    group,
                    AreaBulldozerUIBindingConstants.ToggleTool,
                    () => m_Tool?.ToggleTool()));

            AddBinding(
                new TriggerBinding(
                    group,
                    AreaBulldozerUIBindingConstants.DeactivateTool,
                    () => m_Tool?.DeactivateTool()));

            AddBinding(
                new TriggerBinding<bool>(
                    group,
                    AreaBulldozerUIBindingConstants.SetPointerOverUI,
                    isPointerOverUI =>
                        m_Tool?.SetPointerOverUI(isPointerOverUI)));

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetBrushRadius,
                    value =>
                    {
                        ChangeSetting(
                            setting => setting.BrushRadius =
                                math.clamp(value, 5, 200));

                        m_Tool?.InvalidateSelectionGeometry();
                    }));

            // Compatibility trigger used by older UI builds.
            AddBinding(
                new TriggerBinding<bool>(
                    group,
                    AreaBulldozerUIBindingConstants.SetUseSquareBrush,
                    value => SetSelectionShape(
                        value
                            ? AreaBulldozerSelectionShape.Square
                            : AreaBulldozerSelectionShape.Circle)));

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetSelectionShape,
                    value => SetSelectionShape(
                        (AreaBulldozerSelectionShape)
                        math.clamp(value, 0, 4))));

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetLineWidth,
                    value =>
                    {
                        ChangeSetting(
                            setting => setting.LineWidth =
                                math.clamp(value, 2, 100));

                        m_Tool?.InvalidateSelectionGeometry();
                    }));

            AddBinding(
                new TriggerBinding<bool>(
                    group,
                    AreaBulldozerUIBindingConstants.SetUseCurvedPolyline,
                    value =>
                    {
                        ChangeSetting(
                            setting => setting.UseCurvedPolyline = value);

                        m_Tool?.InvalidateSelectionGeometry();
                    }));

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetPolylineRounding,
                    value =>
                    {
                        ChangeSetting(
                            setting => setting.PolylineRounding =
                                math.clamp(value, 10, 100));

                        m_Tool?.InvalidateSelectionGeometry();
                    }));

            AddBinding(
                new TriggerBinding<float>(
                    group,
                    AreaBulldozerUIBindingConstants.SetSquareRotationDegrees,
                    value => m_Tool?.SetSquareRotationDegrees(value)));

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetLauncherPositionX,
                    value => ChangeSetting(
                        setting => setting.LauncherPositionX =
                            math.clamp(value, 0, 8192))));

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetLauncherPositionY,
                    value => ChangeSetting(
                        setting => setting.LauncherPositionY =
                            math.clamp(value, 0, 8192))));

            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteTrees,
                (setting, value) => setting.DeleteTrees = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteBuildings,
                (setting, value) => setting.DeleteBuildings = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteRoads,
                (setting, value) => setting.DeleteRoads = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeletePaths,
                (setting, value) => setting.DeletePaths = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteRailways,
                (setting, value) => setting.DeleteRailways = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteSurfaces,
                (setting, value) => setting.DeleteSurfaces = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteStaticObjects,
                (setting, value) => setting.DeleteStaticObjects = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteGeneralProps,
                (setting, value) => setting.DeleteGeneralProps = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteStreetLights,
                (setting, value) => setting.DeleteStreetLights = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteQuantityObjects,
                (setting, value) => setting.DeleteQuantityObjects = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteBrandingObjects,
                (setting, value) => setting.DeleteBrandingObjects = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteActivityLocations,
                (setting, value) => setting.DeleteActivityLocations = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteSpawnLocations,
                (setting, value) => setting.DeleteSpawnLocations = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteMarkerNetworks,
                (setting, value) => setting.DeleteMarkerNetworks = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDimMarkerBackground,
                (setting, value) => setting.DimMarkerBackground = value);

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetMarkerBackgroundDarkness,
                    value => ChangeSetting(
                        setting => setting.MarkerBackgroundDarkness =
                            math.clamp(value, 10, 70))));

            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteBuildingSubObjects,
                (setting, value) => setting.DeleteBuildingSubObjects = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetDeleteNetworkSubObjects,
                (setting, value) => setting.DeleteNetworkSubObjects = value);
            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetProtectOwnedObjects,
                (setting, value) => setting.ProtectOwnedObjects = value);

            AddBooleanSettingTrigger(
                AreaBulldozerUIBindingConstants.SetConfirmLargeSelection,
                (setting, value) => setting.ConfirmLargeSelection = value);

            AddBinding(
                new TriggerBinding<int>(
                    group,
                    AreaBulldozerUIBindingConstants.SetLargeSelectionThreshold,
                    value => ChangeSetting(
                        setting => setting.LargeSelectionThreshold =
                            math.clamp(value, 50, 2000))));
        }

        private void SetSelectionShape(
            AreaBulldozerSelectionShape shape)
        {
            // Older settings may still contain the former line value 3.
            // Redirect it to the multi-point line.
            if (shape == AreaBulldozerSelectionShape.LegacyLine)
            {
                shape = AreaBulldozerSelectionShape.Polyline;
            }

            if (shape != AreaBulldozerSelectionShape.Circle &&
                shape != AreaBulldozerSelectionShape.Square &&
                shape != AreaBulldozerSelectionShape.Triangle &&
                shape != AreaBulldozerSelectionShape.Polyline)
            {
                shape = AreaBulldozerSelectionShape.Circle;
            }

            AreaBulldozerSelectionShape normalizedShape = shape;

            ChangeSetting(
                setting =>
                {
                    setting.SelectionShape = normalizedShape;
                    setting.UseSquareBrush =
                        normalizedShape == AreaBulldozerSelectionShape.Square;
                });

            m_Tool?.NotifySelectionShapeChanged();
        }

        private void AddBooleanValueBinding(
            string key,
            Func<bool> getter)
        {
            AddUpdateBinding(
                new GetterValueBinding<bool>(
                    AreaBulldozerUIBindingConstants.ModId,
                    key,
                    getter));
        }

        private void AddBooleanSettingTrigger(
            string key,
            Action<Setting, bool> change)
        {
            AddBinding(
                new TriggerBinding<bool>(
                    AreaBulldozerUIBindingConstants.ModId,
                    key,
                    value => ChangeSetting(
                        setting => change(setting, value))));
        }

        private void ChangeSetting(
            Action<Setting> change)
        {
            Setting setting = Mod.Settings;

            if (setting == null)
            {
                return;
            }

            change(setting);

            m_SettingsDirty = true;
            m_SaveSettingsAt =
                UnityEngine.Time.unscaledTime + kSaveDelaySeconds;
        }
    }
}
