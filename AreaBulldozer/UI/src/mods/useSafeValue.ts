import { useEffect, useState } from "react";
import type { ValueBinding } from "cs2/api";

export function useSafeValue<T>(
    name: string,
    binding: ValueBinding<T> | undefined,
    fallback: T
): T {
    const [value, setValue] = useState<T>(() => {
        try {
            const current = binding?.value;
            return current === undefined ? fallback : current;
        } catch {
            return fallback;
        }
    });

    useEffect(() => {
        if (!binding || typeof binding.subscribe !== "function") {
            console.error(
                `[AreaBulldozer] Binding "${name}" ist ungültig und wird übersprungen.`,
                binding
            );
            return;
        }

        let subscription: { readonly value: T; dispose(): void } | undefined;

        try {
            subscription = binding.subscribe((next) => setValue(next));

            const initial =
                subscription?.value !== undefined ? subscription.value : binding.value;

            if (initial !== undefined) {
                setValue(initial);
            }
        } catch (error) {
            console.error(
                `[AreaBulldozer] Binding "${name}": subscribe() ist fehlgeschlagen.`,
                error
            );
            return;
        }

        return () => {
            try {
                subscription?.dispose();
            } catch (error) {
                console.error(
                    `[AreaBulldozer] Binding "${name}": dispose() ist fehlgeschlagen.`,
                    error
                );
            }
        };
    }, [binding, name]);

    return value;
}
