import React, { useEffect } from "react";
import { ModuleRegistryExtend } from "cs2/modding";
import { bindValue, trigger } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "mod.json";
import styles from "./AreaBulldozerCompact.module.scss";
import { VanillaComponentResolver } from "./VanillaComponentResolver";
import { AreaBulldozerIcon, AreaBulldozerIconType } from "./AreaBulldozerIcons";
import { useSafeValue } from "./useSafeValue";
import {
    BindingKeys,
    heightFilter$,
    undergroundView$,
    brushRadius$,
    deleteActivityLocations$,
    deleteBrandingObjects$,
    deleteBuildings$,
    deleteCars$,
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
    deleteMissingAssets$,
    deleteTrees$,
    treeAgeFilter$,
    protectAssetVegetation$,
    dimMarkerBackground$,
    isToolActive$,
    launcherMode$,
    lineWidth$,
    polylineRounding$,
    selectionShape$,
    useCurvedPolyline$,
    squareRotationDegrees$,
    uiScale$,
    useCompactToolPanel$,
    vegetationUndoCount$,
} from "../bindings";
import {
    toggleToolWindow,
    useToolWindow,
} from "./AreaBulldozerColorEditorState";

const UI_PREFIX = "AreaBulldozer.UI.";

const SHAPE_CIRCLE = 0;
const SHAPE_SQUARE = 1;
const SHAPE_TRIANGLE = 2;
const SHAPE_POLYLINE = 4;

const VANILLA_BULLDOZER_LAUNCHER_MODE = 1;
const MAIN_FILTER_COLUMNS = 8;

const freeAreaPolygon$ = bindValue<boolean>(
    mod.id,
    "freeAreaPolygon",
    false
);

const freeAreaPolygonPointCount$ = bindValue<number>(
    mod.id,
    "freeAreaPolygonPointCount",
    0
);

const RADIUS_MIN = 5;
const RADIUS_MAX = 200;
const RADIUS_STEP = 5;

const LINE_WIDTH_MIN = 2;
const LINE_WIDTH_MAX = 100;
const LINE_WIDTH_STEP = 2;

const POLYLINE_ROUNDING_MIN = 10;
const POLYLINE_ROUNDING_MAX = 100;
const POLYLINE_ROUNDING_STEP = 10;

const ROTATION_STEP = 15;

const BASE_FIELD = 50;

const ToolPanelContext = React.createContext({ scale: 1, compact: false });

function clamp(value: number, min: number, max: number) {
    return Math.min(max, Math.max(min, value));
}

interface IconToolButtonProps {
    icon: AreaBulldozerIconType;
    selected?: boolean;
    disabled?: boolean;
    marker?: boolean;
    tooltipTitle: string;
    tooltipText?: string;
    onSelect: () => void;
}

