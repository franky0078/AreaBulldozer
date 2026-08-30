declare module "cs2/ui" {
    import type {
        ButtonHTMLAttributes,
        ReactElement,
        ReactNode,
    } from "react";


    export interface ButtonProps
        extends
        ButtonHTMLAttributes<
            HTMLButtonElement
        > {

        variant?:
        | "flat"
        | "primary"
        | "round"
        | "menu"
        | "icon"
        | "floating"
        | "text"
        | "default";

        selected?:
        boolean;

        onSelect?:
        () => void;

        focusKey?:
        unknown;

        tooltipLabel?:
        ReactNode;
    }


    export const Button:
        (
            props:
                ButtonProps
        ) =>
            JSX.Element;


    export interface TooltipProps {
        tooltip:
        ReactNode;

        children:
        ReactElement;

        disabled?:
        boolean;

        delayTime?:
        number;

        hideOnInteraction?:
        boolean;
    }


    export const Tooltip:
        (
            props:
                TooltipProps
        ) =>
            JSX.Element;


    export const FOCUS_DISABLED:
        unknown;

    export const FOCUS_AUTO:
        unknown;
}
