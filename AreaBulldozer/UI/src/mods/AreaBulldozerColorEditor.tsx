import React from "react";

import { trigger } from "cs2/api";
import mod from "mod.json";

import {
    BindingKeys,
    confirmationColor$,
    deleteColor$,
    invalidPolygonColor$,
    selectionColor$,
    surfaceColor$,
} from "../bindings";

import styles from "./AreaBulldozerColorEditor.module.scss";
import { useSafeValue } from "./useSafeValue";


type ColorKey =
    | "selection"
    | "confirmation"
    | "invalidPolygon"
    | "delete"
    | "surface";


interface ColorEditorProps {
    scale: number;
    text: (key: string, fallback: string) => string;
    dragging?: boolean;
    onDragStart?: (
        event: React.MouseEvent<HTMLDivElement>
    ) => void;
}


interface ColorDefinition {
    key: ColorKey;
    label: string;
    value: number;
    defaultValue: number;
    triggerKey: string;
}


const PRESET_COLORS = [
    0xff401a,
    0xff8a1a,
    0xffd91a,
    0x1ff238,
    0x14dbff,
    0x2688ff,
    0x8757ff,
    0xe74cff,
    0xff4c91,
    0xffffff,
    0xa7b4bd,
    0x20262a,
];


function clampChannel(value: number) {
    return Math.min(255, Math.max(0, Math.round(value)));
}


function unpackColor(value: number) {
    return {
        red: (value >> 16) & 0xff,
        green: (value >> 8) & 0xff,
        blue: value & 0xff,
    };
}


function packColor(
    red: number,
    green: number,
    blue: number
) {
    return (
        (clampChannel(red) << 16) |
        (clampChannel(green) << 8) |
        clampChannel(blue)
    );
}


function colorToHex(value: number) {
    return `#${(value & 0xffffff)
        .toString(16)
        .padStart(6, "0")
        .toUpperCase()}`;
}


