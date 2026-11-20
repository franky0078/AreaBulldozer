import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { bindValue, trigger } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "mod.json";
import { BindingKeys, getToolPanelScale, isToolActive$, uiScale$ } from "../bindings";
import { useSafeValue } from "./useSafeValue";
import { AreaBulldozerTooltip as Tooltip } from "./AreaBulldozerTooltip";
import styles from "./AreaBulldozerAssetSelection.module.scss";

const items$ = bindValue<string>(mod.id, "assetFilterItems", "[]");
const enabled$ = bindValue<boolean>(mod.id, "assetFilterEnabled", false);
const picker$ = bindValue<boolean>(mod.id, "assetPickerActive", false);
const undoCount$ = bindValue<number>(mod.id, "assetSelectionUndoCount", 0);
type Text = (key: string, fallback: string) => string;
interface AssetItem { id: string; name: string; icon: string; available: boolean; }

let open = false;
let savedPosition: { x: number; y: number } | null = null;
let savedHeight = 400;
const listeners = new Set<() => void>();
function showWindow(value: boolean) {
    open = value;
    listeners.forEach(listener => listener());
}
function useOpen() {
    const [value, setValue] = useState(open);
    useEffect(() => {
        const listener = () => setValue(open);
        listeners.add(listener);
        listener();
        return () => { listeners.delete(listener); };
    }, []);
    return value;
}
let rowVisible = false;
const rowListeners = new Set<() => void>();
function showRow(value: boolean) {
    rowVisible = value;
    rowListeners.forEach(listener => listener());
}
export function useAssetSelectionRow() {
    const [visible, setVisible] = useState(rowVisible);
    const enabled = useSafeValue("assetFilterEnabled$", enabled$, false);
    useEffect(() => {
        const listener = () => setVisible(rowVisible);
        rowListeners.add(listener);
        listener();
        return () => { rowListeners.delete(listener); };
    }, []);
    // Loading an explicit saved profile must never leave an invisible filter active.
    useEffect(() => { if (enabled) showRow(true); }, [enabled]);
    return { visible, toggle: () => {
        const next = !rowVisible;
        showRow(next);
        if (!next) {
            showWindow(false);
            trigger(mod.id, "setAssetPicker", false);
            trigger(mod.id, "setAssetFilterEnabled", false);
        }
    } };
}
function useAssetItems() {
    const json = useSafeValue("assetFilterItems$", items$, "[]");
    return useMemo<AssetItem[]>(() => {
        try {
            const data: unknown = JSON.parse(json);
            return Array.isArray(data) ? data.filter(item => item && typeof item.id === "string" &&
                typeof item.name === "string" && typeof item.icon === "string" && typeof item.available === "boolean") : [];
        } catch { return []; }
    }, [json]);
}

function PipetteIcon() {
    return <svg viewBox="0 0 24 24" aria-hidden="true"><path d="m14 5 5 5M4 17l9-9 3 3-9 9H4v-3Zm11-9 4-4a2 2 0 0 1 3 3l-4 4M3 21l2-2" /></svg>;
}
function ListIcon() {
    return <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M9 5h12M9 12h12M9 19h12M3 4h2v2H3V4Zm0 7h2v2H3v-2Zm0 7h2v2H3v-2Z" /></svg>;
}
function UndoIcon() {
    return <svg viewBox="0 0 24 24" aria-hidden="true"><path d="m8 4-5 5 5 5M3 9h11a6 6 0 0 1 0 12h-3" /></svg>;
}
function AssetIcon({ src }: { src: string }) {
    const [failed, setFailed] = useState("");
    return <span className={styles.thumbnail}>{src && failed !== src
        ? <img src={src} alt="" onError={() => setFailed(src)} /> : <ListIcon />}</span>;
}

