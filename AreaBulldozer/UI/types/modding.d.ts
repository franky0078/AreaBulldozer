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

  export interface ModuleRegistry {
    append(
      target: AppendHookTargets,
      component: ComponentType<Record<string, never>> | (() => JSX.Element),
      index?: number
    ): void;
  }

  export type ModRegistrar = (moduleRegistry: ModuleRegistry) => void;
}
