import React, {
    useCallback,
    useEffect,
    useRef,
    useState,
} from "react";

import { trigger } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import { Button } from "cs2/ui";
import mod from "mod.json";

import styles from "./AreaBulldozerUI.module.scss";

import { BulldozerIcon } from "./mods/AreaBulldozerIcons";
import { useSafeValue } from "./mods/useSafeValue";

import {
    BindingKeys,
    isToolActive$,
    launcherButtonMovable$,
    launcherPositionX$,
    launcherPositionY$,
    useUniversalModMenu$,
} from "./bindings";


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

const LAUNCHER_EDGE_MARGIN = 0;
const LAUNCHER_BUTTON_SIZE = 56;
const LOWER_GAME_UI_RESERVE = 170;
const TOP_ROW_SNAP_Y = 0;
const TOP_ROW_SNAP_DISTANCE = 18;


function clamp(
    value: number,
    min: number,
    max: number
) {
    return Math.min(
        max,
        Math.max(min, value)
    );
}


function clampLauncherPosition(
    x: number,
    y: number
): LauncherPosition {
    const viewportWidth =
        typeof window !== "undefined"
            ? window.innerWidth
            : 1920;

    const viewportHeight =
        typeof window !== "undefined"
            ? window.innerHeight
            : 1080;

    const maxX = Math.max(
        LAUNCHER_EDGE_MARGIN,
        viewportWidth -
        LAUNCHER_BUTTON_SIZE -
        LAUNCHER_EDGE_MARGIN
    );

    const maxY = Math.max(
        LAUNCHER_EDGE_MARGIN,
        viewportHeight -
        LOWER_GAME_UI_RESERVE -
        LAUNCHER_BUTTON_SIZE
    );

    const clampedX =
        Math.round(
            clamp(
                x,
                LAUNCHER_EDGE_MARGIN,
                maxX
            )
        );

    let clampedY =
        Math.round(
            clamp(
                y,
                LAUNCHER_EDGE_MARGIN,
                maxY
            )
        );

    if (
        Math.abs(
            clampedY -
            TOP_ROW_SNAP_Y
        ) <=
        TOP_ROW_SNAP_DISTANCE
    ) {
        clampedY =
            TOP_ROW_SNAP_Y;
    }

    return {
        x: clampedX,
        y: clampedY,
    };
}


/* =========================================================================
 * Separater Startbutton
 * =========================================================================
*/

