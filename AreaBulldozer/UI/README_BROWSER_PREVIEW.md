# Area Bulldozer browser preview

The preview simulates the CS2 bindings so the UI can be inspected and clicked
without starting Cities: Skylines II.

## Easy start

Run from the UI folder:

```cmd
preview-ui.cmd
```

The script installs the additional preview dependency when needed and then
opens:

```text
http://127.0.0.1:8080
```

## Manual start

```cmd
npm install
npm run preview
```

Stop the preview with `Ctrl+C` in the command window.

The browser preview is intended for layout, sizing, scrolling, colors, buttons
and sliders. It cannot test real C# bindings, game object selection, deletion or
world-space mouse controls.
