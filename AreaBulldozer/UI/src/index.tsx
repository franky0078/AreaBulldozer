import type { ModRegistrar } from "cs2/modding";
import {
    AreaBulldozerLauncher,
    AreaBulldozerModMenuButton,
} from "./AreaBulldozerUI";
import { AreaBulldozerSections } from "./mods/AreaBulldozerSections";
import { ToolOptionsVisibility } from "./mods/ToolOptionsVisible";
import { VanillaComponentResolver } from "./mods/VanillaComponentResolver";
import * as bindings from "./bindings";

const UI_BUILD = "r3-usesafevalue";


const ENABLE = {
    LAUNCHER: true,
    MOD_MENU: true,
    TOOL_PANEL: true,
    PANEL_VISIBLE: true,
};


function safely(label: string, action: () => void) {
    try {
        action();
        console.log(`[AreaBulldozer] ${label}: OK`);
    } catch (error) {
        console.error(`[AreaBulldozer] ${label}: FEHLGESCHLAGEN`, error);
    }
}


function validateBindings() {
    const broken: string[] = [];

    for (const [name, value] of Object.entries(bindings)) {
        if (!name.endsWith("$")) {
            continue;
        }

        if (!value || typeof (value as any).subscribe !== "function") {
            broken.push(name);
        }
    }

    if (broken.length > 0) {
        console.error(`[AreaBulldozer] Ungueltige Bindings: ${broken.join(", ")}.`);
    } else {
        console.log("[AreaBulldozer] Bindings geprüft: alle in Ordnung.");
    }
}

const register: ModRegistrar = (moduleRegistry) => {
    console.log(`[AreaBulldozer] UI-Build: ${UI_BUILD}`);
    console.log(
        `[AreaBulldozer] Schalter: Launcher=${ENABLE.LAUNCHER} ModMenu=${ENABLE.MOD_MENU} ` +
        `ToolPanel=${ENABLE.TOOL_PANEL} PanelVisible=${ENABLE.PANEL_VISIBLE}`
    );

    safely("Bindings prüfen", validateBindings);

    safely("VanillaComponentResolver", () => {
        VanillaComponentResolver.setRegistry(moduleRegistry);
    });

    if (ENABLE.LAUNCHER) {
        safely("Startknopf (GameTopLeft)", () => {
            moduleRegistry.append("GameTopLeft", AreaBulldozerLauncher);
        });
    }

    if (ENABLE.MOD_MENU) {
        safely("Mod-Menü-Eintrag", () => {
            moduleRegistry.append("UniversalModMenu", AreaBulldozerModMenuButton);
        });
    }

    if (ENABLE.TOOL_PANEL) {
        safely("Werkzeug-Optionen (MouseToolOptions)", () => {
            moduleRegistry.extend(
                "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
                "MouseToolOptions",
                AreaBulldozerSections
            );
        });
    }

    if (ENABLE.PANEL_VISIBLE) {
        safely("Panel-Sichtbarkeit (useToolOptionsVisible)", () => {
            moduleRegistry.extend(
                "game-ui/game/components/tool-options/tool-options-panel.tsx",
                "useToolOptionsVisible",
                ToolOptionsVisibility
            );
        });
    }

    console.log("[AreaBulldozer] UI-Registrierung abgeschlossen.");
};

export default register;