export function AreaBulldozerLauncher() {
    const { translate } = useLocalization();

    const [
        launcherDragging,
        setLauncherDragging,
    ] = useState(false);


    const text = useCallback(
        (
            key: string,
            fallback: string
        ) =>
            translate(
                `${UI_PREFIX}${key}`,
                fallback
            ) ?? fallback,

        [translate]
    );


    const isToolActive =
        useSafeValue(
            "isToolActive$",
            isToolActive$,
            false
        );


    const useUniversalModMenu =
        useSafeValue(
            "useUniversalModMenu$",
            useUniversalModMenu$,
            false
        );


    const launcherButtonMovable =
        useSafeValue(
            "launcherButtonMovable$",
            launcherButtonMovable$,
            false
        );


    const savedLauncherPositionX =
        useSafeValue(
            "launcherPositionX$",
            launcherPositionX$,
            54
        );


    const savedLauncherPositionY =
        useSafeValue(
            "launcherPositionY$",
            launcherPositionY$,
            8
        );


    const initialLauncherPosition =
        clampLauncherPosition(
            savedLauncherPositionX,
            savedLauncherPositionY
        );


    const [
        launcherPosition,
        setLauncherPosition,
    ] =
        useState<LauncherPosition>(
            initialLauncherPosition
        );


    const launcherPositionRef =
        useRef<LauncherPosition>(
            initialLauncherPosition
        );


    const launcherDragSessionRef =
        useRef<LauncherDragSession | null>(
            null
        );


    const suppressLauncherClickUntilRef =
        useRef(0);


    useEffect(
        () => {
            if (launcherDragging) {
                return;
            }

            const nextPosition =
                clampLauncherPosition(
                    savedLauncherPositionX,
                    savedLauncherPositionY
                );

            launcherPositionRef.current =
                nextPosition;

            setLauncherPosition(
                nextPosition
            );
        },

        [
            launcherDragging,
            savedLauncherPositionX,
            savedLauncherPositionY,
        ]
    );


    const setPointerLock =
        useCallback(
            (value: boolean) => {
                trigger(
                    mod.id,
                    BindingKeys.setPointerOverUI,
                    value
                );
            },

            []
        );


    const finishLauncherDrag =
        useCallback(
            () => {
                if (
                    !launcherDragSessionRef.current
                ) {
                    return;
                }

                launcherDragSessionRef.current =
                    null;

                setLauncherDragging(false);

                setPointerLock(false);


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


                const now =
                    typeof performance !== "undefined"
                        ? performance.now()
                        : Date.now();

                suppressLauncherClickUntilRef.current =
                    now + 300;
            },

            [setPointerLock]
        );


    const updateLauncherDrag =
        useCallback(
            (
                clientX: number,
                clientY: number,
                buttons: number,
                ctrlKey: boolean
            ) => {
                const session =
                    launcherDragSessionRef.current;

                if (!session) {
                    return;
                }


                if (
                    (buttons & 1) === 0 ||
                    !ctrlKey
                ) {
                    finishLauncherDrag();
                    return;
                }


                const nextPosition =
                    clampLauncherPosition(
                        session.startPosition.x +
                        clientX -
                        session.startMouseX,

                        session.startPosition.y +
                        clientY -
                        session.startMouseY
                    );


                launcherPositionRef.current =
                    nextPosition;

                setLauncherPosition(
                    nextPosition
                );
            },

            [finishLauncherDrag]
        );


    const beginLauncherDrag =
        useCallback(
            (
                event:
                    React.MouseEvent<HTMLButtonElement>
            ) => {
                if (
                    !launcherButtonMovable ||
                    event.button !== 0 ||
                    !event.ctrlKey
                ) {
                    return;
                }


                event.preventDefault();
                event.stopPropagation();


                launcherDragSessionRef.current =
                {
                    startMouseX:
                        event.clientX,

                    startMouseY:
                        event.clientY,

                    startPosition:
                        launcherPositionRef.current,
                };


                setLauncherDragging(true);

                setPointerLock(true);
            },

            [
                launcherButtonMovable,
                setPointerLock,
            ]
        );


    const handleLauncherDragMove =
        useCallback(
            (
                event:
                    React.MouseEvent<HTMLDivElement>
            ) => {
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


    const handleLauncherDragMouseUp =
        useCallback(
            (
                event:
                    React.MouseEvent<HTMLDivElement>
            ) => {
                event.preventDefault();
                event.stopPropagation();

                if (event.button === 0) {
                    finishLauncherDrag();
                }
            },

            [finishLauncherDrag]
        );


    useEffect(
        () => {
            if (!launcherDragging) {
                return;
            }


            const onMouseUp =
                (event: MouseEvent) => {
                    if (event.button === 0) {
                        finishLauncherDrag();
                    }
                };


            const onKeyUp =
                (event: KeyboardEvent) => {
                    if (
                        event.key === "Control" ||
                        event.code === "ControlLeft" ||
                        event.code === "ControlRight"
                    ) {
                        finishLauncherDrag();
                    }
                };


            const onBlur =
                () => finishLauncherDrag();


            const onVisibilityChange =
                () => {
                    if (document.hidden) {
                        finishLauncherDrag();
                    }
                };


            document.addEventListener(
                "mouseup",
                onMouseUp,
                true
            );

            document.addEventListener(
                "keyup",
                onKeyUp,
                true
            );

            document.addEventListener(
                "visibilitychange",
                onVisibilityChange,
                true
            );

            window.addEventListener(
                "mouseup",
                onMouseUp,
                true
            );

            window.addEventListener(
                "blur",
                onBlur,
                true
            );


            return () => {
                document.removeEventListener(
                    "mouseup",
                    onMouseUp,
                    true
                );

                document.removeEventListener(
                    "keyup",
                    onKeyUp,
                    true
                );

                document.removeEventListener(
                    "visibilitychange",
                    onVisibilityChange,
                    true
                );

                window.removeEventListener(
                    "mouseup",
                    onMouseUp,
                    true
                );

                window.removeEventListener(
                    "blur",
                    onBlur,
                    true
                );
            };
        },

        [
            finishLauncherDrag,
            launcherDragging,
        ]
    );


    const stopEvent =
        useCallback(
            (
                event:
                    React.SyntheticEvent
            ) => {
                event.stopPropagation();
            },

            []
        );


    const toggleTool =
        useCallback(
            () => {
                const now =
                    typeof performance !== "undefined"
                        ? performance.now()
                        : Date.now();


                if (
                    now <
                    suppressLauncherClickUntilRef.current
                ) {
                    return;
                }


                trigger(
                    mod.id,
                    BindingKeys.toggleTool
                );
            },

            []
        );


    const launcherStyle =
        launcherButtonMovable
            ? ({
                position: "fixed",
                left:
                    `${launcherPosition.x}px`,
                top:
                    `${launcherPosition.y}px`,
                margin: 0,
            } as React.CSSProperties)

            : undefined;


    const launcherTitle =
        launcherButtonMovable
            ? text(
                "LauncherDragHint",
                "Strg + Linksklick halten und ziehen; loslassen zum Speichern"
            )
            : text(
                "ToggleTool",
                "Area Bulldozer umschalten"
            );


    if (
        useUniversalModMenu &&
        !launcherDragging
    ) {
        return null;
    }


    return (
        <div
            className={styles.host}

            onMouseEnter={
                () => setPointerLock(true)
            }

            onMouseLeave={
                () => setPointerLock(false)
            }

            onMouseDown={stopEvent}
            onMouseUp={stopEvent}
            onClick={stopEvent}
        >
            {!useUniversalModMenu && (
                <Button
                    variant="floating"

                    selected={
                        isToolActive
                    }

                    className={[
                        styles.floatingLauncher,

                        launcherButtonMovable
                            ? styles.toolButtonMovable
                            : "",

                        launcherDragging
                            ? styles.toolButtonDragging
                            : "",
                    ]
                        .filter(Boolean)
                        .join(" ")}

                    style={
                        launcherStyle
                    }

                    onMouseDown={
                        beginLauncherDrag
                    }

                    onSelect={
                        toggleTool
                    }

                    tooltipLabel={
                        launcherTitle
                    }

                    aria-label={
                        text(
                            "Title",
                            "Area Bulldozer"
                        )
                    }
                >
                    <BulldozerIcon
                        className={
                            styles.launcherBulldozerIcon
                        }
                    />
                </Button>
            )}


            {launcherDragging && (
                <div
                    className={
                        styles.launcherDragOverlay
                    }

                    onMouseMove={
                        handleLauncherDragMove
                    }

                    onMouseUp={
                        handleLauncherDragMouseUp
                    }

                    onMouseLeave={
                        finishLauncherDrag
                    }

                    onContextMenu={
                        stopEvent
                    }

                    aria-hidden="true"
                />
            )}
        </div>
    );
}


/* =========================================================================
 * Universal Mod Menu
 * =========================================================================
*/

export function AreaBulldozerModMenuButton() {
    const { translate } =
        useLocalization();

    const useUniversalModMenu =
        useSafeValue(
            "useUniversalModMenu$",
            useUniversalModMenu$,
            false
        );

    const isToolActive =
        useSafeValue(
            "isToolActive$",
            isToolActive$,
            false
        );

    const text =
        useCallback(
            (
                key: string,
                fallback: string
            ) =>
                translate(
                    `${UI_PREFIX}${key}`,
                    fallback
                ) ?? fallback,

            [translate]
        );

    const toggleTool =
        useCallback(
            () => {
                trigger(
                    mod.id,
                    BindingKeys.toggleTool
                );
            },

            []
        );

    if (!useUniversalModMenu) {
        return null;
    }
    return (
        <Button
            variant="floating"

            selected={
                isToolActive
            }

            className={
                styles.modMenuAction
            }

            onSelect={
                toggleTool
            }

            tooltipLabel={
                isToolActive
                    ? text(
                        "Close",
                        "Close tool"
                    )
                    : text(
                        "ModMenuOpen",
                        "Open tool"
                    )
            }

            aria-label={
                text(
                    "Title",
                    "Area Bulldozer"
                )
            }
        >
            <BulldozerIcon
                className={
                    styles.modMenuBulldozerIcon
                }
            />
        </Button>
    );
}
