import React from "react";

type ToolWindow = "colors" | "options";

let activeWindow: ToolWindow | null = null;
const listeners = new Set<() => void>();

export function closeToolWindow() {
    if (activeWindow !== null) {
        activeWindow = null;
        listeners.forEach(listener => listener());
    }
}

export function toggleToolWindow(window: ToolWindow) {
    if (activeWindow === window) {
        closeToolWindow();
        return;
    }

    activeWindow = window;
    listeners.forEach(listener => listener());
}

export function useToolWindow() {
    const [window, setWindow] = React.useState(activeWindow);

    React.useEffect(() => {
        const listener = () => setWindow(activeWindow);
        listeners.add(listener);
        listener();

        return () => {
            listeners.delete(listener);
        };
    }, []);

    return window;
}