function iconButtonSize(compact: boolean, scale: number): number {
    return Number(((compact ? 28 : 32) * scale).toFixed(1));
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
    const { scale, compact } = React.useContext(ToolPanelContext);
    const buttonSize = iconButtonSize(compact, scale);

    const buttonStyle: React.CSSProperties = {
        width: `${buttonSize.toFixed(1)}rem`,
        height: `${buttonSize.toFixed(1)}rem`,
    };

    const iconSize = (compact ? 18 : 21) * scale;
    const iconStyle: React.CSSProperties = {
        width: `${iconSize.toFixed(1)}rem`,
        height: `${iconSize.toFixed(1)}rem`,
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
            aria-label={tooltipTitle}
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

export function CompactToolSection({
    title,
    children,
    compact,
}: {
    title: string;
    children: JSX.Element | JSX.Element[];
    compact?: boolean;
}) {
    const Section = VanillaComponentResolver.instance.Section;
    const panel = React.useContext(ToolPanelContext);
    const sectionTitle = (compact ?? panel.compact)
        ? <span className={styles.sectionTitle}>{title}</span>
        : title;

    return (
        <Section title={sectionTitle}>
            {children}
        </Section>
    );
}

interface StepperProps {
    value: string;
    narrow?: boolean;
    wide?: boolean;
    decreaseTooltip: string;
    increaseTooltip: string;
    onDecrease: () => void;
    onIncrease: () => void;
}

function Stepper({
    value,
    narrow = false,
    wide = false,
    decreaseTooltip,
    increaseTooltip,
    onDecrease,
    onIncrease,
}: StepperProps) {
    const numberFieldClass =
        VanillaComponentResolver.instance.mouseToolOptionsTheme?.numberField;

    const { scale, compact } = React.useContext(ToolPanelContext);
    const buttonSize = iconButtonSize(compact, scale);

    const fieldWidth = wide ? 84 : narrow ? BASE_FIELD * 0.85 : BASE_FIELD;
    const fontSize = compact ? 11 : 12;
    const fittedFontSize = wide && value.length > 13 ? fontSize - 1 : fontSize;
    const fieldStyle: React.CSSProperties = {
        minWidth: `${(fieldWidth * scale).toFixed(1)}rem`,
        height: `${buttonSize.toFixed(1)}rem`,
        fontSize: `${(fittedFontSize * scale).toFixed(1)}rem`,
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
                    wide ? styles.valueFieldWide : numberFieldClass ?? "",
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

export const AreaBulldozerSections: ModuleRegistryExtend = (Component: any) => {
    return (props: any) => {
        const resolver = VanillaComponentResolver.instance;
        const Section = resolver.Section;

        const isToolActive = useSafeValue(
            "isToolActive$",
            isToolActive$,
            false
        );

        const launcherMode = useSafeValue(
            "launcherMode$",
            launcherMode$,
            0
        );

        const toolWindow = useToolWindow();
        const colorEditorOpen = toolWindow === "colors";
        const compact = useSafeValue("useCompactToolPanel$", useCompactToolPanel$, false);
        const vegetationUndoCount = useSafeValue("vegetationUndoCount$", vegetationUndoCount$, 0);

        const brushRadius = useSafeValue(
            "brushRadius$",
            brushRadius$,
            30
        );

        const selectionShape = useSafeValue(
            "selectionShape$",
            selectionShape$,
            SHAPE_CIRCLE
        );

        const freeAreaPolygon = useSafeValue(
            "freeAreaPolygon$",
            freeAreaPolygon$,
            false
        );

        const freeAreaPolygonPointCount = useSafeValue(
            "freeAreaPolygonPointCount$",
            freeAreaPolygonPointCount$,
            0
        );

        const lineWidth = useSafeValue(
            "lineWidth$",
            lineWidth$,
            10
        );

        const useCurvedPolyline = useSafeValue(
            "useCurvedPolyline$",
            useCurvedPolyline$,
            false
        );

        const polylineRounding = useSafeValue(
            "polylineRounding$",
            polylineRounding$,
            50
        );

        const squareRotationDegrees = useSafeValue(
            "squareRotationDegrees$",
            squareRotationDegrees$,
            0
        );

        const uiScalePercent = useSafeValue(
            "uiScale$",
            uiScale$,
            100
        );

        const heightFilter = useSafeValue("heightFilter$", heightFilter$, 0);
        const undergroundView = useSafeValue("undergroundView$", undergroundView$, false);

        const deleteMissingAssets = useSafeValue(
            "deleteMissingAssets$",
            deleteMissingAssets$,
            false
        );

        const deleteTrees = useSafeValue(
            "deleteTrees$",
            deleteTrees$,
            false
        );

        const treeAgeFilter = useSafeValue("treeAgeFilter$", treeAgeFilter$, 0);

        const protectAssetVegetation = useSafeValue(
            "protectAssetVegetation$",
            protectAssetVegetation$,
            true
        );

        const deleteBuildings = useSafeValue(
            "deleteBuildings$",
            deleteBuildings$,
            false
        );

        const deleteCars = useSafeValue(
            "deleteCars$",
            deleteCars$,
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
                `Tooltip=${typeof resolver.Tooltip} ` +
                `FreeAreaPolygon=${freeAreaPolygon}`
            );
        }, [isToolActive, freeAreaPolygon]);

        let result: JSX.Element | null = null;

        try {
            result = Component();
        }
        catch (error) {
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

        const scale =
            clamp(uiScalePercent, 50, 125) / 100;

        const treeAgeLabels = [
            text("TreeAgeAll", "Alle Pflanzen"),
            text("TreeAgeSapling", "Setzlinge"),
            text("TreeAgeYoung", "Jung"),
            text("TreeAgeMature", "Ausgewachsen"),
            text("TreeAgeElderly", "Alt"),
            text("TreeAgeDead", "Tot"),
        ];

        const cycleTreeAge = (direction: number) =>
            trigger(
                mod.id,
                BindingKeys.setTreeAgeFilter,
                (treeAgeFilter + direction + treeAgeLabels.length) % treeAgeLabels.length
            );

        const setRadius = (next: number) =>
            trigger(
                mod.id,
                BindingKeys.setBrushRadius,
                clamp(next, RADIUS_MIN, RADIUS_MAX)
            );

        const setLineWidth = (next: number) =>
            trigger(
                mod.id,
                BindingKeys.setLineWidth,
                clamp(next, LINE_WIDTH_MIN, LINE_WIDTH_MAX)
            );

        const setCurvedPolyline = (enabled: boolean) =>
            trigger(
                mod.id,
                BindingKeys.setUseCurvedPolyline,
                enabled
            );

        const setPolylineRounding = (next: number) =>
            trigger(
                mod.id,
                BindingKeys.setPolylineRounding,
                clamp(
                    next,
                    POLYLINE_ROUNDING_MIN,
                    POLYLINE_ROUNDING_MAX
                )
            );

        const setShape = (shape: number) => {
            if (freeAreaPolygon) {
                trigger(
                    mod.id,
                    "setFreeAreaPolygon",
                    false
                );
            }

            trigger(
                mod.id,
                BindingKeys.setSelectionShape,
                shape
            );
        };

        const setFreeAreaPolygon = () => {
            trigger(
                mod.id,
                "setFreeAreaPolygon",
                true
            );
        };

        const setRotation = (next: number) =>
            trigger(
                mod.id,
                BindingKeys.setSquareRotationDegrees,
                ((next % 360) + 360) % 360
            );

        const toggle = (key: string, current: boolean) =>
            trigger(mod.id, key, !current);

        const isFreeAreaPolygon =
            freeAreaPolygon;

        const isPolyline =
            selectionShape === SHAPE_POLYLINE &&
            !isFreeAreaPolygon;

        const isSquare =
            selectionShape === SHAPE_SQUARE &&
            !isFreeAreaPolygon;

        const isTriangle =
            selectionShape === SHAPE_TRIANGLE &&
            !isFreeAreaPolygon;

        const isCircle =
            selectionShape === SHAPE_CIRCLE &&
            !isFreeAreaPolygon;

        const isRotatable =
            isSquare || isTriangle;

        const usesCorridorWidth =
            isPolyline;

        const sizeTitle = isPolyline
            ? text(
                "MultiPointLineWidth",
                "Breite Mehrpunktlinie"
            )
            : isSquare
                ? text(
                    "HalfSide",
                    "Größe Quadrat"
                )
                : isTriangle
                    ? text(
                        "TriangleSize",
                        "Größe Dreieck"
                    )
                    : text(
                        "Radius",
                        "Radius"
                    );

        const sections = (
            <ToolPanelContext.Provider value={{ scale, compact }}>
                {launcherMode !== VANILLA_BULLDOZER_LAUNCHER_MODE && (
                    <div className={styles.compactColorToolbar}>
                        <IconToolButton
                            icon="palette"
                            selected={colorEditorOpen}
                            tooltipTitle={
                                colorEditorOpen
                                    ? text(
                                        "CloseColorEditor",
                                        "Farbeditor schließen"
                                    )
                                    : text(
                                        "OpenColorEditor",
                                        "Farbeditor öffnen"
                                    )
                            }
                            tooltipText={text(
                                "ColorEditorTooltip",
                                "Farben mit direkter Vorschau anpassen."
                            )}
                            onSelect={() => toggleToolWindow("colors")}
                        />
                        <IconToolButton
                            icon="gear"
                            selected={toolWindow === "options"}
                            tooltipTitle={text("ToolOptionsTitle", "Optionen")}
                            onSelect={() => toggleToolWindow("options")}
                        />
                    </div>
                )}

                <CompactToolSection title={text("Selection", "Auswahl")}>
                    <div
                        className={styles.selectionButtons}
                        style={{ width: `${MAIN_FILTER_COLUMNS * (iconButtonSize(compact, scale) + 4)}rem` }}
                    >
                        <IconToolButton
                            icon="undo"
                            disabled={vegetationUndoCount === 0}
                            tooltipTitle={text("UndoVegetation", "Vegetation wiederherstellen")}
                            tooltipText={text(
                                "UndoVegetationTooltip",
                                "Stellt Vegetation aus der letzten Löschaktion wieder her, einschließlich Pflanzen an noch vorhandenen Gebäuden und Parks ({count})."
                            ).replace("{count}", String(vegetationUndoCount))}
                            onSelect={() => trigger(mod.id, BindingKeys.undoVegetation)}
                        />
                        <div className={styles.selectionShapes}>
                            <IconToolButton
                                icon="circle"
                                selected={isCircle}
                                tooltipTitle={text("Circle", "Kreis")}
                                tooltipText={text(
                                    "CircleTooltip",
                                    "Runde Auswahlfläche um den Mauszeiger."
                                )}
                                onSelect={() => setShape(SHAPE_CIRCLE)}
                            />

                            <IconToolButton
                                icon="square"
                                selected={isSquare}
                                tooltipTitle={text("Square", "Quadrat")}
                                tooltipText={text(
                                    "SquareTooltip",
                                    "Quadratische Auswahlfläche, drehbar mit gehaltener rechter Maustaste."
                                )}
                                onSelect={() => setShape(SHAPE_SQUARE)}
                            />

                            <IconToolButton
                                icon="triangle"
                                selected={isTriangle}
                                tooltipTitle={text("Triangle", "Dreieck")}
                                tooltipText={text(
                                    "TriangleTooltip",
                                    "Gleichseitige Auswahlfläche. Größe wie beim normalen Pinsel; mit gehaltener rechter Maustaste drehbar."
                                )}
                                onSelect={() => setShape(SHAPE_TRIANGLE)}
                            />

                            <IconToolButton
                                icon="polyline"
                                selected={isPolyline}
                                tooltipTitle={text(
                                    "MultiPointLine",
                                    "Mehrpunktlinie"
                                )}
                                tooltipText={text(
                                    "MultiPointLineTooltip",
                                    "2 bis 15 Punkte mit geraden oder abgerundeten Übergängen. Linksklick setzt Punkte, Doppelklick schließt ab und löscht. Rechtsklick entfernt den letzten Punkt, Esc bricht ab."
                                )}
                                onSelect={() => setShape(SHAPE_POLYLINE)}
                            />

                            <IconToolButton
                                icon="polygon"
                                selected={isFreeAreaPolygon}
                                tooltipTitle={text(
                                    "FreeAreaPolygon",
                                    "Freie Fläche"
                                )}
                                tooltipText={text(
                                    "FreeAreaPolygonTooltip",
                                    "Beliebig viele Eckpunkte definieren eine freie Löschfläche. Linksklick setzt Punkte. Klick auf den ersten Punkt oder Doppelklick schließt das Polygon und löscht den Inhalt. Rechtsklick entfernt den letzten Punkt, Esc verwirft die Auswahl."
                                )}
                                onSelect={setFreeAreaPolygon}
                            />
                        </div>
                    </div>
                </CompactToolSection>

                {!isFreeAreaPolygon && (
                    <CompactToolSection title={sizeTitle}>
                        {usesCorridorWidth ? (
                            <Stepper
                                value={`${lineWidth} m`}
                                decreaseTooltip={text(
                                    "DecreaseLineWidth",
                                    "Linie schmaler"
                                )}
                                increaseTooltip={text(
                                    "IncreaseLineWidth",
                                    "Linie breiter"
                                )}
                                onDecrease={() =>
                                    setLineWidth(
                                        lineWidth -
                                        LINE_WIDTH_STEP
                                    )
                                }
                                onIncrease={() =>
                                    setLineWidth(
                                        lineWidth +
                                        LINE_WIDTH_STEP
                                    )
                                }
                            />
                        ) : (
                            <Stepper
                                value={`${brushRadius} m`}
                                decreaseTooltip={text(
                                    "DecreaseSize",
                                    "Auswahl verkleinern"
                                )}
                                increaseTooltip={text(
                                    "IncreaseSize",
                                    "Auswahl vergrößern"
                                )}
                                onDecrease={() =>
                                    setRadius(
                                        brushRadius -
                                        RADIUS_STEP
                                    )
                                }
                                onIncrease={() =>
                                    setRadius(
                                        brushRadius +
                                        RADIUS_STEP
                                    )
                                }
                            />
                        )}
                    </CompactToolSection>
                )}

                {isFreeAreaPolygon && (
                    <CompactToolSection
                        title={
                            text(
                                "FreeAreaPolygon",
                                "Freie Fläche"
                            ) +
                            ` · ${freeAreaPolygonPointCount} ` +
                            text(
                                "PolygonPoints",
                                "Punkte"
                            )
                        }
                    >
                        <></>
                    </CompactToolSection>
                )}

                {isPolyline && (
                    <CompactToolSection title={text("PolylineStyle", "Linienform")}>
                        <IconToolButton
                            icon="straight"
                            selected={!useCurvedPolyline}
                            tooltipTitle={text("PolylineStraight", "Gerade")}
                            tooltipText={text(
                                "PolylineStraightTooltip",
                                "Verbindet alle gesetzten Punkte mit geraden Segmenten."
                            )}
                            onSelect={() => setCurvedPolyline(false)}
                        />

                        <IconToolButton
                            icon="curve"
                            selected={useCurvedPolyline}
                            tooltipTitle={text("PolylineCurved", "Kurve")}
                            tooltipText={text(
                                "PolylineCurvedTooltip",
                                "Rundet die Übergänge zwischen den gesetzten Punkten weich ab."
                            )}
                            onSelect={() => setCurvedPolyline(true)}
                        />
                    </CompactToolSection>
                )}

                {isPolyline && useCurvedPolyline && (
                    <CompactToolSection title={text("PolylineRounding", "Kurvenrundung")}>
                        <Stepper
                            value={`${polylineRounding} %`}
                            decreaseTooltip={text(
                                "DecreasePolylineRounding",
                                "Rundung verringern"
                            )}
                            increaseTooltip={text(
                                "IncreasePolylineRounding",
                                "Rundung erhöhen"
                            )}
                            onDecrease={() =>
                                setPolylineRounding(
                                    polylineRounding -
                                    POLYLINE_ROUNDING_STEP
                                )
                            }
                            onIncrease={() =>
                                setPolylineRounding(
                                    polylineRounding +
                                    POLYLINE_ROUNDING_STEP
                                )
                            }
                        />
                    </CompactToolSection>
                )}

                {isRotatable && (
                    <CompactToolSection title={text("Rotation", "Drehung")}>
                        <Stepper
                            narrow
                            value={`${Math.round(squareRotationDegrees)}°`}
                            decreaseTooltip={text(
                                "RotateLeft",
                                "Gegen den Uhrzeigersinn drehen"
                            )}
                            increaseTooltip={text(
                                "RotateRight",
                                "Im Uhrzeigersinn drehen"
                            )}
                            onDecrease={() =>
                                setRotation(
                                    squareRotationDegrees -
                                    ROTATION_STEP
                                )
                            }
                            onIncrease={() =>
                                setRotation(
                                    squareRotationDegrees +
                                    ROTATION_STEP
                                )
                            }
                        />
                    </CompactToolSection>
                )}

                <CompactToolSection title={text("HeightFilter", "Höhenfilter")}>
                    <IconToolButton icon="heightAll" selected={heightFilter === 0}
                        tooltipTitle={text("HeightAll", "Alle Höhen")}
                        tooltipText={text("HeightFilterTooltip", "Höhenfilter für Vorschau und Löschen. Gemischte Übergänge nur bei Alle Höhen.")}
                        onSelect={() => trigger(mod.id, BindingKeys.setHeightFilter, 0)} />
                    <IconToolButton icon="heightAbove" selected={heightFilter === 1}
                        tooltipTitle={text("HeightAbove", "Oberhalb / auf Gelände")}
                        tooltipText={text("HeightFilterTooltip", "Höhenfilter für Vorschau und Löschen. Gemischte Übergänge nur bei Alle Höhen.")}
                        onSelect={() => trigger(mod.id, BindingKeys.setHeightFilter, 1)} />
                    <IconToolButton icon="heightBelow" selected={heightFilter === 2}
                        tooltipTitle={text("HeightBelow", "Unterhalb Gelände")}
                        tooltipText={text("HeightFilterTooltip", "Höhenfilter für Vorschau und Löschen. Gemischte Übergänge nur bei Alle Höhen.")}
                        onSelect={() => trigger(mod.id, BindingKeys.setHeightFilter, 2)} />
                    <IconToolButton icon="undergroundView" selected={undergroundView}
                        tooltipTitle={text("UndergroundView", "Untergrundansicht")}
                        tooltipText={text("UndergroundViewTooltip", "Unterirdische Objekte anzeigen. Ändert den Höhenfilter nicht.")}
                        onSelect={() => trigger(mod.id, BindingKeys.setUndergroundView, !undergroundView)} />
                </CompactToolSection>

                <CompactToolSection title={text("MainFilters", "Filter")}>
                    <div className={styles.filterButtons}>
                    <IconToolButton
                        icon="missingAsset"
                        selected={deleteMissingAssets}
                        tooltipTitle={text("MissingAssets", "Fehlende Assets")}
                        tooltipText={text("MissingAssetsTooltip", "Weiße Platzhalter bestätigter fehlender Fahrzeuge, Pflanzen und Objekte löschen, auch als Unterobjekte. Gebäude und Netzwerke sind nicht enthalten.")}
                        onSelect={() => toggle(BindingKeys.setDeleteMissingAssets, deleteMissingAssets)}
                    />
                    <IconToolButton
                        icon="vegetation"
                        selected={deleteTrees}
                        tooltipTitle={text("VegetationShort", "Vegetation")}
                        tooltipText={text(
                            "VegetationTooltip",
                            "Bäume, Büsche und Pflanzen löschen."
                        )}
                        onSelect={() =>
                            toggle(
                                BindingKeys.setDeleteTrees,
                                deleteTrees
                            )
                        }
                    />

                    <IconToolButton
                        icon="building"
                        selected={deleteBuildings}
                        tooltipTitle={text("Buildings", "Gebäude")}
                        tooltipText={text(
                            "BuildingsTooltip",
                            "Gebäude im Bereich löschen."
                        )}
                        onSelect={() =>
                            toggle(
                                BindingKeys.setDeleteBuildings,
                                deleteBuildings
                            )
                        }
                    />

                    <IconToolButton
                        icon="car"
                        selected={deleteCars}
                        tooltipTitle={text("Cars", "Autos")}
                        tooltipText={text("CarsTooltip", "Fahrende und geparkte Straßenfahrzeuge löschen. Die Simulation pausiert bei Aktivierung automatisch.")}
                        onSelect={() => toggle(BindingKeys.setDeleteCars, deleteCars)}
                    />

                    <IconToolButton
                        icon="road"
                        selected={deleteRoads}
                        tooltipTitle={text("Roads", "Straßen")}
                        tooltipText={text(
                            "RoadsTooltip",
                            "Straßen im Bereich löschen."
                        )}
                        onSelect={() =>
                            toggle(
                                BindingKeys.setDeleteRoads,
                                deleteRoads
                            )
                        }
                    />

                    <IconToolButton
                        icon="path"
                        selected={deletePaths}
                        tooltipTitle={text("Paths", "Fußwege")}
                        tooltipText={text(
                            "PathsTooltip",
                            "Fuß- und Radwege löschen."
                        )}
                        onSelect={() =>
                            toggle(
                                BindingKeys.setDeletePaths,
                                deletePaths
                            )
                        }
                    />

                    <IconToolButton
                        icon="rail"
                        selected={deleteRailways}
                        tooltipTitle={text("Railways", "Gleise")}
                        tooltipText={text(
                            "RailwaysTooltip",
                            "Schienenwege löschen."
                        )}
                        onSelect={() =>
                            toggle(
                                BindingKeys.setDeleteRailways,
                                deleteRailways
                            )
                        }
                    />

                    <IconToolButton
                        icon="surface"
                        selected={deleteSurfaces}
                        tooltipTitle={text("SurfacesShort", "Flächen")}
                        tooltipText={text(
                            "SurfacesTooltip",
                            "Oberflächen und Bereiche löschen."
                        )}
                        onSelect={() =>
                            toggle(
                                BindingKeys.setDeleteSurfaces,
                                deleteSurfaces
                            )
                        }
                    />

                    <IconToolButton
                        icon="props"
                        selected={deleteStaticObjects}
                        tooltipTitle={text(
                            "StaticObjectsShort",
                            "Props / Marker"
                        )}
                        tooltipText={text(
                            "StaticObjectsTooltip",
                            "Hauptschalter für Props und Marker. Aktiviert die Detailfilter darunter."
                        )}
                        onSelect={() =>
                            toggle(
                                BindingKeys.setDeleteStaticObjects,
                                deleteStaticObjects
                            )
                        }
                    />
                    </div>
                </CompactToolSection>

                {deleteTrees && (
                    <CompactToolSection title={text("VegetationShort", "Vegetation")}>
                        <IconToolButton
                            icon="protectVegetation"
                            selected={protectAssetVegetation}
                            tooltipTitle={text("ProtectAssetVegetation", "Asset-Vegetation schützen")}
                            tooltipText={text(
                                "ProtectAssetVegetationTooltip",
                                "Schützt Pflanzen, die zu Gebäuden oder anderen Assets gehören, beim Löschen mit dem Vegetationsfilter."
                            )}
                            onSelect={() => toggle(BindingKeys.setProtectAssetVegetation, protectAssetVegetation)}
                        />
                    </CompactToolSection>
                )}

                {deleteTrees && (
                    <CompactToolSection title={text("TreeAgeFilter", "Baumalter")}>
                        <Stepper
                            wide
                            value={treeAgeLabels[treeAgeFilter] ?? treeAgeLabels[0]}
                            decreaseTooltip={text("TreeAgePrevious", "Vorherige Altersstufe")}
                            increaseTooltip={text("TreeAgeNext", "Nächste Altersstufe")}
                            onDecrease={() => cycleTreeAge(-1)}
                            onIncrease={() => cycleTreeAge(1)}
                        />
                    </CompactToolSection>
                )}

                {deleteStaticObjects && (
                    <CompactToolSection title={text("AdvancedFiltersShort", "Props / Marker")}>
                        <div className={styles.filterButtons}>
                        <IconToolButton
                            marker
                            icon="generalProps"
                            selected={deleteGeneralProps}
                            tooltipTitle={text("GeneralProps", "Allgemeine Props")}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDeleteGeneralProps,
                                    deleteGeneralProps
                                )
                            }
                        />

                        <IconToolButton
                            marker
                            icon="streetLight"
                            selected={deleteStreetLights}
                            tooltipTitle={text("StreetLights", "Straßenlaternen")}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDeleteStreetLights,
                                    deleteStreetLights
                                )
                            }
                        />

                        <IconToolButton
                            marker
                            icon="quantity"
                            selected={deleteQuantityObjects}
                            tooltipTitle={text(
                                "QuantityObjects",
                                "Mülleimer und Mengenobjekte"
                            )}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDeleteQuantityObjects,
                                    deleteQuantityObjects
                                )
                            }
                        />

                        <IconToolButton
                            marker
                            icon="branding"
                            selected={deleteBrandingObjects}
                            tooltipTitle={text(
                                "Branding",
                                "Werbung und Branding"
                            )}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDeleteBrandingObjects,
                                    deleteBrandingObjects
                                )
                            }
                        />

                        <IconToolButton
                            marker
                            icon="activity"
                            selected={deleteActivityLocations}
                            tooltipTitle={text(
                                "ActivityLocations",
                                "Aktivitätspunkte"
                            )}
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
                            tooltipTitle={text(
                                "SpawnLocations",
                                "Spawnpunkte"
                            )}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDeleteSpawnLocations,
                                    deleteSpawnLocations
                                )
                            }
                        />

                        <IconToolButton
                            marker
                            icon="lanes"
                            selected={deleteMarkerNetworks}
                            tooltipTitle={text(
                                "AssetLanes",
                                "Asset-Lanes / SubLanes"
                            )}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDeleteMarkerNetworks,
                                    deleteMarkerNetworks
                                )
                            }
                        />

                        </div>
                    </CompactToolSection>
                )}
                {(deleteStaticObjects || deleteMissingAssets) && (
                    <CompactToolSection title={text("DimBackground", "Hintergrund abdunkeln")}>
                        <IconToolButton
                            marker
                            icon="dim"
                            selected={dimMarkerBackground}
                            tooltipTitle={text(
                                "DimBackground",
                                "Hintergrund abdunkeln"
                            )}
                            onSelect={() =>
                                toggle(
                                    BindingKeys.setDimMarkerBackground,
                                    dimMarkerBackground
                                )
                            }
                        />
                    </CompactToolSection>
                )}
            </ToolPanelContext.Provider>
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
        }
        catch (error) {
            console.error(
                "[AreaBulldozer] Sections konnten nicht eingehängt werden.",
                error
            );

            return result;
        }
    };
};
