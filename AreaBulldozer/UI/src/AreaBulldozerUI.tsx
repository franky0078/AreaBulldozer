import React, {
  useCallback,
  useEffect,
  useRef,
  useState,
} from "react";
import { trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "mod.json";
import styles from "./AreaBulldozerUI.module.scss";
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
  launcherButtonMovable$,
  launcherPositionX$,
  launcherPositionY$,
  squareRotationDegrees$,
  useSquareBrush$,
  useUniversalModMenu$,
  uiScale$,
} from "./bindings";

interface ToggleRowProps {
  label: string;
  checked: boolean;
  triggerKey: string;
  disabled?: boolean;
}

interface SliderRowProps {
  label: string;
  value: number;
  min: number;
  max: number;
  step: number;
  triggerKey: string;
  suffix: string;
  decimals?: number;
}

type FilterIconType =
  | "vegetation"
  | "building"
  | "road"
  | "path"
  | "rail"
  | "surface"
  | "props";

interface IconFilterButtonProps {
  label: string;
  checked: boolean;
  triggerKey: string;
  icon: FilterIconType;
  wide?: boolean;
}

const UI_PREFIX = "AreaBulldozer.UI.";

interface LauncherPosition {
  x: number;
  y: number;
}

interface LauncherDragSession {
  startMouseX: number;
  startMouseY: number;
  startPosition: LauncherPosition;
}

const LAUNCHER_EDGE_MARGIN = 8;
const LAUNCHER_BUTTON_SIZE = 48;
const LOWER_GAME_UI_RESERVE = 170;

function clamp(value: number, min: number, max: number) {
  return Math.min(max, Math.max(min, value));
}

function clampLauncherPosition(x: number, y: number): LauncherPosition {
  const viewportWidth =
    typeof window !== "undefined" ? window.innerWidth : 1920;
  const viewportHeight =
    typeof window !== "undefined" ? window.innerHeight : 1080;

  const maxX = Math.max(
    LAUNCHER_EDGE_MARGIN,
    viewportWidth - LAUNCHER_BUTTON_SIZE - LAUNCHER_EDGE_MARGIN
  );
  const maxY = Math.max(
    LAUNCHER_EDGE_MARGIN,
    viewportHeight - LOWER_GAME_UI_RESERVE - LAUNCHER_BUTTON_SIZE
  );

  return {
    x: Math.round(clamp(x, LAUNCHER_EDGE_MARGIN, maxX)),
    y: Math.round(clamp(y, LAUNCHER_EDGE_MARGIN, maxY)),
  };
}

function snapToStep(value: number, min: number, max: number, step: number) {
  const safeStep = step > 0 ? step : 1;
  const stepped = Math.round((value - min) / safeStep) * safeStep + min;
  return clamp(stepped, min, max);
}

/**
 * Uses a real button instead of a native checkbox. Native checkbox change
 * events are unreliable in the Cities: Skylines II embedded browser, while
 * button click triggers use the same proven path as the circle/square buttons.
 */
function ToggleRow({
  label,
  checked,
  triggerKey,
  disabled = false,
}: ToggleRowProps) {
  const toggle = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();

      if (!disabled) {
        trigger(mod.id, triggerKey, !checked);
      }
    },
    [checked, disabled, triggerKey]
  );

  return (
    <button
      type="button"
      className={`${styles.toggleRow} ${
        checked ? styles.toggleRowActive : ""
      } ${disabled ? styles.disabledRow : ""}`}
      aria-pressed={checked}
      disabled={disabled}
      onClick={toggle}
    >
      <span className={styles.toggleVisual} aria-hidden="true">
        <span className={styles.toggleKnob} />
      </span>
      <span className={styles.toggleLabel}>{label}</span>
    </button>
  );
}

