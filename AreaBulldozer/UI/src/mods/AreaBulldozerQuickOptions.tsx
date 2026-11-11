import React from "react";
import { trigger } from "cs2/api";
import mod from "mod.json";

import {
    BindingKeys,
    confirmLargeSelection$,
    deleteBuildingSubObjects$,
    deleteNetworkSubObjects$,
    largeSelectionThreshold$,
    protectAssetVegetation$,
    protectOwnedObjects$,
    useCompactToolPanel$,
} from "../bindings";
import { closeToolWindow } from "./AreaBulldozerColorEditorState";
import styles from "./AreaBulldozerColorEditor.module.scss";
import { useSafeValue } from "./useSafeValue";
import { VanillaComponentResolver } from "./VanillaComponentResolver";

const THRESHOLD_MIN = 50;
const THRESHOLD_MAX = 2000;
const THRESHOLD_STEP = 10;

interface QuickOptionsProps {
    text: (key: string, fallback: string) => string;
    dragging: boolean;
    onDragStart: (event: React.MouseEvent<HTMLDivElement>) => void;
}

export function AreaBulldozerQuickOptions({ text, dragging, onDragStart }: QuickOptionsProps) {
    const compact = useSafeValue("useCompactToolPanel$", useCompactToolPanel$, false);
    const confirm = useSafeValue("confirmLargeSelection$", confirmLargeSelection$, true);
    const includeBuilding = useSafeValue("deleteBuildingSubObjects$", deleteBuildingSubObjects$, false);
    const protectVegetation = useSafeValue("protectAssetVegetation$", protectAssetVegetation$, true);
    const includeNetwork = useSafeValue("deleteNetworkSubObjects$", deleteNetworkSubObjects$, false);
    const protectOwned = useSafeValue("protectOwnedObjects$", protectOwnedObjects$, true);
    const threshold = useSafeValue("largeSelectionThreshold$", largeSelectionThreshold$, 250);

    const title = text("ToolOptionsTitle", "Optionen");
    const sizeLabel = text("ToolOptionsSize", "UI-Größe");
    const safetyLabel = text("ToolOptionsSafety", "Sicherheit");
    const thresholdLabel = text("ToolOptionsThreshold", "Bestätigungsgrenze");
    const normalLabel = text("ToolOptionsNormal", "Normal");
    const compactLabel = text("ToolOptionsCompact", "Kompakt");
    const safetyOptions = [
        {
            key: BindingKeys.setDeleteBuildingSubObjects,
            label: text("ToolOptionsIncludeBuilding", "Gebäude-Unterobjekte einschließen"),
            description: text("ToolOptionsIncludeBuildingTooltip", "Schließt passende Unterobjekte von Gebäuden ein, wenn ihr Objektfilter aktiv ist."),
            enabled: includeBuilding,
        },
        {
            key: BindingKeys.setProtectAssetVegetation,
            label: text("ProtectAssetVegetation", "Asset-Vegetation schützen"),
            description: text("ProtectAssetVegetationTooltip", "Schützt Pflanzen, die zu Gebäuden oder anderen Assets gehören, beim Löschen mit dem Vegetationsfilter."),
            enabled: protectVegetation,
        },
        {
            key: BindingKeys.setDeleteNetworkSubObjects,
            label: text("ToolOptionsIncludeNetwork", "Netzwerk-Unterobjekte einschließen"),
            description: text("ToolOptionsIncludeNetworkTooltip", "Schließt passende Unterobjekte von Straßen und anderen Netzwerken ein, wenn ihr Objektfilter aktiv ist."),
            enabled: includeNetwork,
        },
        {
            key: BindingKeys.setProtectOwnedObjects,
            label: text("ToolOptionsProtectOwned", "Zugeordnete Objekte schützen"),
            description: text("ToolOptionsProtectOwnedTooltip", "Schützt zugeordnete Objekte, deren Besitzer weder Gebäude noch Netzwerk ist."),
            enabled: protectOwned,
        },
        {
            key: BindingKeys.setConfirmLargeSelection,
            label: text("ToolOptionsConfirm", "Große Auswahl bestätigen"),
            description: text("ToolOptionsConfirmTooltip", "Verlangt ab der Bestätigungsgrenze einen zweiten Klick innerhalb von fünf Sekunden."),
            enabled: confirm,
        },
    ];
    const closeLabel = text("ToolOptionsClose", "Optionen schließen");

    const buttonClass = (selected: boolean) =>
        selected ? `${styles.stateButton} ${styles.stateButtonSelected}` : styles.stateButton;

    const resolver = VanillaComponentResolver.instance;
    const Tooltip = resolver.Tooltip;
    const tooltipTheme = resolver.descriptionTooltipTheme;

    const withTooltip = (control: JSX.Element, label: string, description?: string) =>
        Tooltip ? (
            <Tooltip tooltip={
                <>
                    <div className={tooltipTheme?.title}>{label}</div>
                    {description && <div className={tooltipTheme?.content}>{description}</div>}
                </>
            }>
                {control}
            </Tooltip>
        ) : React.cloneElement(control, { title: description ?? label });

    const adjustThreshold = (delta: number) =>
        trigger(
            mod.id,
            BindingKeys.setLargeSelectionThreshold,
            Math.min(THRESHOLD_MAX, Math.max(THRESHOLD_MIN, threshold + delta))
        );

    return (
        <div className={styles.editor} role="dialog" aria-label={title}>
            <div
                className={[
                    styles.titleRow,
                    styles.titleRowDraggable,
                    dragging ? styles.titleRowDragging : "",
                ].filter(Boolean).join(" ")}
                onMouseDown={onDragStart}
            >
                <strong>{title}</strong>
                {withTooltip(
                    <button
                        type="button"
                        className={styles.optionsClose}
                        aria-label={closeLabel}
                        onMouseDown={event => event.stopPropagation()}
                        onClick={closeToolWindow}
                    >
                        <svg viewBox="0 0 16 16" aria-hidden="true">
                            <path d="M4 4l8 8M12 4l-8 8" fill="none" stroke="#fff" strokeWidth="2" strokeLinecap="round" />
                        </svg>
                    </button>,
                    closeLabel
                )}
            </div>

            <div className={styles.optionsRow} role="group" aria-label={sizeLabel}>
                <span>{sizeLabel}</span>
                <div className={styles.optionsButtons}>
                    {withTooltip(
                        <button
                            type="button"
                            className={buttonClass(!compact)}
                            aria-pressed={!compact}
                            onClick={() => trigger(mod.id, BindingKeys.setUseCompactToolPanel, false)}
                        >
                            {normalLabel}
                        </button>,
                        normalLabel,
                        text("ToolOptionsNormalTooltip", "Verwendet die Standardgröße für Schaltflächen, Symbole und Überschriften.")
                    )}
                    {withTooltip(
                        <button
                            type="button"
                            className={buttonClass(compact)}
                            aria-pressed={compact}
                            onClick={() => trigger(mod.id, BindingKeys.setUseCompactToolPanel, true)}
                        >
                            {compactLabel}
                        </button>,
                        compactLabel,
                        text("ToolOptionsCompactTooltip", "Verkleinert Schaltflächen, Symbole und Überschriften, damit das Werkzeugpanel weniger Platz braucht.")
                    )}
                </div>
            </div>

            <div role="group" aria-label={safetyLabel}>
                <div className={styles.optionsGroupTitle}>{safetyLabel}</div>
                {safetyOptions.map(({ key, label, description, enabled }) => (
                    <React.Fragment key={key}>
                        {withTooltip(
                            <div className={styles.optionsRow}>
                                <span className={label.length > 25 ? styles.longLabel : undefined}>{label}</span>
                                <div className={styles.optionsButtons}>
                                    <button
                                        type="button"
                                        className={buttonClass(enabled)}
                                        role="switch"
                                        aria-label={label}
                                        aria-checked={enabled}
                                        onClick={() => trigger(mod.id, key, !enabled)}
                                    >
                                        {enabled ? text("ToolOptionsOn", "Ein") : text("ToolOptionsOff", "Aus")}
                                    </button>
                                </div>
                            </div>,
                            label,
                            description
                        )}
                    </React.Fragment>
                ))}

                {withTooltip(
                    <div className={styles.optionsRow} role="group" aria-label={thresholdLabel}>
                        <span className={thresholdLabel.length > 25 ? styles.longLabel : undefined}>{thresholdLabel}</span>
                        <div className={styles.optionsButtons}>
                            <button
                                type="button"
                                className={styles.thresholdButton}
                                aria-label={`${thresholdLabel} −${THRESHOLD_STEP}`}
                                disabled={threshold <= THRESHOLD_MIN}
                                onClick={() => adjustThreshold(-THRESHOLD_STEP)}
                            >
                                −
                            </button>
                            <span className={styles.thresholdValue}>{threshold}</span>
                            <button
                                type="button"
                                className={styles.thresholdButton}
                                aria-label={`${thresholdLabel} +${THRESHOLD_STEP}`}
                                disabled={threshold >= THRESHOLD_MAX}
                                onClick={() => adjustThreshold(THRESHOLD_STEP)}
                            >
                                +
                            </button>
                        </div>
                    </div>,
                    thresholdLabel,
                    text("ToolOptionsThresholdTooltip", "Legt fest, ab wie vielen ausgewählten Objekten eine zweite Bestätigung erforderlich ist.")
                )}
            </div>
        </div>
    );
}
