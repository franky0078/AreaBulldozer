import React from "react";

import { tool } from "cs2/bindings";
import { useLocalization } from "cs2/l10n";
import type { ModuleRegistryExtend } from "cs2/modding";

import {
    isToolActive$,
    launcherMode$,
} from "../bindings";

import styles from "./BackToBulldozer.module.scss";
import { useSafeValue } from "./useSafeValue";
import { VanillaComponentResolver } from "./VanillaComponentResolver";


const UI_PREFIX = "AreaBulldozer.UI.";

const VANILLA_BULLDOZER_LAUNCHER_MODE = 1;


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
                        {wrappedButton}
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