function FilterIcon({ type }: { type: FilterIconType }) {
  const common = {
    viewBox: "0 0 32 32",
    fill: "none",
    stroke: "currentColor",
    strokeWidth: 2,
    strokeLinecap: "round" as const,
    strokeLinejoin: "round" as const,
    "aria-hidden": true,
  };

  switch (type) {
    case "vegetation":
      return (
        <svg {...common}>
          <path d="M16 4 9 14h4l-6 8h7v6h4v-6h7l-6-8h4L16 4Z" />
        </svg>
      );
    case "building":
      return (
        <svg {...common}>
          <path d="M6 27V8h13v19M19 13h7v14M10 12h4M10 17h4M10 22h4M22 17h1M22 22h1" />
        </svg>
      );
    case "road":
      return (
        <svg {...common}>
          <path d="M9 28 12.5 4M23 28 19.5 4" />
          <path d="M16 5v4M16 12v4M16 19v4M16 26v2" />
        </svg>
      );
    case "path":
      return (
        <svg {...common}>
          <path d="M7 27c2-8 6-7 8-13 2-5-1-7 3-10M14 27c1-5 4-6 6-10 2-4 1-7 5-10" />
          <circle cx="8" cy="8" r="2" />
        </svg>
      );
    case "rail":
      return (
        <svg {...common}>
          <path d="M10 4v24M22 4v24M9 8h14M8 14h16M8 20h16M9 26h14" />
        </svg>
      );
    case "surface":
      return (
        <svg {...common}>
          <path d="m5 11 10-6 12 7-10 6L5 11Z" />
          <path d="m5 17 12 7 10-6M5 22l12 7 10-6" />
        </svg>
      );
    default:
      return (
        <svg {...common}>
          <path d="M6 24h20M9 24V12h14v12M12 12V8h8v4" />
          <circle cx="12" cy="18" r="2" />
          <circle cx="20" cy="18" r="2" />
        </svg>
      );
  }
}


function BulldozerIcon({ className }: { className?: string }) {
  return (
    <svg
      className={className}
      viewBox="0 0 64 48"
      fill="none"
      stroke="currentColor"
      strokeWidth="2.2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      {/* Front bucket */}
      <path d="M4 31L10 24H28L36 27L33 40H9L4 31Z" />
      <path d="M6 33H34" />
      <path d="M9 40H33" />

      {/* Loader arms and hydraulic linkage */}
      <path d="M23 22L30 16L38 20" />
      <path d="M27 24L33 18" />
      <circle cx="30" cy="16" r="1.6" />
      <circle cx="38" cy="20" r="1.6" />
      <path d="M30 16V24" />

      {/* Cab */}
      <path d="M38 8L52 7L58 10V24H40L38 8Z" />
      <path d="M41 11L50 10.5L50 21H41V11Z" />
      <path d="M52 10L56 12V22H52V10Z" />
      <path d="M40 7.5L53 6.8L58 9.5" />

      {/* Mirrors */}
      <path d="M37 10H34V16" />
      <path d="M56 10H59V16" />

      {/* Chassis and fenders */}
      <path d="M36 24H56L60 27V33" />
      <path d="M35 29H43" />
      <path d="M44 31C44 26 48 22 53 22C58 22 61 26 61 31" />

      {/* Wheels */}
      <circle cx="51" cy="34" r="8" />
      <circle cx="51" cy="34" r="3.5" />
      <circle cx="61" cy="36" r="5" />
      <circle cx="61" cy="36" r="2" />
    </svg>
  );
}

function IconFilterButton({
  label,
  checked,
  triggerKey,
  icon,
  wide = false,
}: IconFilterButtonProps) {
  const toggle = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      trigger(mod.id, triggerKey, !checked);
    },
    [checked, triggerKey]
  );

  return (
    <button
      type="button"
      className={`${styles.iconFilterButton} ${
        checked ? styles.iconFilterButtonActive : ""
      } ${wide ? styles.iconFilterButtonWide : ""}`}
      aria-pressed={checked}
      title={label}
      onClick={toggle}
    >
      <span className={styles.iconFilterGraphic}>
        <FilterIcon type={icon} />
      </span>
      <span className={styles.iconFilterLabel}>{label}</span>
      <span className={styles.iconFilterState} aria-hidden="true" />
    </button>
  );
}

/**
 * Custom slider because the embedded game browser can render
 * <input type="range"> as a normal text field. The track below works with
 * mouse clicks and dragging and sends the selected numeric value to C#.
 */
