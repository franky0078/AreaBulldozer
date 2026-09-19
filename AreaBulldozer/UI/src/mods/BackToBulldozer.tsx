import React from "react";

import { trigger } from "cs2/api";
import { tool } from "cs2/bindings";
import { useLocalization } from "cs2/l10n";
import type { ModuleRegistryExtend } from "cs2/modding";
import mod from "mod.json";

import {
    BindingKeys,
    includeOverriddenVegetation$,
    isToolActive$,
    launcherMode$,
} from "../bindings";

import styles from "./BackToBulldozer.module.scss";
import { AreaBulldozerIcon } from "./AreaBulldozerIcons";
import {
    toggleColorEditor,
    useColorEditorOpen,
} from "./AreaBulldozerColorEditorState";
import { useSafeValue } from "./useSafeValue";
import { VanillaComponentResolver } from "./VanillaComponentResolver";


const UI_PREFIX = "AreaBulldozer.UI.";

const VANILLA_BULLDOZER_LAUNCHER_MODE = 1;

const ANARCHY_ICON_STANDARD =
    "coui://uil/Standard/Anarchy.svg";

const ANARCHY_ICON_COLORED =
    "coui://uil/Colored/Anarchy.svg";


function BackArrowIcon() {
    return (
        <svg
            className={
                styles.backIcon
            }

            viewBox="0 0 32 32"

            fill="none"

            stroke="currentColor"

            strokeWidth="2.5"

            strokeLinecap="round"

            strokeLinejoin="round"

            aria-hidden="true"
        >
            <path
                d="M19 7 10 16l9 9"
            />

            <path
                d="M11 16h14"
            />
        </svg>
    );
}

