import type { ModRegistrar } from "cs2/modding";
import {
  AreaBulldozerModMenuButton,
  AreaBulldozerUI,
} from "./AreaBulldozerUI";

const register: ModRegistrar = (moduleRegistry) => {
  console.log("[AreaBulldozer] UI module registered.");
  moduleRegistry.append("GameTopLeft", AreaBulldozerUI);
  moduleRegistry.append("UniversalModMenu", AreaBulldozerModMenuButton);
};

export default register;