export function AreaBulldozerAssetSelectionButton({ text, scale, compact }: { text: Text; scale: number; compact: boolean }) {
    const visible = useOpen();
    const items = useAssetItems();
    const enabled = useSafeValue("assetFilterEnabled$", enabled$, false);
    const picking = useSafeValue("assetPickerActive$", picker$, false);
    const size = (compact ? 24 : 28) * scale;
    const icon = (compact ? 16 : 18) * scale;
    const buttonStyle = { height: `${size}rem`, minWidth: `${size}rem`, fontSize: `${12 * scale}rem` };
    return <div className={styles.launcher} onMouseDown={event => event.stopPropagation()} onClick={event => event.stopPropagation()}>
        <Tooltip title={text("AssetListOpen", "Assetliste öffnen")} description={text("AssetProfileHint", "Die Assetliste wird mit eigenen Filterprofilen gespeichert.")}>
        <button type="button" style={buttonStyle} className={visible || enabled ? styles.selected : ""}
            aria-pressed={visible}
            onClick={() => {
                showWindow(!visible);
                if (visible) trigger(mod.id, "setAssetPicker", false);
            }}>
            <span style={{ width: `${icon}rem`, height: `${icon}rem` }}><ListIcon /></span>
            <span className={styles.launchLabel}>{`${text("AssetList", "Assetliste")} (${items.length})`}</span>
            {enabled && <span className={styles.activeDot} />}
        </button></Tooltip>
        <Tooltip title={text("AssetPicker", "Assets mit Pipette aufnehmen")} description={text("AssetPickerHint", "Objekte anklicken, um ihre Assets aufzunehmen. Rechtsklick oder Esc beendet die Pipette.")}>
        <button type="button" style={{ ...buttonStyle, width: `${size}rem` }} className={`${styles.pipetteButton} ${picking ? styles.pipetteSelected : ""}`}
            aria-pressed={picking}
            onClick={() => { showWindow(true); trigger(mod.id, "setAssetPicker", !picking); }}>
            <span style={{ width: `${icon}rem`, height: `${icon}rem` }}><PipetteIcon /></span>
        </button></Tooltip>
    </div>;
}

