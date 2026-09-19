import React from "react";


let colorEditorOpen = false;

const listeners = new Set<() => void>();


function notifyListeners() {
    listeners.forEach(listener => listener());
}


export function toggleColorEditor() {
    colorEditorOpen = !colorEditorOpen;
    notifyListeners();
}


export function closeColorEditor() {
    if (!colorEditorOpen) {
        return;
    }

    colorEditorOpen = false;
    notifyListeners();
}


export function useColorEditorOpen() {
    const [open, setOpen] = React.useState(colorEditorOpen);

    React.useEffect(() => {
        const listener = () => setOpen(colorEditorOpen);

        listeners.add(listener);

        return () => {
            listeners.delete(listener);
        };
    }, []);

    return open;
}