function SliderRow({
  label,
  value,
  min,
  max,
  step,
  triggerKey,
  suffix,
  decimals = 0,
}: SliderRowProps) {
  const trackRef = useRef<HTMLButtonElement>(null);
  const draggingRef = useRef(false);
  const lastSentValueRef = useRef<number | null>(null);
  const [dragging, setDragging] = useState(false);

  const safeValue = clamp(value, min, max);
  const percentage = max > min ? ((safeValue - min) / (max - min)) * 100 : 0;

  const sendValue = useCallback(
    (nextValue: number) => {
      const snappedValue = snapToStep(nextValue, min, max, step);

      // Avoid flooding the C# binding with the same value while dragging.
      if (lastSentValueRef.current === snappedValue) {
        return;
      }

      lastSentValueRef.current = snappedValue;
      trigger(mod.id, triggerKey, snappedValue);
    },
    [max, min, step, triggerKey]
  );

  const sendValueFromClientX = useCallback(
    (clientX: number) => {
      const track = trackRef.current;
      if (!track) return;

      const bounds = track.getBoundingClientRect();
      if (bounds.width <= 0) return;

      const ratio = clamp((clientX - bounds.left) / bounds.width, 0, 1);
      sendValue(min + ratio * (max - min));
    },
    [max, min, sendValue]
  );

  const stopDragging = useCallback(() => {
    draggingRef.current = false;
    setDragging(false);
    lastSentValueRef.current = null;
  }, []);


  const beginDrag = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      if (event.button !== 0) {
        return;
      }

      event.preventDefault();
      event.stopPropagation();
      draggingRef.current = true;
      lastSentValueRef.current = null;
      setDragging(true);
      sendValueFromClientX(event.clientX);
    },
    [sendValueFromClientX]
  );

  const moveDrag = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      if (!draggingRef.current) {
        return;
      }

      event.preventDefault();
      event.stopPropagation();
      sendValueFromClientX(event.clientX);
    },
    [sendValueFromClientX]
  );

  const endDrag = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      stopDragging();
    },
    [stopDragging]
  );

  const clickTrack = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      sendValueFromClientX(event.clientX);
      lastSentValueRef.current = null;
    },
    [sendValueFromClientX]
  );

  const decrease = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      lastSentValueRef.current = null;
      sendValue(safeValue - step);
    },
    [safeValue, sendValue, step]
  );

  const increase = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      lastSentValueRef.current = null;
      sendValue(safeValue + step);
    },
    [safeValue, sendValue, step]
  );

  return (
    <div className={styles.sliderRow}>
      <div className={styles.sliderHeader}>
        <span className={styles.sliderLabel}>{label}</span>
        <span className={styles.sliderValue}>
          {safeValue.toFixed(decimals)} {suffix}
        </span>
      </div>
      <div className={styles.sliderControls}>
        <button
          type="button"
          className={styles.sliderStepButton}
          onClick={decrease}
          aria-label={`${label} minus`}
        >
          -
        </button>
        <button
          type="button"
          ref={trackRef}
          className={`${styles.sliderTrack} ${
            dragging ? styles.sliderTrackDragging : ""
          }`}
          onMouseDown={beginDrag}
          onMouseMove={moveDrag}
          onMouseUp={endDrag}
          onMouseLeave={endDrag}
          onClick={clickTrack}
          role="slider"
          aria-label={label}
          aria-valuemin={min}
          aria-valuemax={max}
          aria-valuenow={safeValue}
        >
          <span
            className={styles.sliderFill}
            style={{ width: `${percentage}%` }}
          />
          <span
            className={styles.sliderThumb}
            style={{ left: `${percentage}%` }}
          />
        </button>
        <button
          type="button"
          className={styles.sliderStepButton}
          onClick={increase}
          aria-label={`${label} plus`}
        >
          +
        </button>
      </div>
    </div>
  );
}

