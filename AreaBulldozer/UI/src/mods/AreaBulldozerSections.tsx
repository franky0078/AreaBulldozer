import React, { useEffect } from "react";
import { ModuleRegistryExtend } from "cs2/modding";
import { trigger } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "mod.json";
import styles from "./AreaBulldozerCompact.module.scss";
import { VanillaComponentResolver } from "./VanillaComponentResolver";
import { AreaBulldozerIcon, AreaBulldozerIconType } from "./AreaBulldozerIcons";
import { useSafeValue } from "./useSafeValue";
import {
    BindingKeys,
    brushRadius$,
    deleteActivityLocations$,
    deleteBrandingObjects$,
    deleteBuildings$,
    deleteGeneralProps$,
    deleteMarkerNetworks$,
    deletePaths$,
    deleteQuantityObjects$,
    deleteRailways$,
    deleteRoads$,
    deleteSpawnLocations$,
    deleteStaticObjects$,
    deleteStreetLights$,
    deleteSurfaces$,
    deleteTrees$,
    dimMarkerBackground$,
    isToolActive$,
    squareRotationDegrees$,
    uiScale$,
    useSquareBrush$,
} from "../bindings";

const UI_PREFIX = "AreaBulldozer.UI.";

const RADIUS_MIN = 5;
const RADIUS_MAX = 200;
const RADIUS_STEP = 5;

const ROTATION_STEP = 15;

const BASE_BUTTON = 32;
const BASE_ICON = 21;
const BASE_FIELD = 50;
const BASE_FONT = 12;

const ScaleContext = React.createContext(1);

function clamp(value: number, min: number, max: number) {
    return Math.min(max, Math.max(min, value));
}

/* -------------------------------------------------------------------------
 * Bausteine
 * ---------------------------------------------------------------------- */

interface IconToolButtonProps {
    icon: AreaBulldozerIconType;
    selected?: boolean;
    disabled?: boolean;
    //  Props-/Marker-Detailfilter Farbe.
    marker?: boolean;
    tooltipTitle: string;
    tooltipText?: string;
    onSelect: () => void;
}

function IconToolButton({
    icon,
    selected = false,
    disabled = false,
    marker = false,
    tooltipTitle,
    tooltipText,
    onSelect,
}: IconToolButtonProps) {
    const resolver = VanillaComponentResolver.instance;
    const theme = resolver.toolButtonTheme;
    const tooltipTheme = resolver.descriptionTooltipTheme;
    const Tooltip = resolver.Tooltip;
    const scale = React.useContext(ScaleContext);


    const buttonStyle: React.CSSProperties = {
        width: `${(BASE_BUTTON * scale).toFixed(1)}rem`,
        height: `${(BASE_BUTTON * scale).toFixed(1)}rem`,
    };

    const iconStyle: React.CSSProperties = {
        width: `${(BASE_ICON * scale).toFixed(1)}rem`,
        height: `${(BASE_ICON * scale).toFixed(1)}rem`,
    };

    const className = [
        theme?.button ?? "",
        styles.iconButton,
        selected
            ? marker
                ? styles.iconButtonMarkerSelected
                : styles.iconButtonSelected
            : "",
        disabled ? styles.iconButtonDisabled : "",
    ]
        .filter(Boolean)
        .join(" ");

    const button = (
        <button
            type="button"
            className={className}
            style={buttonStyle}
            aria-pressed={selected}
            disabled={disabled}
            onClick={(event) => {
                event.preventDefault();
                event.stopPropagation();
                if (!disabled) {
                    onSelect();
                }
            }}
        >
            <span className={styles.iconGraphic} style={iconStyle}>
                <AreaBulldozerIcon type={icon} />
            </span>
        </button>
    );

    if (!Tooltip) {
        return button;
    }

    const tooltipContent = (
        <>
            <div className={tooltipTheme?.title}>{tooltipTitle}</div>
            {tooltipText ? (
                <div className={tooltipTheme?.content}>{tooltipText}</div>
            ) : null}
        </>
    );

    return <Tooltip tooltip={tooltipContent}>{button}</Tooltip>;
}

interface StepperProps {
    value: string;
    narrow?: boolean;
    decreaseTooltip: string;
    increaseTooltip: string;
    onDecrease: () => void;
    onIncrease: () => void;
}

