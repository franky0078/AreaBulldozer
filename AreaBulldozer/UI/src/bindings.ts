import { bindValue } from "cs2/api";
import mod from "mod.json";

export const BindingKeys = {
    toggleTool: "toggleTool",
    deactivateTool: "deactivateTool",
    setPointerOverUI: "setPointerOverUI",
    setBrushRadius: "setBrushRadius",
    setUseSquareBrush: "setUseSquareBrush",
    setSquareRotationDegrees: "setSquareRotationDegrees",
    setLauncherPositionX: "setLauncherPositionX",
    setLauncherPositionY: "setLauncherPositionY",

    setDeleteTrees: "setDeleteTrees",
    setDeleteBuildings: "setDeleteBuildings",
    setDeleteRoads: "setDeleteRoads",
    setDeletePaths: "setDeletePaths",
    setDeleteRailways: "setDeleteRailways",
    setDeleteSurfaces: "setDeleteSurfaces",
    setDeleteStaticObjects: "setDeleteStaticObjects",

    setDeleteGeneralProps: "setDeleteGeneralProps",
    setDeleteStreetLights: "setDeleteStreetLights",
    setDeleteQuantityObjects: "setDeleteQuantityObjects",
    setDeleteBrandingObjects: "setDeleteBrandingObjects",
    setDeleteActivityLocations: "setDeleteActivityLocations",
    setDeleteSpawnLocations: "setDeleteSpawnLocations",
    setDeleteMarkerNetworks: "setDeleteMarkerNetworks",
    setDimMarkerBackground: "setDimMarkerBackground",
    setMarkerBackgroundDarkness: "setMarkerBackgroundDarkness",

    setDeleteBuildingSubObjects: "setDeleteBuildingSubObjects",
    setDeleteNetworkSubObjects: "setDeleteNetworkSubObjects",
    setProtectOwnedObjects: "setProtectOwnedObjects",

    setConfirmLargeSelection: "setConfirmLargeSelection",
    setLargeSelectionThreshold: "setLargeSelectionThreshold",
} as const;

export const isToolActive$ = bindValue<boolean>(
    mod.id,
    "isToolActive",
    false
);

export const brushRadius$ = bindValue<number>(mod.id, "brushRadius", 30);

export const useSquareBrush$ = bindValue<boolean>(
    mod.id,
    "useSquareBrush",
    false
);

export const squareRotationDegrees$ = bindValue<number>(
    mod.id,
    "squareRotationDegrees",
    0
);

export const uiScale$ = bindValue<number>(
    mod.id,
    "uiScale",
    100
);

export const launcherMode$ = bindValue<number>(
    mod.id,
    "launcherMode",
    0
);

// Legacy-Binding für die bereits vorhandenen Launcher-Komponenten.
export const useUniversalModMenu$ = bindValue<boolean>(
    mod.id,
    "useUniversalModMenu",
    false
);

export const launcherButtonMovable$ = bindValue<boolean>(
    mod.id,
    "launcherButtonMovable",
    false
);

export const launcherPositionX$ = bindValue<number>(
    mod.id,
    "launcherPositionX",
    54
);

export const launcherPositionY$ = bindValue<number>(
    mod.id,
    "launcherPositionY",
    8
);

export const deleteTrees$ = bindValue<boolean>(
    mod.id,
    "deleteTrees",
    true
);

export const deleteBuildings$ = bindValue<boolean>(
    mod.id,
    "deleteBuildings",
    false
);

export const deleteRoads$ = bindValue<boolean>(
    mod.id,
    "deleteRoads",
    false
);

export const deletePaths$ = bindValue<boolean>(
    mod.id,
    "deletePaths",
    false
);

export const deleteRailways$ = bindValue<boolean>(
    mod.id,
    "deleteRailways",
    false
);

export const deleteSurfaces$ = bindValue<boolean>(
    mod.id,
    "deleteSurfaces",
    false
);

export const deleteStaticObjects$ = bindValue<boolean>(
    mod.id,
    "deleteStaticObjects",
    false
);

export const deleteGeneralProps$ = bindValue<boolean>(
    mod.id,
    "deleteGeneralProps",
    true
);

export const deleteStreetLights$ = bindValue<boolean>(
    mod.id,
    "deleteStreetLights",
    true
);

export const deleteQuantityObjects$ = bindValue<boolean>(
    mod.id,
    "deleteQuantityObjects",
    true
);

export const deleteBrandingObjects$ = bindValue<boolean>(
    mod.id,
    "deleteBrandingObjects",
    true
);

export const deleteActivityLocations$ = bindValue<boolean>(
    mod.id,
    "deleteActivityLocations",
    false
);

export const deleteSpawnLocations$ = bindValue<boolean>(
    mod.id,
    "deleteSpawnLocations",
    false
);

export const deleteMarkerNetworks$ = bindValue<boolean>(
    mod.id,
    "deleteMarkerNetworks",
    false
);

export const dimMarkerBackground$ = bindValue<boolean>(
    mod.id,
    "dimMarkerBackground",
    true
);

export const markerBackgroundDarkness$ = bindValue<number>(
    mod.id,
    "markerBackgroundDarkness",
    40
);

export const deleteBuildingSubObjects$ = bindValue<boolean>(
    mod.id,
    "deleteBuildingSubObjects",
    false
);

export const deleteNetworkSubObjects$ = bindValue<boolean>(
    mod.id,
    "deleteNetworkSubObjects",
    false
);

export const protectOwnedObjects$ = bindValue<boolean>(
    mod.id,
    "protectOwnedObjects",
    true
);

export const confirmLargeSelection$ = bindValue<boolean>(
    mod.id,
    "confirmLargeSelection",
    true
);

export const largeSelectionThreshold$ = bindValue<number>(
    mod.id,
    "largeSelectionThreshold",
    250
);