export function AreaBulldozerUI() {
  const { translate } = useLocalization();
  const [advancedOpen, setAdvancedOpen] = useState(false);
  const [launcherDragging, setLauncherDragging] = useState(false);

  const text = useCallback(
    (key: string, fallback: string) =>
      translate(`${UI_PREFIX}${key}`, fallback) ?? fallback,
    [translate]
  );

  const isToolActive = useValue(isToolActive$);
  const brushRadius = useValue(brushRadius$);
  const useSquareBrush = useValue(useSquareBrush$);
  const squareRotationDegrees = useValue(squareRotationDegrees$);
  const uiScalePercent = clamp(useValue(uiScale$), 75, 125);
  const uiScaleFactor = uiScalePercent / 100;

  const useUniversalModMenu = useValue(useUniversalModMenu$);
  const launcherButtonMovable = useValue(launcherButtonMovable$);
  const savedLauncherPositionX = useValue(launcherPositionX$);
  const savedLauncherPositionY = useValue(launcherPositionY$);

  const initialLauncherPosition = clampLauncherPosition(
    savedLauncherPositionX,
    savedLauncherPositionY
  );
  const [launcherPosition, setLauncherPosition] =
    useState<LauncherPosition>(initialLauncherPosition);
  const launcherPositionRef = useRef<LauncherPosition>(
    initialLauncherPosition
  );
  const launcherDragSessionRef = useRef<LauncherDragSession | null>(null);
  const suppressLauncherClickUntilRef = useRef(0);

  useEffect(() => {
    if (launcherDragging) {
      return;
    }

    const nextPosition = clampLauncherPosition(
      savedLauncherPositionX,
      savedLauncherPositionY
    );
    launcherPositionRef.current = nextPosition;
    setLauncherPosition(nextPosition);
  }, [
    launcherDragging,
    savedLauncherPositionX,
    savedLauncherPositionY,
  ]);

  // Keep the complete panel above the lower game HUD at every configured
  // scale. The unscaled maximum height is calculated so the transformed
  // visual height always reserves the same safe area at the bottom.
  const panelStyle = {
    transform: `scale(${uiScaleFactor})`,
    transformOrigin: "top left",
    maxHeight: `calc(${(100 / uiScaleFactor).toFixed(3)}vh - ${(
      240 / uiScaleFactor
    ).toFixed(2)}rem)`,
  } as React.CSSProperties;

  const deleteTrees = useValue(deleteTrees$);
  const deleteBuildings = useValue(deleteBuildings$);
  const deleteRoads = useValue(deleteRoads$);
  const deletePaths = useValue(deletePaths$);
  const deleteRailways = useValue(deleteRailways$);
  const deleteSurfaces = useValue(deleteSurfaces$);
  const deleteStaticObjects = useValue(deleteStaticObjects$);

  const deleteGeneralProps = useValue(deleteGeneralProps$);
  const deleteStreetLights = useValue(deleteStreetLights$);
  const deleteQuantityObjects = useValue(deleteQuantityObjects$);
  const deleteBrandingObjects = useValue(deleteBrandingObjects$);
  const deleteActivityLocations = useValue(deleteActivityLocations$);
  const deleteSpawnLocations = useValue(deleteSpawnLocations$);
  const deleteMarkerNetworks = useValue(deleteMarkerNetworks$);
  const dimMarkerBackground = useValue(dimMarkerBackground$);


  const setPointerLock = useCallback((value: boolean) => {
    trigger(mod.id, BindingKeys.setPointerOverUI, value);
  }, []);

  const finishLauncherDrag = useCallback(() => {
    if (!launcherDragSessionRef.current) {
      return;
    }

    launcherDragSessionRef.current = null;
    setLauncherDragging(false);
    setPointerLock(false);

    // Persist the final clamped position exactly when the drag ends.
    trigger(
      mod.id,
      BindingKeys.setLauncherPositionX,
      launcherPositionRef.current.x
    );
    trigger(
      mod.id,
      BindingKeys.setLauncherPositionY,
      launcherPositionRef.current.y
    );

    // Ignore only the synthetic click that Coherent UI may emit directly
    // after mouse-up. A later normal click must continue to toggle the tool.
    const now =
      typeof performance !== "undefined" ? performance.now() : Date.now();
    suppressLauncherClickUntilRef.current = now + 300;
  }, [setPointerLock]);

  const updateLauncherDrag = useCallback(
    (
      clientX: number,
      clientY: number,
      buttons: number,
      ctrlKey: boolean
    ) => {
      const session = launcherDragSessionRef.current;
      if (!session) {
        return;
      }

      // Coherent UI can occasionally lose the mouse-up event. Every movement
      // therefore verifies both the physical left-button state and Ctrl.
      // Releasing either one ends and saves the drag immediately.
      if ((buttons & 1) === 0 || !ctrlKey) {
        finishLauncherDrag();
        return;
      }

      const nextPosition = clampLauncherPosition(
        session.startPosition.x + clientX - session.startMouseX,
        session.startPosition.y + clientY - session.startMouseY
      );

      launcherPositionRef.current = nextPosition;
      setLauncherPosition(nextPosition);
    },
    [finishLauncherDrag]
  );

  const beginLauncherDrag = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      // A normal left click only toggles the tool. Moving the launcher starts
      // exclusively with Ctrl + left mouse button.
      if (
        !launcherButtonMovable ||
        event.button !== 0 ||
        !event.ctrlKey
      ) {
        return;
      }

      event.preventDefault();
      event.stopPropagation();

      launcherDragSessionRef.current = {
        startMouseX: event.clientX,
        startMouseY: event.clientY,
        startPosition: launcherPositionRef.current,
      };

      setLauncherDragging(true);
      setPointerLock(true);
    },
    [launcherButtonMovable, setPointerLock]
  );

  const handleLauncherDragMove = useCallback(
    (event: React.MouseEvent<HTMLDivElement>) => {
      event.preventDefault();
      event.stopPropagation();
      updateLauncherDrag(
        event.clientX,
        event.clientY,
        event.buttons,
        event.ctrlKey
      );
    },
    [updateLauncherDrag]
  );

  const handleLauncherDragMouseUp = useCallback(
    (event: React.MouseEvent<HTMLDivElement>) => {
      event.preventDefault();
      event.stopPropagation();
      if (event.button === 0) {
        finishLauncherDrag();
      }
    },
    [finishLauncherDrag]
  );

  useEffect(() => {
    if (!launcherDragging) {
      return;
    }

    // Capture-phase fallbacks run before other UI handlers can consume the
    // events. The full-screen overlay below remains the primary drag surface.
    const onMouseUp = (event: MouseEvent) => {
      if (event.button === 0) {
        finishLauncherDrag();
      }
    };
    const onKeyUp = (event: KeyboardEvent) => {
      if (
        event.key === "Control" ||
        event.code === "ControlLeft" ||
        event.code === "ControlRight"
      ) {
        finishLauncherDrag();
      }
    };
    const onBlur = () => finishLauncherDrag();
    const onVisibilityChange = () => {
      if (document.hidden) {
        finishLauncherDrag();
      }
    };

    document.addEventListener("mouseup", onMouseUp, true);
    document.addEventListener("keyup", onKeyUp, true);
    document.addEventListener("visibilitychange", onVisibilityChange, true);
    window.addEventListener("mouseup", onMouseUp, true);
    window.addEventListener("blur", onBlur, true);

    return () => {
      document.removeEventListener("mouseup", onMouseUp, true);
      document.removeEventListener("keyup", onKeyUp, true);
      document.removeEventListener("visibilitychange", onVisibilityChange, true);
      window.removeEventListener("mouseup", onMouseUp, true);
      window.removeEventListener("blur", onBlur, true);
    };
  }, [finishLauncherDrag, launcherDragging]);

  const stopEvent = useCallback((event: React.SyntheticEvent) => {
    event.stopPropagation();
  }, []);

  const toggleTool = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();

      const now =
        typeof performance !== "undefined" ? performance.now() : Date.now();
      if (now < suppressLauncherClickUntilRef.current) {
        return;
      }

      trigger(mod.id, BindingKeys.toggleTool);
    },
    []
  );

  const closeTool = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      trigger(mod.id, BindingKeys.deactivateTool);
      setPointerLock(false);
    },
    [setPointerLock]
  );

  const chooseCircle = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      trigger(mod.id, BindingKeys.setUseSquareBrush, false);
    },
    []
  );

  const chooseSquare = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      trigger(mod.id, BindingKeys.setUseSquareBrush, true);
    },
    []
  );

  const launcherStyle = launcherButtonMovable
    ? ({
        position: "fixed",
        left: `${launcherPosition.x}px`,
        top: `${launcherPosition.y}px`,
        margin: 0,
      } as React.CSSProperties)
    : undefined;

  const launcherTitle = launcherButtonMovable
    ? text(
        "LauncherDragHint",
        "Strg + Linksklick halten und ziehen; loslassen zum Speichern"
      )
    : text("ToggleTool", "Area Bulldozer umschalten");

  return (
    <div
      className={styles.host}
      onMouseEnter={() => setPointerLock(true)}
      onMouseLeave={() => setPointerLock(false)}
      onMouseDown={stopEvent}
      onMouseUp={stopEvent}
      onClick={stopEvent}
    >
      {!useUniversalModMenu && (
        <button
          type="button"
          className={`${styles.toolButton} ${
            isToolActive ? styles.toolButtonActive : ""
          } ${
            launcherButtonMovable ? styles.toolButtonMovable : ""
          } ${launcherDragging ? styles.toolButtonDragging : ""}`}
          style={launcherStyle}
          onMouseDown={beginLauncherDrag}
          onClick={toggleTool}
          title={launcherTitle}
        >
          <BulldozerIcon className={styles.launcherBulldozerIcon} />
        </button>
      )}

      {launcherDragging && (
        <div
          className={styles.launcherDragOverlay}
          onMouseMove={handleLauncherDragMove}
          onMouseUp={handleLauncherDragMouseUp}
          onMouseLeave={finishLauncherDrag}
          onContextMenu={stopEvent}
          aria-hidden="true"
        />
      )}

      {isToolActive && (
        <section className={styles.panel} style={panelStyle}>
          <header className={styles.panelHeader}>
            <div className={styles.headerIcon} aria-hidden="true">
              <BulldozerIcon className={styles.headerBulldozerIcon} />
            </div>
            <div className={styles.headerText}>
              <div className={styles.titleRow}>
                <h2 className={styles.title}>{text("Title", "Area Bulldozer")}</h2>
                <span className={styles.activeBadge}>{text("Active", "Aktiv")}</span>
              </div>
              <p className={styles.subtitle}>
                {text("Subtitle", "Mehrere Objekte in einem Bereich entfernen")}
              </p>
            </div>
            <button
              type="button"
              className={styles.closeButton}
              onClick={closeTool}
              title={text("Close", "Werkzeug schließen")}
            >
              X
            </button>
          </header>

          <div className={styles.scrollArea} onWheel={stopEvent}>
            <section className={styles.section}>
              <div className={styles.sectionHeadingRow}>
                <h3 className={styles.sectionTitle}>{text("Selection", "Auswahl")}</h3>
              </div>

              <div className={styles.segmentedControl}>
                <button
                  type="button"
                  className={`${styles.segmentButton} ${
                    !useSquareBrush ? styles.segmentButtonActive : ""
                  }`}
                  onClick={chooseCircle}
                  aria-pressed={!useSquareBrush}
                >
                  <span className={styles.circleSymbol} aria-hidden="true" />
                  <span className={styles.segmentLabel}>{text("Circle", "Kreis")}</span>
                </button>
                <button
                  type="button"
                  className={`${styles.segmentButton} ${
                    useSquareBrush ? styles.segmentButtonActive : ""
                  }`}
                  onClick={chooseSquare}
                  aria-pressed={useSquareBrush}
                >
                  <span className={styles.squareSymbol} aria-hidden="true" />
                  <span className={styles.segmentLabel}>{text("Square", "Quadrat")}</span>
                </button>
              </div>

              <div
                className={`${styles.brushSliderGrid} ${
                  !useSquareBrush ? styles.brushSliderGridSingle : ""
                }`}
              >
                <SliderRow
                  label={
                    useSquareBrush
                      ? text("HalfSide", "Größe Quadrat")
                      : text("Radius", "Radius")
                  }
                  value={brushRadius}
                  min={5}
                  max={200}
                  step={5}
                  triggerKey={BindingKeys.setBrushRadius}
                  suffix="m"
                />

                {useSquareBrush && (
                  <SliderRow
                    label={text("Rotation", "Drehung")}
                    value={squareRotationDegrees}
                    min={0}
                    max={359}
                    step={1}
                    triggerKey={BindingKeys.setSquareRotationDegrees}
                    suffix={text("Degrees", "Grad")}
                  />
                )}
              </div>
            </section>

            <section className={styles.section}>
              <div className={styles.sectionHeadingRow}>
                <h3 className={styles.sectionTitle}>
                  {text("MainFilters", "Objektfilter")}
                </h3>
                <span className={styles.sectionHint}>
                  {text("ChooseFilters", "Auswahl festlegen")}
                </span>
              </div>

              <div className={styles.iconFilterGrid}>
                <div className={styles.iconFilterRow}>
                  <IconFilterButton
                    label={text("VegetationShort", "Vegetation")}
                    checked={deleteTrees}
                    triggerKey={BindingKeys.setDeleteTrees}
                    icon="vegetation"
                  />
                  <IconFilterButton
                    label={text("Buildings", "Gebäude")}
                    checked={deleteBuildings}
                    triggerKey={BindingKeys.setDeleteBuildings}
                    icon="building"
                  />
                </div>
                <div className={styles.iconFilterRow}>
                  <IconFilterButton
                    label={text("Roads", "Straßen")}
                    checked={deleteRoads}
                    triggerKey={BindingKeys.setDeleteRoads}
                    icon="road"
                  />
                  <IconFilterButton
                    label={text("Paths", "Fußwege")}
                    checked={deletePaths}
                    triggerKey={BindingKeys.setDeletePaths}
                    icon="path"
                  />
                </div>
                <div className={styles.iconFilterRow}>
                  <IconFilterButton
                    label={text("Railways", "Gleise")}
                    checked={deleteRailways}
                    triggerKey={BindingKeys.setDeleteRailways}
                    icon="rail"
                  />
                  <IconFilterButton
                    label={text("SurfacesShort", "Flächen")}
                    checked={deleteSurfaces}
                    triggerKey={BindingKeys.setDeleteSurfaces}
                    icon="surface"
                  />
                </div>
                <IconFilterButton
                  label={text("StaticObjectsShort", "Props / Marker")}
                  checked={deleteStaticObjects}
                  triggerKey={BindingKeys.setDeleteStaticObjects}
                  icon="props"
                  wide
                />
              </div>
            </section>

            <section className={styles.accordionSection}>
              <button
                type="button"
                className={styles.sectionToggle}
                onClick={(event) => {
                  event.preventDefault();
                  event.stopPropagation();
                  setAdvancedOpen((open) => !open);
                }}
                aria-expanded={advancedOpen}
              >
                <span className={styles.accordionTitle}>
                  {text("AdvancedFilters", "Erweiterte Prop- und Markerfilter")}
                </span>
                <span
                  className={`${styles.chevron} ${
                    advancedOpen ? styles.chevronOpen : ""
                  }`}
                  aria-hidden="true"
                />
              </button>

              {advancedOpen && (
                <div className={styles.sectionBody}>
                  {!deleteStaticObjects && (
                    <p className={styles.notice}>
                      {text(
                        "StaticMasterNotice",
                        "Aktiviere zuerst Props und Marker im Objektfilter."
                      )}
                    </p>
                  )}

                  <div className={styles.toggleGrid}>
                    <ToggleRow
                      label={text("GeneralProps", "Allgemeine Props")}
                      checked={deleteGeneralProps}
                      triggerKey={BindingKeys.setDeleteGeneralProps}
                      disabled={!deleteStaticObjects}
                    />
                    <ToggleRow
                      label={text("StreetLights", "Straßenlaternen")}
                      checked={deleteStreetLights}
                      triggerKey={BindingKeys.setDeleteStreetLights}
                      disabled={!deleteStaticObjects}
                    />
                    <ToggleRow
                      label={text("QuantityObjects", "Mülleimer und Mengenobjekte")}
                      checked={deleteQuantityObjects}
                      triggerKey={BindingKeys.setDeleteQuantityObjects}
                      disabled={!deleteStaticObjects}
                    />
                    <ToggleRow
                      label={text("Branding", "Werbung und Branding")}
                      checked={deleteBrandingObjects}
                      triggerKey={BindingKeys.setDeleteBrandingObjects}
                      disabled={!deleteStaticObjects}
                    />
                    <ToggleRow
                      label={text("ActivityLocations", "Aktivitätspunkte")}
                      checked={deleteActivityLocations}
                      triggerKey={BindingKeys.setDeleteActivityLocations}
                      disabled={!deleteStaticObjects}
                    />
                    <ToggleRow
                      label={text("SpawnLocations", "Spawnpunkte")}
                      checked={deleteSpawnLocations}
                      triggerKey={BindingKeys.setDeleteSpawnLocations}
                      disabled={!deleteStaticObjects}
                    />
                    <ToggleRow
                      label={text("AssetLanes", "Asset-Lanes / SubLanes")}
                      checked={deleteMarkerNetworks}
                      triggerKey={BindingKeys.setDeleteMarkerNetworks}
                      disabled={!deleteStaticObjects}
                    />
                  </div>

                  <div className={styles.subSection}>
                    <ToggleRow
                      label={text("DimBackground", "Marker-Hintergrund abdunkeln")}
                      checked={dimMarkerBackground}
                      triggerKey={BindingKeys.setDimMarkerBackground}
                    />
                  </div>
                </div>
              )}
            </section>

          </div>

          <footer className={styles.footer}>
            <div className={styles.helpLine}>
              <span className={styles.mouseKey}>LMB</span>
              <span>{text("ApplyHelp", "Ausgewählte Objekte löschen")}</span>
            </div>
            {useSquareBrush && (
              <div className={styles.helpLine}>
                <span className={styles.mouseKey}>RMB</span>
                <span>{text("RotateHelp", "Gedrückt halten und drehen")}</span>
              </div>
            )}
            <div className={styles.helpLine}>
              <span className={styles.keyCap}>Shift+B</span>
              <span>{text("ShortcutHelp", "Werkzeug umschalten")}</span>
            </div>
          </footer>
        </section>
      )}
    </div>
  );
}