function Stepper({
    value,
    narrow = false,
    decreaseTooltip,
    increaseTooltip,
    onDecrease,
    onIncrease,
}: StepperProps) {
    const numberFieldClass =
        VanillaComponentResolver.instance.mouseToolOptionsTheme?.numberField;
    const scale = React.useContext(ScaleContext);

    const fieldStyle: React.CSSProperties = {
        minWidth: `${(BASE_FIELD * (narrow ? 0.85 : 1) * scale).toFixed(1)}rem`,
        height: `${(BASE_BUTTON * scale).toFixed(1)}rem`,
        fontSize: `${(BASE_FONT * scale).toFixed(1)}rem`,
    };

    return (
        <>
            <IconToolButton
                icon="minus"
                tooltipTitle={decreaseTooltip}
                onSelect={onDecrease}
            />
            <div
                className={[
                    numberFieldClass ?? "",
                    styles.valueField,
                    narrow ? styles.valueFieldNarrow : "",
                ]
                    .filter(Boolean)
                    .join(" ")}
                style={fieldStyle}
            >
                {value}
            </div>
            <IconToolButton
                icon="plus"
                tooltipTitle={increaseTooltip}
                onSelect={onIncrease}
            />
        </>
    );
}

/* -------------------------------------------------------------------------
 * Hauptkomponente
 * ---------------------------------------------------------------------- */

