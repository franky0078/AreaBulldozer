import React, { useState } from "react";
import { createRoot } from "react-dom/client";
import mod from "mod.json";
import {
  AreaBulldozerModMenuButton,
  AreaBulldozerUI,
} from "./AreaBulldozerUI";
import { setPreviewValue } from "./preview/cs2-api";
import "./preview/preview.scss";

setPreviewValue(mod.id, "isToolActive", true);
setPreviewValue(mod.id, "uiScale", 100);
setPreviewValue(mod.id, "useUniversalModMenu", false);
setPreviewValue(mod.id, "launcherButtonMovable", false);
setPreviewValue(mod.id, "launcherPositionX", 54);
setPreviewValue(mod.id, "launcherPositionY", 8);

function PreviewApp() {
  const [scale, setScale] = useState(100);
  const [useModMenu, setUseModMenu] = useState(false);
  const [movable, setMovable] = useState(false);

  const changeScale = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const nextScale = Number(event.currentTarget.value);
    setScale(nextScale);
    setPreviewValue(mod.id, "uiScale", nextScale);
  };

  const changeUseModMenu = (event: React.ChangeEvent<HTMLInputElement>) => {
    const enabled = event.currentTarget.checked;
    setUseModMenu(enabled);
    setPreviewValue(mod.id, "useUniversalModMenu", enabled);
  };

  const changeMovable = (event: React.ChangeEvent<HTMLInputElement>) => {
    const enabled = event.currentTarget.checked;
    setMovable(enabled);
    setPreviewValue(mod.id, "launcherButtonMovable", enabled);
  };

  const openPanel = () => {
    setPreviewValue(mod.id, "isToolActive", true);
  };

  return (
    <main className="previewApp">
      <div className="previewToolbar">
        <strong>Area Bulldozer – Browser-Vorschau</strong>
        <label>
          UI-Skalierung
          <select value={scale} onChange={changeScale}>
            <option value={75}>75 %</option>
            <option value={85}>85 %</option>
            <option value={90}>90 %</option>
            <option value={100}>100 %</option>
            <option value={110}>110 %</option>
            <option value={125}>125 %</option>
          </select>
        </label>
        <label>
          <input
            type="checkbox"
            checked={useModMenu}
            onChange={changeUseModMenu}
          />
          Neues Mod-Menü
        </label>
        <label>
          <input
            type="checkbox"
            checked={movable}
            onChange={changeMovable}
          />
          Button verschiebbar
        </label>
        <button type="button" onClick={openPanel}>
          Fenster öffnen
        </button>
        <span className="previewHint">
          Layout-Test ohne Spiel – C# und echte Spielobjekte werden simuliert.
        </span>
      </div>

      <div className="previewStage">
        <div className="previewMapGrid" aria-hidden="true" />
        <div className="previewTopInfo" aria-hidden="true">i</div>
        <div className="previewUiAnchor">
          <AreaBulldozerUI />
        </div>
        {useModMenu && (
          <aside className="previewUniversalModMenu">
            <strong>Universelles Mod-Menü</strong>
            <AreaBulldozerModMenuButton />
          </aside>
        )}
        <div className="previewBottomHud" aria-hidden="true">
          <div className="previewHudBars">
            <span />
            <span />
            <span />
          </div>
          <div className="previewHudCenter">MEGALOPOLIS</div>
        </div>
      </div>
    </main>
  );
}

const rootElement = document.getElementById("root");

if (!rootElement) {
  throw new Error("Preview root element was not found.");
}

createRoot(rootElement).render(<PreviewApp />);
