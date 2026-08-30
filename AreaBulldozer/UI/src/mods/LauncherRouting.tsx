import React from "react";
import { trigger, useValue } from "cs2/api";
import { tool } from "cs2/bindings";
import { useLocalization } from "cs2/l10n";
import type { ModuleRegistryExtend } from "cs2/modding";
import mod from "mod.json";

import {
    AreaBulldozerLauncher,
    AreaBulldozerModMenuButton,
} from "../AreaBulldozerUI";

import {
    BindingKeys,
    isToolActive$,
    launcherMode$,
} from "../bindings";

import { BulldozerIcon } from "./AreaBulldozerIcons";
import styles from "./VanillaBulldozerLauncher.module.scss";
import { useSafeValue } from "./useSafeValue";
import { VanillaComponentResolver } from "./VanillaComponentResolver";


const UI_PREFIX = "AreaBulldozer.UI.";

enum LauncherMode {
    Standalone = 0,
    VanillaBulldozer = 1,
    UniversalModMenu = 2,
}


export function RoutedAreaBulldozerLauncher() {
    const launcherMode = useSafeValue(
        "launcherMode$",
        launcherMode$,
        LauncherMode.Standalone
    );

    if (launcherMode !== LauncherMode.Standalone) {
        return null;
    }

    return <AreaBulldozerLauncher />;
}


export function RoutedAreaBulldozerModMenuButton() {
    const launcherMode = useSafeValue(
        "launcherMode$",
        launcherMode$,
        LauncherMode.Standalone
    );

    if (launcherMode !== LauncherMode.UniversalModMenu) {
        return null;
    }

    return <AreaBulldozerModMenuButton />;
}


 // Erweitert das normale Bulldozer-Toolfenster.

export const VanillaBulldozerLauncher: ModuleRegistryExtend = (Component: any) => {
    return (props: any) => {
        const launcherMode = useSafeValue(
            "launcherMode$",
            launcherMode$,
            LauncherMode.Standalone
        );

        const isAreaBulldozerActive = useSafeValue(
            "isToolActive$",
            isToolActive$,
            false
        );

        const activeTool = useValue(tool.activeTool$);

        const { translate } = useLocalization();

        let result: JSX.Element | null = null;

        try {
            result = Component(props);
        }
        catch (error) {
            console.error(
                "[AreaBulldozer] Vanilla MouseToolOptions konnte nicht gerendert werden.",
                error
            );

            return null;
        }

        if (
            launcherMode !== LauncherMode.VanillaBulldozer ||
            isAreaBulldozerActive ||
            activeTool?.id !== tool.BULLDOZE_TOOL
        ) {
            return result;
        }

        const resolver = VanillaComponentResolver.instance;
        const Section = resolver.Section;

        if (typeof Section !== "function") {
            console.error(
                "[AreaBulldozer] Vanilla Section ist für den Bulldozer-Launcher nicht verfügbar."
            );

            return result;
        }

        const Tooltip = resolver.Tooltip;
        const tooltipTheme = resolver.descriptionTooltipTheme;
        const toolButtonTheme = resolver.toolButtonTheme;

        const text = (key: string, fallback: string) =>
            translate(`${UI_PREFIX}${key}`, fallback) ?? fallback;

        const title =
            text("Title", "Area Bulldozer");

        const description =
            text(
                "VanillaLauncherTooltip",
                "Open Area Bulldozer for deleting multiple objects with a circular or square selection."
            );

        const button = (
            <button
                type="button"
                className={[
                    toolButtonTheme?.button ?? "",
                    styles.launchButton,
                ]
                    .filter(Boolean)
                    .join(" ")}
                onClick={(event) => {
                    event.preventDefault();
                    event.stopPropagation();

                    trigger(
                        mod.id,
                        BindingKeys.toggleTool
                    );
                }}
                title={Tooltip ? undefined : description}
                aria-label={title}
            >
                <BulldozerIcon
                    className={styles.bulldozerIcon}
                />
            </button>
        );

        const launcherButton = Tooltip
            ? (
                <Tooltip
                    tooltip={
                        <>
                            <div className={tooltipTheme?.title}>
                                {title}
                            </div>

                            <div className={tooltipTheme?.content}>
                                {description}
                            </div>
                        </>
                    }
                >
                    {button}
                </Tooltip>
            )
            : button;

        const launcherSection = (
            <Section title={title}>
                {launcherButton}
            </Section>
        );

        try {
            if (!result) {
                return (
                    <div
                        className={
                            resolver.mouseToolOptionsTheme?.mouseToolOptions
                        }
                    >
                        {launcherSection}
                    </div>
                );
            }

            return React.cloneElement(
                result,
                {},
                ...React.Children.toArray(
                    result.props?.children
                ),
                launcherSection
            );
        }
        catch (error) {
            console.error(
                "[AreaBulldozer] Launcher konnte nicht in die Vanilla-Bulldozer-Optionen eingehängt werden.",
                error
            );

            return result;
        }
    };
};
