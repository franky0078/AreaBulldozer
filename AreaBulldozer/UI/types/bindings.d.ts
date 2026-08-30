declare module "cs2/bindings" {
    import type {
        ValueBinding,
    } from "cs2/api";

    export interface Tool {
        id: string;

        [key: string]:
        unknown;
    }


    export const tool: {
        activeTool$:
        ValueBinding<Tool>;

        BULLDOZE_TOOL:
        string;

        DEFAULT_TOOL:
        string;

        OBJECT_TOOL:
        string;

        AREA_TOOL:
        string;

        NET_TOOL:
        string;

        selectTool(
            toolID: string
        ): void;

        selectToolMode(
            modeIndex: number
        ): void;

        [key: string]:
        unknown;
    };

     // Focus-Typen für VanillaComponentResolver
    export class FocusSymbol {
        readonly debugName:
            string;

        constructor(
            debugName: string
        );

        toString():
            string;
    }


    export const FOCUS_DISABLED:
        FocusSymbol;

    export const FOCUS_AUTO:
        FocusSymbol;


    export type UniqueFocusKey =
        | FocusSymbol
        | string
        | number;


    export type FocusKey =
        | typeof FOCUS_DISABLED
        | typeof FOCUS_AUTO
        | UniqueFocusKey;



    // SCSS-/Theme-Objekt

    export type Theme =
        Record<
            string,
            string
        >;
}
