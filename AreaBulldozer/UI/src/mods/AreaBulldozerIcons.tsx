import React from "react";

export type AreaBulldozerIconType =
    | "circle"
    | "square"
    | "triangle"
    | "polyline"
    | "polygon"
    | "straight"
    | "curve"
    | "vegetation"
    | "building"
    | "road"
    | "path"
    | "rail"
    | "surface"
    | "props"
    | "generalProps"
    | "streetLight"
    | "quantity"
    | "branding"
    | "activity"
    | "spawn"
    | "lanes"
    | "dim"
    | "minus"
    | "plus";

const common = {
    viewBox: "0 0 32 32",
    fill: "none",
    stroke: "currentColor",
    strokeWidth: 2,
    strokeLinecap: "round" as const,
    strokeLinejoin: "round" as const,
    "aria-hidden": true,
};

export function AreaBulldozerIcon({ type }: { type: AreaBulldozerIconType }) {
    switch (type) {
        case "circle":
            return (
                <svg {...common}>
                    <circle cx="16" cy="16" r="11" />
                </svg>
            );

        case "square":
            return (
                <svg {...common}>
                    <rect x="6" y="6" width="20" height="20" rx="2" />
                </svg>
            );

        case "triangle":
            return (
                <svg {...common}>
                    <path d="M16 4 28 26H4L16 4Z" />
                </svg>
            );

        case "polyline":
            return (
                <svg {...common}>
                    <path d="M5 25 11 18 17 21 22 10 27 7" />
                    <circle cx="5" cy="25" r="1.8" />
                    <circle cx="11" cy="18" r="1.8" />
                    <circle cx="17" cy="21" r="1.8" />
                    <circle cx="22" cy="10" r="1.8" />
                    <circle cx="27" cy="7" r="1.8" />
                </svg>
            );

        case "polygon":
            return (
                <svg {...common}>
                    <path d="M5 22 8 8 18 5 27 12 24 26 12 28 5 22Z" />
                    <circle cx="5" cy="22" r="1.8" />
                    <circle cx="8" cy="8" r="1.8" />
                    <circle cx="18" cy="5" r="1.8" />
                    <circle cx="27" cy="12" r="1.8" />
                    <circle cx="24" cy="26" r="1.8" />
                    <circle cx="12" cy="28" r="1.8" />
                </svg>
            );

        case "straight":
            return (
                <svg {...common}>
                    <path d="M5 24 14 12 27 20" />
                    <circle cx="5" cy="24" r="1.8" />
                    <circle cx="14" cy="12" r="1.8" />
                    <circle cx="27" cy="20" r="1.8" />
                </svg>
            );

        case "curve":
            return (
                <svg {...common}>
                    <path d="M5 24C10 24 10 11 16 11S22 20 27 20" />
                    <circle cx="5" cy="24" r="1.8" />
                    <circle cx="16" cy="11" r="1.8" />
                    <circle cx="27" cy="20" r="1.8" />
                </svg>
            );

        case "vegetation":
            return (
                <svg {...common}>
                    <path d="M16 4 9 14h4l-6 8h7v6h4v-6h7l-6-8h4L16 4Z" />
                </svg>
            );

        case "building":
            return (
                <svg {...common}>
                    <path d="M6 27V8h13v19M19 13h7v14M10 12h4M10 17h4M10 22h4M22 17h1M22 22h1" />
                </svg>
            );

        case "road":
            return (
                <svg {...common}>
                    <path d="M9 28 12.5 4M23 28 19.5 4" />
                    <path d="M16 5v4M16 12v4M16 19v4M16 26v2" />
                </svg>
            );

        case "path":
            return (
                <svg {...common}>
                    <path d="M7 27c2-8 6-7 8-13 2-5-1-7 3-10M14 27c1-5 4-6 6-10 2-4 1-7 5-10" />
                    <circle cx="8" cy="8" r="2" />
                </svg>
            );

        case "rail":
            return (
                <svg {...common}>
                    <path d="M10 4v24M22 4v24M9 8h14M8 14h16M8 20h16M9 26h14" />
                </svg>
            );

        case "surface":
            return (
                <svg {...common}>
                    <path d="m5 11 10-6 12 7-10 6L5 11Z" />
                    <path d="m5 17 12 7 10-6M5 22l12 7 10-6" />
                </svg>
            );

        case "props":
            return (
                <svg {...common}>
                    <path d="M6 24h20M9 24V12h14v12M12 12V8h8v4" />
                    <circle cx="12" cy="18" r="2" />
                    <circle cx="20" cy="18" r="2" />
                </svg>
            );

        case "generalProps":
            return (
                <svg {...common}>
                    <path d="M4 25h24" />
                    <path d="M7 25v-7h9v7" />
                    <path d="M7 21h9" />
                    <path d="M22 25v-9" />
                    <path d="M22 16c-3 0-5-2-5-4h10c0 2-2 4-5 4Z" />
                </svg>
            );

        case "streetLight":
            return (
                <svg {...common}>
                    <path d="M12 28h8" />
                    <path d="M16 28V10" />
                    <path d="M16 10c0-4 3-6 6-6" />
                    <path d="M18 8h8l-2 5h-4l-2-5Z" />
                </svg>
            );

        case "quantity":
            return (
                <svg {...common}>
                    <path d="M8 10h16l-1.5 17h-13L8 10Z" />
                    <path d="M6 10h20" />
                    <path d="M13 6h6v4h-6V6Z" />
                    <path d="M14 15v7M18 15v7" />
                </svg>
            );

        case "branding":
            return (
                <svg {...common}>
                    <rect x="5" y="6" width="22" height="13" rx="2" />
                    <path d="M16 19v9" />
                    <path d="M11 28h10" />
                    <path d="M10 11h8M10 15h5" />
                </svg>
            );

        case "activity":
            return (
                <svg {...common}>
                    <circle cx="16" cy="9" r="3" />
                    <path d="M16 12v9" />
                    <path d="m10 16 6-2 6 2" />
                    <path d="m12 28 4-7 4 7" />
                </svg>
            );

        case "spawn":
            return (
                <svg {...common}>
                    <circle cx="16" cy="16" r="3" />
                    <path d="M16 4v5M16 23v5M4 16h5M23 16h5" />
                    <path d="M8 8l3 3M24 8l-3 3M8 24l3-3M24 24l-3-3" />
                </svg>
            );

        case "lanes":
            return (
                <svg {...common}>
                    <path d="M4 24h4M12 24h4M20 24h4" />
                    <path d="M4 15h4M12 15h4M20 15h4" />
                    <path d="M6 8h20" />
                    <circle cx="27" cy="24" r="2" />
                </svg>
            );

        case "dim":
            return (
                <svg {...common}>
                    <circle cx="16" cy="16" r="10" />
                    <path d="M16 6a10 10 0 0 1 0 20Z" fill="currentColor" />
                </svg>
            );

        case "minus":
            return (
                <svg {...common}>
                    <path d="M8 16h16" />
                </svg>
            );

        case "plus":
            return (
                <svg {...common}>
                    <path d="M8 16h16M16 8v16" />
                </svg>
            );

        default:
            return (
                <svg {...common}>
                    <circle cx="16" cy="16" r="10" />
                </svg>
            );
    }
}

export function BulldozerIcon({ className }: { className?: string }) {
    return (
        <svg
            className={className}
            viewBox="0 0 64 48"
            fill="none"
            stroke="currentColor"
            strokeWidth="2.2"
            strokeLinecap="round"
            strokeLinejoin="round"
            aria-hidden="true"
        >
            <path d="M4 31L10 24H28L36 27L33 40H9L4 31Z" />
            <path d="M6 33H34" />
            <path d="M23 22L30 16L38 20" />
            <path d="M30 16V24" />
            <path d="M38 8L52 7L58 10V24H40L38 8Z" />
            <path d="M41 11L50 10.5L50 21H41V11Z" />
            <path d="M36 24H56L60 27V33" />
            <circle cx="51" cy="34" r="8" />
            <circle cx="61" cy="36" r="5" />
        </svg>
    );
}
