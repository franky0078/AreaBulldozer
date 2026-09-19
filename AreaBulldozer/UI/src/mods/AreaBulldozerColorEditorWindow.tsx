import React from "react";

import { trigger } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "mod.json";

import {
    BindingKeys,
    colorEditorPositionX$,
    colorEditorPositionY$,
    isToolActive$,
    uiScale$,
} from "../bindings";

import { AreaBulldozerColorEditor } from "./AreaBulldozerColorEditor";
import {
    closeColorEditor,
    useColorEditorOpen,
} from "./AreaBulldozerColorEditorState";
import styles from "./AreaBulldozerColorEditorWindow.module.scss";
import { useSafeValue } from "./useSafeValue";
import { VanillaComponentResolver } from "./VanillaComponentResolver";


const UI_PREFIX = "AreaBulldozer.UI.";
const DEFAULT_LEFT_RATIO = 0.5;
const DEFAULT_TOP_RATIO = 0.18;
const DEFAULT_WIDTH = 470;
const WINDOW_MARGIN = 16;


interface EditorPosition {
    x: number;
    y: number;
}


interface DragSession {
    pointerX: number;
    pointerY: number;
    startX: number;
    startY: number;
    width: number;
    height: number;
}


function clamp(value: number, min: number, max: number) {
    return Math.min(max, Math.max(min, value));
}


function resolveSavedPosition(
    savedX: number,
    savedY: number,
    width = DEFAULT_WIDTH,
    height = 320
): EditorPosition {
    const viewportWidth =
        typeof window !== "undefined"
            ? window.innerWidth
            : 1920;

    const viewportHeight =
        typeof window !== "undefined"
            ? window.innerHeight
            : 1080;

    const hasSavedPosition =
        savedX >= 0 && savedY >= 0;

    const defaultX =
        viewportWidth * DEFAULT_LEFT_RATIO;

    const defaultY =
        viewportHeight * DEFAULT_TOP_RATIO;

    return {
        x: clamp(
            hasSavedPosition ? savedX : defaultX,
            WINDOW_MARGIN,
            Math.max(WINDOW_MARGIN, viewportWidth - width - WINDOW_MARGIN)
        ),
        y: clamp(
            hasSavedPosition ? savedY : defaultY,
            WINDOW_MARGIN,
            Math.max(WINDOW_MARGIN, viewportHeight - height - WINDOW_MARGIN)
        ),
    };
}


export function AreaBulldozerColorEditorWindow() {
    const { translate } = useLocalization();
    const open = useColorEditorOpen();

    const isToolActive = useSafeValue(
        "isToolActive$",
        isToolActive$,
        false
    );

    const uiScalePercent = useSafeValue(
        "uiScale$",
        uiScale$,
        100
    );

    const savedX = useSafeValue(
        "colorEditorPositionX$",
        colorEditorPositionX$,
        -1
    );

    const savedY = useSafeValue(
        "colorEditorPositionY$",
        colorEditorPositionY$,
        -1
    );

    const panelRef = React.useRef<HTMLDivElement | null>(null);
    const dragSessionRef = React.useRef<DragSession | null>(null);
    const positionRef = React.useRef<EditorPosition>(
        resolveSavedPosition(savedX, savedY)
    );

    const [position, setPosition] = React.useState<EditorPosition>(
        positionRef.current
    );

    const [dragging, setDragging] = React.useState(false);

    const text = React.useCallback(
        (key: string, fallback: string) =>
            translate(`${UI_PREFIX}${key}`, fallback) ?? fallback,
        [translate]
    );

    const applyPosition = React.useCallback((next: EditorPosition) => {
        positionRef.current = next;
        setPosition(next);
    }, []);

    React.useEffect(() => {
        if (!isToolActive) {
            closeColorEditor();
        }
    }, [isToolActive]);

    React.useEffect(() => {
        if (dragging) {
            return;
        }

        const rect = panelRef.current?.getBoundingClientRect();

        applyPosition(
            resolveSavedPosition(
                savedX,
                savedY,
                rect?.width ?? DEFAULT_WIDTH,
                rect?.height ?? 320
            )
        );
    }, [savedX, savedY, dragging, applyPosition]);

    React.useEffect(() => {
        const handleResize = () => {
            const rect = panelRef.current?.getBoundingClientRect();

            applyPosition(
                resolveSavedPosition(
                    positionRef.current.x,
                    positionRef.current.y,
                    rect?.width ?? DEFAULT_WIDTH,
                    rect?.height ?? 320
                )
            );
        };

        window.addEventListener("resize", handleResize);

        return () => window.removeEventListener("resize", handleResize);
    }, [applyPosition]);

    React.useEffect(() => {
        if (!dragging) {
            return;
        }

        const handleMove = (event: MouseEvent) => {
            const session = dragSessionRef.current;

            if (!session) {
                return;
            }

            const next = resolveSavedPosition(
                session.startX + event.clientX - session.pointerX,
                session.startY + event.clientY - session.pointerY,
                session.width,
                session.height
            );

            applyPosition(next);
        };

        const handleUp = (event: MouseEvent) => {
            dragSessionRef.current = null;
            setDragging(false);

            const finalPosition = positionRef.current;

            trigger(
                mod.id,
                BindingKeys.setColorEditorPositionX,
                Math.round(finalPosition.x)
            );

            trigger(
                mod.id,
                BindingKeys.setColorEditorPositionY,
                Math.round(finalPosition.y)
            );

            const rect = panelRef.current?.getBoundingClientRect();
            const pointerInside = rect
                ? event.clientX >= rect.left &&
                  event.clientX <= rect.right &&
                  event.clientY >= rect.top &&
                  event.clientY <= rect.bottom
                : false;

            if (!pointerInside) {
                trigger(
                    mod.id,
                    BindingKeys.setPointerOverUI,
                    false
                );
            }
        };

        window.addEventListener("mousemove", handleMove);
        window.addEventListener("mouseup", handleUp);

        return () => {
            window.removeEventListener("mousemove", handleMove);
            window.removeEventListener("mouseup", handleUp);
        };
    }, [dragging, applyPosition]);

    if (!open || !isToolActive) {
        return null;
    }

    const panelTheme =
        VanillaComponentResolver.instance
            .toolOptionsPanelTheme
            ?.toolOptionsPanel;

    return (
        <div
            ref={panelRef}
            className={[
                panelTheme ?? "",
                styles.window,
                dragging ? styles.dragging : "",
            ]
                .filter(Boolean)
                .join(" ")}
            style={{
                left: `${position.x}px`,
                top: `${position.y}px`,
            }}
            onMouseEnter={() => trigger(
                mod.id,
                BindingKeys.setPointerOverUI,
                true
            )}
            onMouseLeave={() => {
                if (!dragging) {
                    trigger(
                        mod.id,
                        BindingKeys.setPointerOverUI,
                        false
                    );
                }
            }}
        >
            <AreaBulldozerColorEditor
                scale={clamp(uiScalePercent, 50, 125) / 100}
                text={text}
                dragging={dragging}
                onDragStart={event => {
                    event.preventDefault();
                    event.stopPropagation();

                    const rect = panelRef.current?.getBoundingClientRect();

                    if (!rect) {
                        return;
                    }

                    trigger(
                        mod.id,
                        BindingKeys.setPointerOverUI,
                        true
                    );

                    dragSessionRef.current = {
                        pointerX: event.clientX,
                        pointerY: event.clientY,
                        startX: positionRef.current.x,
                        startY: positionRef.current.y,
                        width: rect.width,
                        height: rect.height,
                    };

                    setDragging(true);
                }}
            />
        </div>
    );
}
