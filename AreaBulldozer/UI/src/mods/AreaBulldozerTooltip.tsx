import React, { useEffect, useLayoutEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import styles from "./AreaBulldozerTooltip.module.scss";

// Clone the control instead of adding a layout wrapper. In particular, filter
// buttons must keep their original flex wrapping and mouse hit areas.
export function AreaBulldozerTooltip({ title, description, children }: {
    title: string; description?: string; children: React.ReactElement;
}) {
    const [anchor, setAnchor] = useState<{ x: number; y: number } | null>(null);
    const timer = useRef<ReturnType<typeof setTimeout> | null>(null);
    const popup = useRef<HTMLDivElement>(null);
    const clear = () => {
        if (timer.current !== null) clearTimeout(timer.current);
        timer.current = null;
    };
    const hide = () => { clear(); setAnchor(null); };
    const show = (x: number, y: number) => {
        clear();
        timer.current = setTimeout(() => { timer.current = null; setAnchor({ x, y }); }, 150);
    };
    useEffect(() => () => clear(), []);
    useEffect(() => {
        if (!anchor) return;
        window.addEventListener("blur", hide);
        window.addEventListener("resize", hide);
        document.addEventListener("scroll", hide, true);
        document.addEventListener("mousedown", hide, true);
        return () => {
            window.removeEventListener("blur", hide);
            window.removeEventListener("resize", hide);
            document.removeEventListener("scroll", hide, true);
            document.removeEventListener("mousedown", hide, true);
        };
    }, [anchor]);
    useLayoutEffect(() => {
        if (!anchor || !popup.current) return;
        const card = popup.current;
        const rect = card.getBoundingClientRect();
        const x = anchor.x + 14 + rect.width <= window.innerWidth - 8
            ? anchor.x + 14 : anchor.x - rect.width - 14;
        const y = anchor.y + 18 + rect.height <= window.innerHeight - 8
            ? anchor.y + 18 : anchor.y - rect.height - 14;
        card.style.left = `${Math.max(8, Math.min(x, window.innerWidth - rect.width - 8))}px`;
        card.style.top = `${Math.max(8, Math.min(y, window.innerHeight - rect.height - 8))}px`;
    }, [anchor, title, description]);
    return <>
        {React.cloneElement(children, {
            title: undefined,
            "aria-label": children.props["aria-label"] ?? title,
            onMouseEnter: (event: React.MouseEvent<HTMLElement>) => {
                children.props.onMouseEnter?.(event);
                show(event.clientX, event.clientY);
            },
            onMouseLeave: (event: React.MouseEvent<HTMLElement>) => {
                children.props.onMouseLeave?.(event); hide();
            },
            onMouseDown: (event: React.MouseEvent<HTMLElement>) => {
                children.props.onMouseDown?.(event); hide();
            },
            onFocus: (event: React.FocusEvent<HTMLElement>) => {
                children.props.onFocus?.(event);
                const rect = event.currentTarget.getBoundingClientRect();
                show(rect.right, rect.top);
            },
            onBlur: (event: React.FocusEvent<HTMLElement>) => {
                children.props.onBlur?.(event); hide();
            },
        })}
        {anchor && createPortal(<div ref={popup} className={styles.tooltip} role="tooltip"
            style={{ left: anchor.x + 14, top: anchor.y + 18 }}>
            <div className={styles.title}>{title}</div>
            {description && <div className={styles.description}>{description}</div>}
        </div>, document.body)}
    </>;
}