/**
 * Optional launcher displayed inside the game's universal mod menu.
 * The normal GameTopLeft component remains registered because it owns the
 * actual Area Bulldozer panel; only the separate floating launcher is hidden.
 */
export function AreaBulldozerModMenuButton() {
  const { translate } = useLocalization();
  const useUniversalModMenu = useValue(useUniversalModMenu$);
  const isToolActive = useValue(isToolActive$);

  const text = useCallback(
    (key: string, fallback: string) =>
      translate(`${UI_PREFIX}${key}`, fallback) ?? fallback,
    [translate]
  );

  const toggleTool = useCallback(
    (event: React.MouseEvent<HTMLButtonElement>) => {
      event.preventDefault();
      event.stopPropagation();
      trigger(mod.id, BindingKeys.toggleTool);
    },
    []
  );

  if (!useUniversalModMenu) {
    return null;
  }

  return (
    <button
      type="button"
      className={`${styles.modMenuButton} ${
        isToolActive ? styles.modMenuButtonActive : ""
      }`}
      onClick={toggleTool}
      title={text("ToggleTool", "Area Bulldozer umschalten")}
      aria-pressed={isToolActive}
    >
      <span className={styles.modMenuIcon} aria-hidden="true">
        <BulldozerIcon className={styles.modMenuBulldozerIcon} />
      </span>
      <span className={styles.modMenuText}>
        <span className={styles.modMenuTitle}>
          {text("Title", "Area Bulldozer")}
        </span>
        <span className={styles.modMenuSubtitle}>
          {isToolActive
            ? text("Active", "Aktiv")
            : text("ModMenuOpen", "Werkzeug öffnen")}
        </span>
      </span>
      <span className={styles.modMenuState} aria-hidden="true" />
    </button>
  );
}
