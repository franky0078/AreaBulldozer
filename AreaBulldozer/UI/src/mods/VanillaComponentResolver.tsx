import type { FocusKey, Theme, UniqueFocusKey } from "cs2/bindings";
import type { ModuleRegistry } from "cs2/modding";
import { HTMLAttributes, ReactNode } from "react";


type PropsToolButton = {
    focusKey?: UniqueFocusKey | null;
    src?: string;
    selected?: boolean;
    multiSelect?: boolean;
    disabled?: boolean;
    tooltip?: string | ReactNode | null;
    selectSound?: any;
    uiTag?: string;
    className?: string;
    children?: string | JSX.Element | JSX.Element[];
    onSelect?: (x: any) => any;
} & HTMLAttributes<any>;

type PropsSection = {
    title?: string | null;
    uiTag?: string;
    children: string | JSX.Element | JSX.Element[];
};

const registryIndex = {
    Section: [
        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
        "Section",
    ],
    ToolButton: [
        "game-ui/game/components/tool-options/tool-button/tool-button.tsx",
        "ToolButton",
    ],
    toolButtonTheme: [
        "game-ui/game/components/tool-options/tool-button/tool-button.module.scss",
        "classes",
    ],
    mouseToolOptionsTheme: [
        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.module.scss",
        "classes",
    ],
    descriptionTooltipTheme: [
        "game-ui/common/tooltip/description-tooltip/description-tooltip.module.scss",
        "classes",
    ],
    Tooltip: ["game-ui/common/tooltip/tooltip.tsx", "Tooltip"],
    FOCUS_DISABLED: ["game-ui/common/focus/focus-key.ts", "FOCUS_DISABLED"],
    FOCUS_AUTO: ["game-ui/common/focus/focus-key.ts", "FOCUS_AUTO"],
    useUniqueFocusKey: ["game-ui/common/focus/focus-key.ts", "useUniqueFocusKey"],
};

export class VanillaComponentResolver {
    public static get instance(): VanillaComponentResolver {
        return this._instance!!;
    }
    private static _instance?: VanillaComponentResolver;

    public static setRegistry(in_registry: ModuleRegistry) {
        this._instance = new VanillaComponentResolver(in_registry);
    }

    private registryData: ModuleRegistry;

    constructor(in_registry: ModuleRegistry) {
        this.registryData = in_registry;
    }

    private cachedData: Partial<Record<keyof typeof registryIndex, any>> = {};

    private updateCache(entry: keyof typeof registryIndex) {
        const entryData = registryIndex[entry];
        try {
            return (this.cachedData[entry] = this.registryData.registry.get(
                entryData[0]
            )!![entryData[1]]);
        } catch (error) {
            console.error(
                `[AreaBulldozer] Vanilla-Modul nicht gefunden: ${entryData[0]} -> ${entryData[1]}`,
                error
            );
            return undefined;
        }
    }

    public get Section(): (props: PropsSection) => JSX.Element {
        return this.cachedData["Section"] ?? this.updateCache("Section");
    }

    public get ToolButton(): (props: PropsToolButton) => JSX.Element {
        return this.cachedData["ToolButton"] ?? this.updateCache("ToolButton");
    }

    public get toolButtonTheme(): Theme | any {
        return (
            this.cachedData["toolButtonTheme"] ?? this.updateCache("toolButtonTheme")
        );
    }

    public get mouseToolOptionsTheme(): Theme | any {
        return (
            this.cachedData["mouseToolOptionsTheme"] ??
            this.updateCache("mouseToolOptionsTheme")
        );
    }

    public get descriptionTooltipTheme(): Theme | any {
        return (
            this.cachedData["descriptionTooltipTheme"] ??
            this.updateCache("descriptionTooltipTheme")
        );
    }

    public get Tooltip(): ((props: any) => JSX.Element) | undefined {
        return this.cachedData["Tooltip"] ?? this.updateCache("Tooltip");
    }

    public get FOCUS_DISABLED(): UniqueFocusKey {
        return this.cachedData["FOCUS_DISABLED"] ?? this.updateCache("FOCUS_DISABLED");
    }

    public get FOCUS_AUTO(): UniqueFocusKey {
        return this.cachedData["FOCUS_AUTO"] ?? this.updateCache("FOCUS_AUTO");
    }

    public get useUniqueFocusKey(): (
        focusKey: FocusKey,
        debugName: string
    ) => UniqueFocusKey | null {
        return (
            this.cachedData["useUniqueFocusKey"] ?? this.updateCache("useUniqueFocusKey")
        );
    }
}
