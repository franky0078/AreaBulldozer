import React from "react";

export type AreaBulldozerIconType =
    | "heightAll"
    | "heightAbove"
    | "heightBelow"
    | "undergroundView"
    | "circle"
    | "square"
    | "triangle"
    | "polyline"
    | "polygon"
    | "palette"
    | "gear"
    | "straight"
    | "curve"
    | "missingAsset"
    | "undo"
    | "vegetation"
    | "protectVegetation"
    | "anarchy"
    | "building"
    | "car"
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
        case "heightAll":
        case "heightAbove":
        case "heightBelow":
        case "undergroundView":
            return (
                <svg {...common}>
                    <path d="M3 16h26" strokeDasharray="3 2" />
                    {(type === "heightAll" || type === "heightAbove") &&
                        <path d="M16 12V3 M11 8l5-5 5 5" />}
                    {(type === "heightAll" || type === "heightBelow") &&
                        <path d="M16 20v9 M11 24l5 5 5-5" />}
                    {type === "undergroundView" &&
                        <><path d="M5 24q11-12 22 0-11 12-22 0Z" /><circle cx="16" cy="24" r="3" /></>}
                </svg>
            );
        case "undo":
            return (
                <svg {...common}>
                    <path d="M11 5l-7 7 7 7 M4 12h15a7.5 7.5 0 0 1 0 15h-5" />
                </svg>
            );

        case "protectVegetation":
            return (
                <svg {...common}>
                    <path d="M16 3l11 4v8c0 6-4 11-11 14-7-3-11-8-11-14V7l11-4Z" />
                    <path d="M16 9l-5 9h10l-5-9Z M16 18v5" />
                </svg>
            );

        case "missingAsset":
            return (
                <svg {...common}>
                    <path d="M16 3 28 10v13l-12 7L4 23V10L16 3Z M4 10l12 7 12-7 M16 17v13" />
                    <path d="M16 7v4 M16 14h.01" />
                </svg>
            );

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

        case "gear":
            return (
                <svg {...common}>
                    <path d="M13 4h6l1 4 3 2 4-1 3 5-3 3v3l2 3-3 5-4-1-3 2h-6l-1-3-3-2-4 1-3-5 3-3v-3l-2-3 3-5 4 1 3-2Z" />
                    <circle cx="16" cy="16" r="4" />
                </svg>
            );

        case "palette":
            return (
                <svg {...common}>
                    <path d="M16 4C9 4 4 9 4 15.5S9.2 27 15.5 27h2.2c2 0 3.1-2.3 1.9-3.8-.8-1-.1-2.5 1.2-2.5H24c2.2 0 4-1.8 4-4C28 9.7 22.6 4 16 4Z" />
                    <circle cx="10" cy="13" r="1.6" />
                    <circle cx="14" cy="9" r="1.6" />
                    <circle cx="20" cy="9.5" r="1.6" />
                    <circle cx="23" cy="14" r="1.6" />
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

        case "anarchy":
            return (
                <svg {...common}>
                    <circle
                        data-anarchy-part="ring"
                        cx="16"
                        cy="16"
                        r="11.5"
                    />
                    <path
                        data-anarchy-part="letter"
                        d="M9 25 16 6.5 23 25M11.3 19h9.4"
                    />
                </svg>
            );

        case "building":
            return (
                <svg {...common}>
                    <path d="M6 27V8h13v19M19 13h7v14M10 12h4M10 17h4M10 22h4M22 17h1M22 22h1" />
                </svg>
            );

        case "car":
            return (
                <svg {...common}>
                    <path d="M5 21V14l4-6h14l4 6v7H5Z" />
                    <path d="M7 14h18M9 8l-2 6M23 8l2 6" />
                    <path d="M7 21v3h4v-3M21 21v3h4v-3" />
                    <circle cx="11" cy="18" r="1.4" />
                    <circle cx="21" cy="18" r="1.4" />
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
        <svg className={className} viewBox="0 0 96 88" fill="#fff" aria-hidden="true">
            {/* Cab, bonnet and grille */}
            <path fillRule="evenodd" d="
                M11 39 15 31 20 28 23 16 21 15Q19 14 20 11L39 5Q41 4 43 5L55 11
                Q57 12 56 16L55 17 58 32 64 34 72 39Q75 41 75 45V55L49 64
                Q48 59 42 55L22 42Q17 39 11 39Z
                M23 29 32 34 35 19 26 16Z M36 34 54 27 52 16 39 20Z
                M45 39 52 43 52 41 45 37Q43 36 43 38Z
                M44 45 52 50 52 48 44 43Q42 42 42 44Z
                M58 47 72 42V40L58 45Z M58 52 72 47V45L58 50Z
                M58 57 72 52V50L58 55Z
            " />
            {/* Exhaust and the far track */}
            <path d="M61 22 65 18Q67 17 67 19L65 22V32Q63 34 61 32ZM77 47 82 45 84 48 77 51Z" />
            {/* Near track with recessed tread and wheel cutouts */}
            <path fillRule="evenodd" d="
                M7 43Q13 39 21 44L43 59Q50 64 46 71Q43 77 36 74L12 61Q2 55 5 47Z
                M9 47Q6 53 14 58L36 70Q41 73 44 69Q46 65 40 61L18 47Q12 43 9 47Z
                M14 45 21 42 24 44 17 47Z M21 49 28 46 31 48 24 51Z
                M28 54 35 51 38 53 31 56Z M35 59 42 56 45 58 38 61Z
            " />
            <ellipse cx="14" cy="52" rx="2.8" ry="3.9" transform="rotate(-16 14 52)" />
            <ellipse cx="22" cy="57" rx="2.8" ry="3.9" transform="rotate(-16 22 57)" />
            <ellipse cx="30" cy="62" rx="2.8" ry="3.9" transform="rotate(-16 30 62)" />
            <ellipse cx="38" cy="67" rx="2.8" ry="3.9" transform="rotate(-16 38 67)" />
            {/* White blade, with a transparent crease between its faces */}
            <path fillRule="evenodd" d="
                M46 64 87 50Q89 49 90 51L91 63 95 70Q96 72 94 73L52 86
                Q50 87 48 85L43 82Q42 81 43 78Z
                M48 68 45 80 49 83 47 80Z
            " />
        </svg>
    );
}