export function AreaBulldozerColorEditor({
    scale,
    text,
    dragging = false,
    onDragStart,
}: ColorEditorProps) {
    const selectionColor = useSafeValue(
        "selectionColor$",
        selectionColor$,
        0xff401a
    );

    const confirmationColor = useSafeValue(
        "confirmationColor$",
        confirmationColor$,
        0xffd91a
    );

    const deleteColor = useSafeValue(
        "deleteColor$",
        deleteColor$,
        0x1ff238
    );

    const invalidPolygonColor = useSafeValue(
        "invalidPolygonColor$",
        invalidPolygonColor$,
        0xff4c91
    );

    const surfaceColor = useSafeValue(
        "surfaceColor$",
        surfaceColor$,
        0x14dbff
    );

    const [activeKey, setActiveKey] =
        React.useState<ColorKey>("selection");

    const colors: ColorDefinition[] = [
        {
            key: "selection",
            label: text("ColorSelection", "Normale Auswahl"),
            value: selectionColor,
            defaultValue: 0xff401a,
            triggerKey: BindingKeys.setSelectionColor,
        },
        {
            key: "confirmation",
            label: text("ColorConfirmation", "Bestätigung"),
            value: confirmationColor,
            defaultValue: 0xffd91a,
            triggerKey: BindingKeys.setConfirmationColor,
        },
        {
            key: "invalidPolygon",
            label: text("ColorInvalidPolygon", "Ungültiges Polygon"),
            value: invalidPolygonColor,
            defaultValue: 0xff4c91,
            triggerKey: BindingKeys.setInvalidPolygonColor,
        },
        {
            key: "delete",
            label: text("ColorDeletion", "Aktiver Löschvorgang"),
            value: deleteColor,
            defaultValue: 0x1ff238,
            triggerKey: BindingKeys.setDeleteColor,
        },
        {
            key: "surface",
            label: text("ColorSurfaces", "Ausgewählte Objekte"),
            value: surfaceColor,
            defaultValue: 0x14dbff,
            triggerKey: BindingKeys.setSurfaceColor,
        },
    ];

    const activeColor =
        colors.find(color => color.key === activeKey) ??
        colors[0];

    const channels = unpackColor(activeColor.value);
    const colorHex = colorToHex(activeColor.value);

    const [hexDraft, setHexDraft] =
        React.useState(colorHex);

    React.useEffect(
        () => setHexDraft(colorHex),
        [colorHex, activeKey]
    );

    const setColor = React.useCallback(
        (value: number) => {
            trigger(
                mod.id,
                activeColor.triggerKey,
                value & 0xffffff
            );
        },
        [activeColor.triggerKey]
    );

    const setChannel = (
        channel: "red" | "green" | "blue",
        value: number
    ) => {
        setColor(
            packColor(
                channel === "red" ? value : channels.red,
                channel === "green" ? value : channels.green,
                channel === "blue" ? value : channels.blue
            )
        );
    };

    const applyHexDraft = () => {
        const normalized = hexDraft.trim();

        if (!/^#[0-9a-f]{6}$/i.test(normalized)) {
            setHexDraft(colorHex);
            return;
        }

        setColor(parseInt(normalized.substring(1), 16));
    };

    const editorStyle = {
        "--ab-current-color": colorHex,
        fontSize: `${(12 * scale).toFixed(1)}rem`,
    } as React.CSSProperties;

    return (
        <div
            className={styles.editor}
            style={editorStyle}
            aria-label={text("ColorEditor", "Farbeditor")}
        >
            <div
                className={[
                    styles.titleRow,
                    onDragStart ? styles.titleRowDraggable : "",
                    dragging ? styles.titleRowDragging : "",
                ]
                    .filter(Boolean)
                    .join(" ")}
                onMouseDown={onDragStart}
            >
                <strong>{text("ColorEditor", "Farbeditor")}</strong>
                <span>{activeColor.label}</span>
            </div>

            <div className={styles.editorBody}>
                <div className={styles.stateColumn}>
                    {colors.map(color => {
                        const hex = colorToHex(color.value);

                        return (
                            <button
                                key={color.key}
                                type="button"
                                className={[
                                    styles.stateButton,
                                    color.key === activeKey
                                        ? styles.stateButtonSelected
                                        : "",
                                ]
                                    .filter(Boolean)
                                    .join(" ")}
                                onClick={() => setActiveKey(color.key)}
                                aria-pressed={color.key === activeKey}
                            >
                                <span
                                    className={styles.stateSwatch}
                                    style={{ backgroundColor: hex }}
                                    aria-hidden="true"
                                />
                                <span>{color.label}</span>
                            </button>
                        );
                    })}
                </div>

                <div className={styles.controlsColumn}>
                    <div className={styles.previewArea}>
                        <div className={styles.previewShape} />
                        <div className={styles.previewText}>
                            <strong>{activeColor.label}</strong>
                            <span>{colorHex}</span>
                        </div>
                    </div>

                    <div className={styles.presetArea}>
                        <span className={styles.areaLabel}>
                            {text("ColorPresets", "Farbvorgaben")}
                        </span>

                        <div className={styles.presetList}>
                            {PRESET_COLORS.map(value => {
                                const hex = colorToHex(value);

                                return (
                                    <button
                                        key={value}
                                        type="button"
                                        className={[
                                            styles.presetButton,
                                            value === activeColor.value
                                                ? styles.presetButtonSelected
                                                : "",
                                        ]
                                            .filter(Boolean)
                                            .join(" ")}
                                        style={{ backgroundColor: hex }}
                                        onClick={() => setColor(value)}
                                        aria-label={`${text("ChooseColor", "Farbe auswählen")} ${hex}`}
                                        aria-pressed={value === activeColor.value}
                                    />
                                );
                            })}
                        </div>
                    </div>

                    <ColorChannel
                        label={text("ColorRed", "Rot")}
                        value={channels.red}
                        start={`rgb(0, ${channels.green}, ${channels.blue})`}
                        end={`rgb(255, ${channels.green}, ${channels.blue})`}
                        onChange={value => setChannel("red", value)}
                    />

                    <ColorChannel
                        label={text("ColorGreen", "Grün")}
                        value={channels.green}
                        start={`rgb(${channels.red}, 0, ${channels.blue})`}
                        end={`rgb(${channels.red}, 255, ${channels.blue})`}
                        onChange={value => setChannel("green", value)}
                    />

                    <ColorChannel
                        label={text("ColorBlue", "Blau")}
                        value={channels.blue}
                        start={`rgb(${channels.red}, ${channels.green}, 0)`}
                        end={`rgb(${channels.red}, ${channels.green}, 255)`}
                        onChange={value => setChannel("blue", value)}
                    />

                    <div className={styles.hexRow}>
                        <label htmlFor="area-bulldozer-color-hex">
                            {text("ColorHex", "Hex-Wert")}
                        </label>
                        <input
                            id="area-bulldozer-color-hex"
                            className={styles.hexInput}
                            type="text"
                            value={hexDraft}
                            maxLength={7}
                            onChange={event => setHexDraft(event.target.value)}
                            onBlur={applyHexDraft}
                            onKeyDown={event => {
                                if (event.key === "Enter") {
                                    applyHexDraft();
                                }
                            }}
                        />
                    </div>

                    <div className={styles.actionRow}>
                        <button
                            type="button"
                            className={styles.actionButton}
                            onClick={() => setColor(activeColor.defaultValue)}
                        >
                            {text("ResetCurrentColor", "Diese Farbe zurücksetzen")}
                        </button>

                        <button
                            type="button"
                            className={styles.actionButton}
                            onClick={() => trigger(
                                mod.id,
                                BindingKeys.resetColors
                            )}
                        >
                            {text("ResetAllColors", "Alle Farben zurücksetzen")}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}


interface ColorChannelProps {
    label: string;
    value: number;
    start: string;
    end: string;
    onChange: (value: number) => void;
}


function ColorChannel({
    label,
    value,
    start,
    end,
    onChange,
}: ColorChannelProps) {
    const sliderRef = React.useRef<HTMLDivElement | null>(null);
    const [dragging, setDragging] = React.useState(false);

    const setValueFromPointer = React.useCallback(
        (clientX: number) => {
            const rect = sliderRef.current?.getBoundingClientRect();

            if (!rect || rect.width <= 0) {
                return;
            }

            const ratio = Math.min(
                1,
                Math.max(0, (clientX - rect.left) / rect.width)
            );

            onChange(Math.round(ratio * 255));
        },
        [onChange]
    );

    React.useEffect(() => {
        if (!dragging) {
            return;
        }

        const handleMove = (event: MouseEvent) => {
            event.preventDefault();
            setValueFromPointer(event.clientX);
        };

        const handleUp = () => setDragging(false);

        window.addEventListener("mousemove", handleMove);
        window.addEventListener("mouseup", handleUp);

        return () => {
            window.removeEventListener("mousemove", handleMove);
            window.removeEventListener("mouseup", handleUp);
        };
    }, [dragging, setValueFromPointer]);

    const setKeyboardValue = (nextValue: number) => {
        onChange(Math.min(255, Math.max(0, nextValue)));
    };

    return (
        <div className={styles.channelRow}>
            <span>{label}</span>
            <div
                ref={sliderRef}
                className={styles.channelSlider}
                style={{
                    background: `linear-gradient(90deg, ${start}, ${end})`,
                }}
                role="slider"
                tabIndex={0}
                aria-label={label}
                aria-valuemin={0}
                aria-valuemax={255}
                aria-valuenow={value}
                onMouseDown={event => {
                    event.preventDefault();
                    event.stopPropagation();
                    setValueFromPointer(event.clientX);
                    setDragging(true);
                }}
                onKeyDown={event => {
                    if (event.key === "ArrowLeft" || event.key === "ArrowDown") {
                        event.preventDefault();
                        setKeyboardValue(value - 1);
                    }
                    else if (event.key === "ArrowRight" || event.key === "ArrowUp") {
                        event.preventDefault();
                        setKeyboardValue(value + 1);
                    }
                    else if (event.key === "Home") {
                        event.preventDefault();
                        setKeyboardValue(0);
                    }
                    else if (event.key === "End") {
                        event.preventDefault();
                        setKeyboardValue(255);
                    }
                }}
            >
                <span
                    className={styles.channelSliderThumb}
                    style={{
                        left: `${(value / 255) * 100}%`,
                    }}
                    aria-hidden="true"
                />
            </div>
            <output>{value}</output>
        </div>
    );
}
