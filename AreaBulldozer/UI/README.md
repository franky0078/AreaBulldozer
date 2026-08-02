# Area Bulldozer UI

This folder contains the React/TypeScript interface for the Area Bulldozer
Cities: Skylines II mod.

## Requirements

- Cities: Skylines II Modding Toolchain
- Node.js 18 or newer
- The `CSII_USERDATAPATH` environment variable created by the toolchain

## First build

1. Open a terminal in this `UI` folder.
2. Run `npm install`.
3. Run `npm run build`.

The build is written directly to:

`%CSII_USERDATAPATH%\Mods\AreaBulldozer`

For development with automatic rebuilds, run:

`npm run dev`

You can also double-click `build-ui.cmd` for a normal one-time build.

## Important

The C# project and the UI project must both be built. The C# project provides
the bindings and tool logic; this UI project creates the `.mjs` and `.css`
files loaded by the game.