export const AreaBulldozerSections: ModuleRegistryExtend = (Component: any) => {
    return (props: any) => {
        const resolver = VanillaComponentResolver.instance;
        const Section = resolver.Section;

        const isToolActive = useSafeValue(
            "isToolActive$",
            isToolActive$,
            false
        );
        const brushRadius = useSafeValue(
            "brushRadius$",
            brushRadius$,
            30
        );
        const useSquareBrush = useSafeValue(
            "useSquareBrush$",
            useSquareBrush$,
            false
        );
        const squareRotationDegrees = useSafeValue(
            "squareRotationDegrees$",
            squareRotationDegrees$,
            0
        );
        const uiScalePercent = useSafeValue("uiScale$", uiScale$, 100);

        const deleteTrees = useSafeValue(
            "deleteTrees$",
            deleteTrees$,
            false
        );
        const deleteBuildings = useSafeValue(
            "deleteBuildings$",
            deleteBuildings$,
            false
        );
        const deleteRoads = useSafeValue(
            "deleteRoads$",
            deleteRoads$,
            false
        );
        const deletePaths = useSafeValue(
            "deletePaths$",
            deletePaths$,
            false
        );
        const deleteRailways = useSafeValue(
            "deleteRailways$",
            deleteRailways$,
            false
        );
        const deleteSurfaces = useSafeValue(
            "deleteSurfaces$",
            deleteSurfaces$,
            false
        );
        const deleteStaticObjects = useSafeValue(
            "deleteStaticObjects$",
            deleteStaticObjects$,
            false
        );

        const deleteGeneralProps = useSafeValue(
            "deleteGeneralProps$",
            deleteGeneralProps$,
            false
        );
        const deleteStreetLights = useSafeValue(
            "deleteStreetLights$",
            deleteStreetLights$,
            false
        );
        const deleteQuantityObjects = useSafeValue(
            "deleteQuantityObjects$",
            deleteQuantityObjects$,
            false
        );
        const deleteBrandingObjects = useSafeValue(
            "deleteBrandingObjects$",
            deleteBrandingObjects$,
            false
        );
        const deleteActivityLocations = useSafeValue(
            "deleteActivityLocations$",
            deleteActivityLocations$,
            false
        );
        const deleteSpawnLocations = useSafeValue(
            "deleteSpawnLocations$",
            deleteSpawnLocations$,
            false
        );
        const deleteMarkerNetworks = useSafeValue(
            "deleteMarkerNetworks$",
            deleteMarkerNetworks$,
            false
        );
        const dimMarkerBackground = useSafeValue(
            "dimMarkerBackground$",
            dimMarkerBackground$,
            false
        );

        const { translate } = useLocalization();
        const text = (key: string, fallback: string) =>
            translate(`${UI_PREFIX}${key}`, fallback) ?? fallback;

        useEffect(() => {
            if (!isToolActive) {
                return;
            }

            console.log(
                "[AreaBulldozer] Sections rendern. " +
                `Section=${typeof Section} ` +
                `ToolButtonTheme=${resolver.toolButtonTheme ? "ok" : "FEHLT"} ` +
                `MouseToolOptionsTheme=${resolver.mouseToolOptionsTheme ? "ok" : "FEHLT"} ` +
                `Tooltip=${typeof resolver.Tooltip}`
            );
        }, [isToolActive]);

        let result: JSX.Element | null = null;

        try {
            result = Component();
        } catch (error) {
            console.error(
                "[AreaBulldozer] MouseToolOptions (Original) hat geworfen.",
                error
            );
            return null;
        }

        if (!isToolActive) {
            return result;
        }

        if (result === null || result === undefined) {
            console.log(
                "[AreaBulldozer] MouseToolOptions lieferte nichts - eigener Rahmen wird gebaut."
            );
        }

        if (typeof Section !== "function") {
            console.error(
                "[AreaBulldozer] Vanilla-Section nicht verfügbar. Sections werden übersprungen."
            );
            return result;
        }

        const scale = clamp(uiScalePercent, 50, 125) / 100;

        const setRadius = (next: number) =>
            trigger(
                mod.id,
                BindingKeys.setBrushRadius,
                clamp(next, RADIUS_MIN, RADIUS_MAX)
            );

        const setRotation = (next: number) =>
            trigger(mod.id, BindingKeys.setSquareRotationDegrees, ((next % 360) + 360) % 360);

        const toggle = (key: string, current: boolean) =>
            trigger(mod.id, key, !current);

        const sections = (
            <ScaleContext.Provider value={scale}>
                <Section title={text("Selection", "Auswahl")}>
                    <IconToolButton
                        icon="circle"
                        selected={!useSquareBrush}
                        tooltipTitle={text("Circle", "Kreis")}
                        tooltipText={text(
                            "CircleTooltip",
                            "Runde Auswahlfläche um den Mauszeiger."
                        )}
                        onSelect={() => trigger(mod.id, BindingKeys.setUseSquareBrush, false)}
                    />
                    <IconToolButton
                        icon="square"
                        selected={useSquareBrush}
                        tooltipTitle={text("Square", "Quadrat")}
                        tooltipText={text(
                            "SquareTooltip",
                            "Quadratische Auswahlfläche, drehbar mit gehaltener rechter Maustaste."
                        )}
                        onSelect={() => trigger(mod.id, BindingKeys.setUseSquareBrush, true)}
                    />
                </Section>

                <Section
                    title={
                        useSquareBrush
                            ? text("HalfSide", "Größe Quadrat")
                            : text("Radius", "Radius")
                    }
                >
                    <Stepper
                        value={`${brushRadius} m`}
                        decreaseTooltip={text("DecreaseSize", "Auswahl verkleinern")}
                        increaseTooltip={text("IncreaseSize", "Auswahl vergrößern")}
                        onDecrease={() => setRadius(brushRadius - RADIUS_STEP)}
                        onIncrease={() => setRadius(brushRadius + RADIUS_STEP)}
                    />
                </Section>

                {useSquareBrush && (
                    <Section title={text("Rotation", "Drehung")}>
                        <Stepper
                            narrow
                            value={`${Math.round(squareRotationDegrees)}°`}
                            decreaseTooltip={text("RotateLeft", "Gegen den Uhrzeigersinn drehen")}
                            increaseTooltip={text("RotateRight", "Im Uhrzeigersinn drehen")}
                            onDecrease={() => setRotation(squareRotationDegrees - ROTATION_STEP)}
                            onIncrease={() => setRotation(squareRotationDegrees + ROTATION_STEP)}
                        />
                    </Section>
                )}

                <Section title={text("MainFilters", "Filter")}>
                    <IconToolButton
                        icon="vegetation"
                        selected={deleteTrees}
                        tooltipTitle={text("VegetationShort", "Vegetation")}
                        tooltipText={text(
                            "VegetationTooltip",
                            "Bäume, Büsche und Pflanzen löschen."
                        )}
                        onSelect={() => toggle(BindingKeys.setDeleteTrees, deleteTrees)}
                    />
                    <IconToolButton
                        icon="building"
                        selected={deleteBuildings}
                        tooltipTitle={text("Buildings", "Gebäude")}
                        tooltipText={text("BuildingsTooltip", "Gebäude im Bereich löschen.")}
                        onSelect={() => toggle(BindingKeys.setDeleteBuildings, deleteBuildings)}
                    />
                    <IconToolButton
                        icon="road"
                        selected={deleteRoads}
                        tooltipTitle={text("Roads", "Straßen")}
                        tooltipText={text("RoadsTooltip", "Straßen im Bereich löschen.")}
                        onSelect={() => toggle(BindingKeys.setDeleteRoads, deleteRoads)}
                    />
                    <IconToolButton
                        icon="path"
                        selected={deletePaths}
                        tooltipTitle={text("Paths", "Fußwege")}
                        tooltipText={text("PathsTooltip", "Fuß- und Radwege löschen.")}
                        onSelect={() => toggle(BindingKeys.setDeletePaths, deletePaths)}
                    />
                    <IconToolButton
                        icon="rail"
                        selected={deleteRailways}
                        tooltipTitle={text("Railways", "Gleise")}
                        tooltipText={text("RailwaysTooltip", "Schienenwege löschen.")}
                        onSelect={() => toggle(BindingKeys.setDeleteRailways, deleteRailways)}
                    />
                    <IconToolButton
                        icon="surface"
                        selected={deleteSurfaces}
                        tooltipTitle={text("SurfacesShort", "Flächen")}
                        tooltipText={text("SurfacesTooltip", "Oberflächen und Bereiche löschen.")}
                        onSelect={() => toggle(BindingKeys.setDeleteSurfaces, deleteSurfaces)}
                    />
                    <IconToolButton
                        icon="props"
                        selected={deleteStaticObjects}
                        tooltipTitle={text("StaticObjectsShort", "Props / Marker")}
                        tooltipText={text(
                            "StaticObjectsTooltip",
                            "Hauptschalter für Props und Marker. Aktiviert die Detailfilter darunter."
                        )}
                        onSelect={() =>
                            toggle(BindingKeys.setDeleteStaticObjects, deleteStaticObjects)
                        }
                    />
                </Section>

                {deleteStaticObjects && (
                    <Section title={text("AdvancedFiltersShort", "Props / Marker")}>
                        <IconToolButton
                            marker
                            icon="generalProps"
                            selected={deleteGeneralProps}
                            tooltipTitle={text("GeneralProps", "Allgemeine Props")}
                            onSelect={() =>
                                toggle(BindingKeys.setDeleteGeneralProps, deleteGeneralProps)
                            }
                        />
                        <IconToolButton
                            marker
                            icon="streetLight"
                            selected={deleteStreetLights}
                            tooltipTitle={text("StreetLights", "Straßenlaternen")}
                            onSelect={() =>
                                toggle(BindingKeys.setDeleteStreetLights, deleteStreetLights)
                            }
                        />
                        <IconToolButton
                            marker
                            icon="quantity"
                            selected={deleteQuantityObjects}
                            tooltipTitle={text("QuantityObjects", "Mülleimer und Mengenobjekte")}
                            onSelect={() =>
                                toggle(BindingKeys.setDeleteQuantityObjects, deleteQuantityObjects)
                            }
                        />
                        <IconToolButton
                            marker
                            icon="branding"
                            selected={deleteBrandingObjects}
                            tooltipTitle={text("Branding", "Werbung und Branding")}
                            onSelect={() =>
                                toggle(BindingKeys.setDeleteBrandingObjects, deleteBrandingObjects)
                            }
                        />
                        <IconToolButton
                            marker
                            icon="activity"
                            selected={deleteActivityLocations}
                            tooltipTitle={text("ActivityLocations", "Aktivitätspunkte")}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDeleteActivityLocations,
                                    deleteActivityLocations
                                )
                            }
                        />
                        <IconToolButton
                            marker
                            icon="spawn"
                            selected={deleteSpawnLocations}
                            tooltipTitle={text("SpawnLocations", "Spawnpunkte")}
                            onSelect={() =>
                                toggle(BindingKeys.setDeleteSpawnLocations, deleteSpawnLocations)
                            }
                        />
                        <IconToolButton
                            marker
                            icon="lanes"
                            selected={deleteMarkerNetworks}
                            tooltipTitle={text("AssetLanes", "Asset-Lanes / SubLanes")}
                            onSelect={() =>
                                toggle(BindingKeys.setDeleteMarkerNetworks, deleteMarkerNetworks)
                            }
                        />
                        <IconToolButton
                            marker
                            icon="dim"
                            selected={dimMarkerBackground}
                            tooltipTitle={text("DimBackground", "Marker-Hintergrund abdunkeln")}
                            onSelect={() =>
                                toggle(BindingKeys.setDimMarkerBackground, dimMarkerBackground)
                            }
                        />
                    </Section>
                )}
            </ScaleContext.Provider>
        );

        try {
            if (!result) {
                return (
                    <div className={resolver.mouseToolOptionsTheme?.mouseToolOptions}>
                        {sections}
                    </div>
                );
            }

            if (Array.isArray(result.props?.children)) {
                result.props.children.push(sections);
                return result;
            }

            return React.cloneElement(
                result,
                {},
                ...React.Children.toArray(result.props?.children),
                sections
            );
        } catch (error) {
            console.error(
                "[AreaBulldozer] Sections konnten nicht eingehängt werden.",
                error
            );
            return result;
        }
    };
};