export function AreaBulldozerAssetSelectionWindow() {
    const visible = useOpen();
    const active = useSafeValue("isToolActive$", isToolActive$, false);
    const scale = getToolPanelScale(useSafeValue("uiScale$", uiScale$, 100));
    const enabled = useSafeValue("assetFilterEnabled$", enabled$, false);
    const picking = useSafeValue("assetPickerActive$", picker$, false);
    const undoCount = useSafeValue("assetSelectionUndoCount$", undoCount$, 0);
    const items = useAssetItems();
    const { translate } = useLocalization();
    const text = useCallback<Text>((key, fallback) => translate(`AreaBulldozer.UI.${key}`, fallback) ?? fallback, [translate]);
    const panel = useRef<HTMLDivElement>(null);
    const pointerInside = useRef(false);
    const drag = useRef<{ x: number; y: number; left: number; top: number } | null>(null);
    const resizeSession = useRef<{ y: number; height: number } | null>(null);
    const [height, setHeight] = useState(savedHeight);
    const [position, setPosition] = useState(savedPosition ?? { x: 560, y: 260 });
    const [dragging, setDragging] = useState(false);
    const shown = visible && active;
    const setPointer = useCallback((inside: boolean) => {
        if (inside !== pointerInside.current) {
            pointerInside.current = inside;
            trigger(mod.id, BindingKeys.setPointerOverUI, inside);
        }
    }, []);
    const clampPosition = useCallback((x: number, y: number) => {
        const rect = panel.current?.getBoundingClientRect();
        const next = {
            x: Math.max(8, Math.min(x, Math.max(8, window.innerWidth - (rect?.width ?? 345) - 8))),
            y: Math.max(8, Math.min(y, Math.max(8, window.innerHeight - (rect?.height ?? 380) - 8))),
        };
        savedPosition = next;
        setPosition(next);
    }, []);
    useEffect(() => {
        if (!active) showWindow(false);
    }, [active]);
    useEffect(() => {
        if (!shown) return;
        const start = savedPosition ?? { x: window.innerWidth * 0.55, y: window.innerHeight * 0.22 };
        clampPosition(start.x, start.y);
        const move = (event: MouseEvent) => {
            const session = drag.current;
            const resizing = resizeSession.current;
            if (resizing) {
                const top = panel.current?.getBoundingClientRect().top ?? 8;
                const minimum = Math.min(230 * scale, window.innerHeight - top - 8);
                const pixels = Math.max(minimum, Math.min(resizing.height + event.clientY - resizing.y,
                    window.innerHeight - top - 8));
                savedHeight = pixels / scale;
                setHeight(savedHeight);
            } else if (session) {
                clampPosition(session.left + event.clientX - session.x, session.top + event.clientY - session.y);
            }
            setPointer(!!panel.current?.contains(event.target as Node | null) || !!session || !!resizing);
        };
        const release = (event: MouseEvent) => {
            drag.current = null;
            resizeSession.current = null;
            setDragging(false);
            const rect = panel.current?.getBoundingClientRect();
            setPointer(!!rect && event.clientX >= rect.left && event.clientX <= rect.right &&
                event.clientY >= rect.top && event.clientY <= rect.bottom);
        };
        const resize = () => {
            const current = savedPosition ?? start;
            clampPosition(current.x, current.y);
        };
        document.addEventListener("mousemove", move, true);
        document.addEventListener("mouseup", release, true);
        window.addEventListener("resize", resize);
        return () => {
            drag.current = null;
            resizeSession.current = null;
            setDragging(false);
            document.removeEventListener("mousemove", move, true);
            document.removeEventListener("mouseup", release, true);
            window.removeEventListener("resize", resize);
            setPointer(false);
        };
    }, [shown, scale, clampPosition, setPointer]);
    const close = () => {
        trigger(mod.id, "setAssetPicker", false);
        setPointer(false);
        showWindow(false);
    };
    if (!shown) return null;
    return <div ref={panel} className={`${styles.window} ${dragging ? styles.dragging : ""}`}
        style={{ left: `${position.x}px`, top: `${position.y}px`, width: `${345 * scale}rem`, height: `${height * scale}px`, fontSize: `${12 * scale}rem` }}
        onMouseEnter={() => setPointer(true)} onMouseLeave={() => { if (!drag.current && !resizeSession.current) setPointer(false); }}
        onMouseDown={event => event.stopPropagation()} onMouseUp={event => event.stopPropagation()}
        onClick={event => event.stopPropagation()} onContextMenu={event => { event.preventDefault(); event.stopPropagation(); }}>
        <div className={styles.header} style={{ height: `${28 * scale}rem`, minHeight: `${28 * scale}rem`, padding: `0 ${8 * scale}rem` }} onMouseDown={event => {
            if (event.button !== 0 || (event.target as HTMLElement).closest("button")) return;
            event.preventDefault();
            const rect = panel.current?.getBoundingClientRect();
            if (!rect) return;
            drag.current = { x: event.clientX, y: event.clientY, left: rect.left, top: rect.top };
            setDragging(true);
            setPointer(true);
        }}>
            <Tooltip title={text("AssetProfileHint", "Die Assetliste wird mit eigenen Filterprofilen gespeichert.")}>
                <strong style={{ fontSize: `${13 * scale}rem`, lineHeight: `${18 * scale}rem` }}>{`${text("AssetList", "Assetliste")} (${items.length})`}</strong>
            </Tooltip>
            <Tooltip title={text("AssetListClose", "Assetliste schließen")}>
                <button type="button" style={{ width: `${18 * scale}rem`, height: `${22 * scale}rem`, fontSize: `${13 * scale}rem` }} onClick={close}>×</button>
            </Tooltip>
        </div>
        <div className={styles.controls}>
            <div className={styles.actions}>
                <Tooltip title={text("AssetPicker", "Assets mit Pipette aufnehmen")} description={text("AssetPickerHint", "Objekte anklicken, um ihre Assets aufzunehmen. Rechtsklick oder Esc beendet die Pipette.")}>
                <button type="button" className={picking ? styles.selected : ""} aria-pressed={picking}
                    onClick={() => trigger(mod.id, "setAssetPicker", !picking)}><PipetteIcon />
                    <span>{picking ? text("AssetPickerFinish", "Aufnahme beenden") : text("AssetPicker", "Assets mit Pipette aufnehmen")}</span></button></Tooltip>
                <Tooltip title={text("AssetListClear", "Liste leeren")}>
                <button type="button" className={!items.length ? styles.disabled : ""} disabled={!items.length}
                    onClick={() => trigger(mod.id, "clearAssetFilter")}>
                    <span>{text("AssetListClear", "Liste leeren")}</span></button></Tooltip>
                <Tooltip title={`${text("AssetListUndo", "Listenänderung rückgängig")} (${undoCount})`}
                    description={text("AssetListUndoHint", "Letzte Änderung der Assetliste rückgängig machen: Pipettenauswahl, Entfernen, Leeren oder Assetfilter umschalten. Der Undo-Pfeil im Werkzeug stellt gelöschte Objekte wieder her.")}>
                    <button type="button" className={`${styles.undoButton} ${undoCount <= 0 ? styles.disabled : ""}`}
                        aria-disabled={undoCount <= 0}
                        onClick={() => { if (undoCount > 0) trigger(mod.id, "undoAssetSelection"); }}>
                        <UndoIcon />
                    </button>
                </Tooltip>
            </div>
            <Tooltip title={text("AssetFilterEnabled", "Nur ausgewählte Assets")} description={text("AssetFilterHint", "Die Liste schränkt die aktiven Objektfilter zusätzlich ein. Höhenfilter und Schutzoptionen gelten weiterhin.")}>
            <button type="button" className={`${styles.mode} ${enabled ? styles.selected : ""}`}
                aria-pressed={enabled}
                onClick={() => trigger(mod.id, "setAssetFilterEnabled", !enabled)}>
                <span className={styles.checkMark} aria-hidden="true">{enabled && <svg viewBox="0 0 16 16"><path d="m3 8 3 3 7-8" /></svg>}</span>
                <span>{text("AssetFilterEnabled", "Nur ausgewählte Assets")}</span>
            </button></Tooltip>
            {items.length >= 128 && <div className={styles.hint}>{text("AssetListLimit", "Die Liste kann bis zu 128 Assets enthalten. Zum Hinzufügen einen Eintrag entfernen.")}</div>}
        </div>
        <div className={styles.list}>
            {!items.length && <div className={styles.empty}>{enabled
                ? text("AssetFilterEmpty", "Die Liste ist leer. Mit aktivem Assetfilter wird nichts gelöscht.")
                : text("AssetListEmpty", "Noch keine Assets. Mit der Pipette Objekte im Spiel aufnehmen.")}</div>}
            {items.map(item => <div key={item.id} className={`${styles.item} ${!item.available ? styles.unavailable : ""}`}>
                <AssetIcon src={item.icon} />
                <Tooltip title={item.name} description={!item.available ? text("AssetUnavailable", "Asset nicht verfügbar") : undefined}>
                <div className={styles.name}>{item.name}
                    {!item.available && <small>{text("AssetUnavailable", "Asset nicht verfügbar")}</small>}</div></Tooltip>
                <Tooltip title={text("AssetRemove", "Aus Liste entfernen")}>
                <button type="button"
                    aria-label={`${text("AssetRemove", "Aus Liste entfernen")}: ${item.name}`}
                    onClick={() => trigger(mod.id, "removeAssetFilterItem", item.id)}>×</button></Tooltip>
            </div>)}
        </div>
        <Tooltip title={text("AssetListResize", "Höhe der Assetliste ändern")}>
        <div className={styles.resizeGrip} role="separator" aria-orientation="horizontal"
            aria-label={text("AssetListResize", "Höhe der Assetliste ändern")}
            onMouseDown={event => {
                if (event.button !== 0) return;
                event.preventDefault();
                event.stopPropagation();
                const rect = panel.current?.getBoundingClientRect();
                if (!rect) return;
                resizeSession.current = { y: event.clientY, height: rect.height };
                setDragging(true);
                setPointer(true);
            }} /></Tooltip>
    </div>;
}
