import { ModuleRegistryExtend } from "cs2/modding";
import { isToolActive$ } from "../bindings";
import { useSafeValue } from "./useSafeValue";


export const ToolOptionsVisibility: ModuleRegistryExtend = (Component: any) => {
    return (...args: any[]) => {

        const isToolActive = useSafeValue("isToolActive$", isToolActive$, false);

        let original: any = false;

        try {
            original = Component(...args);
        } catch (error) {
            console.error(
                "[AreaBulldozer] useToolOptionsVisible (Original) hat geworfen.",
                error
            );
            return isToolActive;
        }

        return original || isToolActive;
    };
};
