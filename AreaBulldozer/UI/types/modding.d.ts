declare module "cs2/modding" {
    import type { ComponentType } from "react";

    export type AppendHookTargets =
        | "Menu"
        | "Editor"
        | "Game"
        | "GameTopLeft"
        | "GameTopRight"
        | "GameBottomRight"
        | "UniversalModMenu";

    /**
     * Erweiterung eines vorhandenen Spiel-UI-Exports.
     *
     * Bewusst lose typisiert (any -> any): `extend` wird sowohl auf
     * React-Komponenten (MouseToolOptions) als auch auf Hooks
     * (useToolOptionsVisible, liefert ein boolean) angewendet. Eine engere
     * Typisierung auf ComponentType wuerde den Hook-Fall faelschlich als
     * Fehler melden.
     */
    export type ModuleRegistryExtend = (Component: any) => any;

    export interface ModuleRegistry {
        append(
            target: AppendHookTargets,
            component: ComponentType<Record<string, never>> | (() => JSX.Element),
            index?: number
        ): void;

        /**
         * Haengt sich in einen benannten Export eines Spiel-UI-Moduls ein.
         *
         * @param modulePath z. B. "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx"
         * @param exportName z. B. "MouseToolOptions"
         */
        extend(
            modulePath: string,
            exportName: string,
            extension: ModuleRegistryExtend
        ): void;

        /**
         * Rohzugriff auf die Modulregistrierung des Spiels. Wird vom
         * VanillaComponentResolver benutzt, um Section, ToolButton und die
         * zugehoerigen SCSS-Themes zu holen.
         */
        registry: Map<string, Record<string, any>>;
    }

    export type ModRegistrar = (moduleRegistry: ModuleRegistry) => void;

    /** Direkter Zugriff auf einen einzelnen Export eines Spiel-UI-Moduls. */
    export function getModule(modulePath: string, exportName: string): any;
}
