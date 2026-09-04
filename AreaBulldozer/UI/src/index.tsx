import type { ModRegistrar } from "cs2/modding";

import { AreaBulldozerSections } from "./mods/AreaBulldozerSections";
import { BackToBulldozer } from "./mods/BackToBulldozer";

import {
    RoutedAreaBulldozerLauncher,
    RoutedAreaBulldozerModMenuButton,
    VanillaBulldozerLauncher,
} from "./mods/LauncherRouting";

import { ToolOptionsVisibility } from "./mods/ToolOptionsVisible";
import { VanillaComponentResolver } from "./mods/VanillaComponentResolver";

import * as bindings from "./bindings";
import { launcherMode$ } from "./bindings";


const UI_BUILD =
    "r7-free-area-polygon-direct";


const ENABLE = {
    LAUNCHER: true,
    MOD_MENU: true,
    TOOL_PANEL: true,
    BACK_TO_BULLDOZER: true,
    VANILLA_LAUNCHER: true,
    PANEL_VISIBLE: true,
};


function safely(
    label: string,
    action: () => void
) {
    try {
        action();

        console.log(
            `[AreaBulldozer] ${label}: OK`
        );
    }
    catch (error) {
        console.error(
            `[AreaBulldozer] ${label}: FEHLGESCHLAGEN`,
            error
        );
    }
}


function validateBindings() {
    const broken:
        string[] =
        [];


    for (
        const [
            name,
            value,
        ]
        of Object.entries(bindings)
    ) {
        if (
            !name.endsWith("$")
        ) {
            continue;
        }


        if (
            !value ||
            typeof
            (value as any)
                .subscribe
            !==
            "function"
        ) {
            broken.push(
                name
            );
        }
    }


    if (
        broken.length > 0
    ) {
        console.error(
            `[AreaBulldozer] Ungueltige Bindings: ${broken.join(", ")}.`
        );
    }
    else {
        console.log(
            "[AreaBulldozer] Bindings geprüft: alle in Ordnung."
        );
    }
}


const register:
    ModRegistrar =
    (moduleRegistry) => {

        console.log(
            `[AreaBulldozer] UI-Build: ${UI_BUILD}`
        );


        safely(
            "Bindings prüfen",
            validateBindings
        );


        safely(
            "VanillaComponentResolver",
            () => {
                VanillaComponentResolver
                    .setRegistry(
                        moduleRegistry
                    );
            }
        );


        if (
            ENABLE.LAUNCHER
        ) {
            safely(
                "Startknopf (GameTopLeft)",
                () => {
                    moduleRegistry.append(
                        "GameTopLeft",
                        RoutedAreaBulldozerLauncher
                    );
                }
            );
        }


        if (
            ENABLE.MOD_MENU
        ) {
            safely(
                "Universal Mod Menu",
                () => {
                    const registryAny =
                        moduleRegistry as any;

                    const originalHasAppend =
                        typeof registryAny.hasAppend === "function"
                            ? registryAny.hasAppend.bind(
                                moduleRegistry
                            )
                            : null;

                    if (originalHasAppend) {
                        registryAny.hasAppend =
                            (target: string) =>
                                target ===
                                    "UniversalModMenu"
                                    ? launcherMode$.value === 2 ||
                                    originalHasAppend(
                                        target
                                    )
                                    : originalHasAppend(
                                        target
                                    );
                    }

                    moduleRegistry.extend(
                        "game-ui/modding/modding-hook.tsx",
                        "ModdingHook",
                        (Prev: any) =>
                            (props: any) => {
                                if (
                                    props.name ===
                                    "UniversalModMenu"
                                ) {
                                    const {
                                        children,
                                        ...rest
                                    } = props;

                                    return (
                                        <Prev
                                            {...rest}
                                        >
                                            {children}

                                            <RoutedAreaBulldozerModMenuButton />
                                        </Prev>
                                    );
                                }

                                return (
                                    <Prev
                                        {...props}
                                    />
                                );
                            }
                    );
                }
            );
        }


        if (
            ENABLE.TOOL_PANEL
        ) {
            safely(
                "Area Bulldozer Tool Options",
                () => {
                    moduleRegistry.extend(
                        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
                        "MouseToolOptions",
                        AreaBulldozerSections
                    );
                }
            );
        }


        if (
            ENABLE.BACK_TO_BULLDOZER
        ) {
            safely(
                "Zurück zum normalen Bulldozer",
                () => {
                    moduleRegistry.extend(
                        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
                        "MouseToolOptions",
                        BackToBulldozer
                    );
                }
            );
        }


        if (
            ENABLE.VANILLA_LAUNCHER
        ) {
            safely(
                "Area Launcher im normalen Bulldozer",
                () => {
                    moduleRegistry.extend(
                        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
                        "MouseToolOptions",
                        VanillaBulldozerLauncher
                    );
                }
            );
        }


        if (
            ENABLE.PANEL_VISIBLE
        ) {
            safely(
                "Panel-Sichtbarkeit",
                () => {
                    moduleRegistry.extend(
                        "game-ui/game/components/tool-options/tool-options-panel.tsx",
                        "useToolOptionsVisible",
                        ToolOptionsVisibility
                    );
                }
            );
        }


        console.log(
            "[AreaBulldozer] UI-Registrierung abgeschlossen."
        );
    };


export default register;