export const BackToBulldozer:
    ModuleRegistryExtend =
    (Component: any) => {

        return (props: any) => {
            const isToolActive =
                useSafeValue(
                    "isToolActive$",
                    isToolActive$,
                    false
                );

            const launcherMode =
                useSafeValue(
                    "launcherMode$",
                    launcherMode$,
                    0
                );

            const includeOverriddenVegetation =
                useSafeValue(
                    "includeOverriddenVegetation$",
                    includeOverriddenVegetation$,
                    false
                );

            const colorEditorOpen = useColorEditorOpen();

            const [loadedAnarchyIconSource, setLoadedAnarchyIconSource] =
                React.useState<string | null>(null);

            const [failedAnarchyIconSource, setFailedAnarchyIconSource] =
                React.useState<string | null>(null);

            const { translate } =
                useLocalization();


            let result:
                React.ReactElement | null =
                null;


            try {
                result =
                    Component(props);
            }
            catch (error) {
                console.error(
                    "[AreaBulldozer] BackToBulldozer: MouseToolOptions konnte nicht gerendert werden.",
                    error
                );

                return null;
            }


            if (
                !isToolActive ||
                launcherMode !==
                VANILLA_BULLDOZER_LAUNCHER_MODE
            ) {
                return result;
            }


            const resolver =
                VanillaComponentResolver.instance;

            const Section =
                resolver.Section;

            const Tooltip =
                resolver.Tooltip;

            const tooltipTheme =
                resolver.descriptionTooltipTheme;

            const toolButtonTheme =
                resolver.toolButtonTheme;

            const text = (key: string, fallback: string) =>
                translate(`${UI_PREFIX}${key}`, fallback) ?? fallback;


            if (
                typeof Section !== "function"
            ) {
                return result;
            }


            const title =
                translate(
                    `${UI_PREFIX}BackToBulldozer`,
                    "Back to normal Bulldozer"
                ) ??
                "Back to normal Bulldozer";


            const button =
                (
                    <button
                        type="button"

                        className={[
                            toolButtonTheme?.button ?? "",
                            styles.backButton,
                        ]
                            .filter(Boolean)
                            .join(" ")}

                        onClick={
                            (event) => {
                                event.preventDefault();
                                event.stopPropagation();

                                trigger(
                                    mod.id,
                                    BindingKeys.deactivateTool
                                );

                                tool.selectTool(
                                    tool.BULLDOZE_TOOL
                                );
                            }
                        }

                        aria-label={
                            title
                        }

                        title={
                            Tooltip
                                ? undefined
                                : title
                        }
                    >
                        <BackArrowIcon />
                    </button>
                );


            const wrappedButton =
                Tooltip
                    ? (
                        <Tooltip
                            tooltip={
                                <>
                                    <div
                                        className={
                                            tooltipTheme?.title
                                        }
                                    >
                                        {title}
                                    </div>
                                </>
                            }
                        >
                            {button}
                        </Tooltip>
                    )
                    : button;


            const anarchyTitle =
                translate(
                    `${UI_PREFIX}AnarchyVegetation`,
                    "Anarchy"
                ) ??
                "Anarchy";

            const anarchyText =
                translate(
                    `${UI_PREFIX}AnarchyVegetationTooltip`,
                    "Together with the vegetation filter, allows overridden or Anarchy-placed trees to be removed."
                ) ??
                "Together with the vegetation filter, allows overridden or Anarchy-placed trees to be removed.";

            const anarchyIconSource =
                includeOverriddenVegetation
                    ? ANARCHY_ICON_COLORED
                    : ANARCHY_ICON_STANDARD;

            const anarchyButton =
                (
                    <button
                        type="button"
                        className={[
                            toolButtonTheme?.button ?? "",
                            styles.anarchyButton,
                            includeOverriddenVegetation
                                ? styles.anarchyButtonActive
                                : "",
                        ]
                            .filter(Boolean)
                            .join(" ")}
                        onClick={(event) => {
                            event.preventDefault();
                            event.stopPropagation();

                            trigger(
                                mod.id,
                                BindingKeys.setIncludeOverriddenVegetation,
                                !includeOverriddenVegetation
                            );
                        }}
                        aria-label={anarchyTitle}
                        aria-pressed={includeOverriddenVegetation}
                        title={Tooltip ? undefined : anarchyTitle}
                    >
                        <span
                            className={styles.anarchyIcon}
                            aria-hidden="true"
                        >
                            {loadedAnarchyIconSource !== anarchyIconSource && (
                                <AreaBulldozerIcon type="anarchy" />
                            )}

                            {failedAnarchyIconSource !== anarchyIconSource && (
                                <img
                                    key={anarchyIconSource}
                                    className={styles.anarchyLibraryIcon}
                                    src={anarchyIconSource}
                                    alt=""
                                    onLoad={() => {
                                        setLoadedAnarchyIconSource(
                                            anarchyIconSource
                                        );
                                    }}
                                    onError={() => {
                                        setFailedAnarchyIconSource(
                                            anarchyIconSource
                                        );
                                    }}
                                />
                            )}
                        </span>
                    </button>
                );

            const wrappedAnarchyButton =
                Tooltip
                    ? (
                        <Tooltip
                            tooltip={
                                <>
                                    <div className={tooltipTheme?.title}>
                                        {anarchyTitle}
                                    </div>
                                    <div className={tooltipTheme?.content}>
                                        {anarchyText}
                                    </div>
                                </>
                            }
                        >
                            {anarchyButton}
                        </Tooltip>
                    )
                    : anarchyButton;


            const colorTitle = colorEditorOpen
                ? text("CloseColorEditor", "Farbeditor schließen")
                : text("OpenColorEditor", "Farbeditor öffnen");

            const colorButton = (
                <button
                    type="button"
                    className={[
                        toolButtonTheme?.button ?? "",
                        styles.colorButton,
                        colorEditorOpen
                            ? styles.colorButtonActive
                            : "",
                    ]
                        .filter(Boolean)
                        .join(" ")}
                    onClick={(event) => {
                        event.preventDefault();
                        event.stopPropagation();
                        toggleColorEditor();
                    }}
                    aria-label={colorTitle}
                    aria-pressed={colorEditorOpen}
                    title={Tooltip ? undefined : colorTitle}
                >
                    <span className={styles.colorIcon} aria-hidden="true">
                        <AreaBulldozerIcon type="palette" />
                    </span>
                </button>
            );

            const wrappedColorButton = Tooltip
                ? (
                    <Tooltip
                        tooltip={
                            <>
                                <div className={tooltipTheme?.title}>
                                    {colorTitle}
                                </div>
                                <div className={tooltipTheme?.content}>
                                    {text(
                                        "ColorEditorTooltip",
                                        "Farben mit direkter Vorschau anpassen."
                                    )}
                                </div>
                            </>
                        }
                    >
                        {colorButton}
                    </Tooltip>
                )
                : colorButton;


            const backSection =
                (
                    <Section
                        title={
                            translate(
                                `${UI_PREFIX}Navigation`,
                                "Navigation"
                            ) ??
                            "Navigation"
                        }
                    >
                        <div className={styles.navigationButtons}>
                            <div className={styles.navigationColorButton}>
                                {wrappedColorButton}
                            </div>

                            <div className={styles.navigationRightButtons}>
                                {wrappedButton}
                                {wrappedAnarchyButton}
                            </div>
                        </div>
                    </Section>
                );

            try {
                if (!result) {
                    return (
                        <div
                            className={
                                resolver
                                    .mouseToolOptionsTheme
                                    ?.mouseToolOptions
                            }
                        >
                            {backSection}
                        </div>
                    );
                }


                return React.cloneElement(
                    result,
                    {},
                    backSection,
                    ...React.Children.toArray(
                        result.props?.children
                    )
                );
            }
            catch (error) {
                console.error(
                    "[AreaBulldozer] Zurück-Button konnte nicht in die Tool Options eingefügt werden.",
                    error
                );

                return result;
            }
        };
    };
