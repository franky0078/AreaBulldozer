# Area Bulldozer

Area Bulldozer is an area deletion tool for **Cities: Skylines II**. It allows multiple objects to be selected and removed at once using an adjustable circular or square selection area.

![Area Bulldozer thumbnail](Properties/Thumbnail.png)

## Features

- Circular and square selection modes
- Adjustable selection size
- Adjustable rotation for the square selection
- Live preview of the selected area
- Individual filters for:
  - Vegetation
  - Buildings
  - Roads
  - Pedestrian paths
  - Railway tracks
  - Surfaces and areas
  - Props and markers
- Advanced prop and marker filters
- Optional visualization of activity locations, spawn locations and asset lanes
- Optional background darkening for improved marker visibility
- Safety settings for sub-objects, owned objects and large selections
- Compact and scalable in-game interface
- Optional integration into the universal mod menu
- Optional movable floating launcher button
- Configurable keyboard shortcut
- English and German localization

## Usage

1. Activate Area Bulldozer using the launcher button, the universal mod menu or the configured keyboard shortcut.
2. Select the circular or square selection mode.
3. Adjust the selection size.
4. When using the square selection, adjust its rotation in the tool interface or by holding the right mouse button.
5. Enable the object filters you want to use.
6. Move the selection over the area you want to clear.
7. Press the left mouse button to delete the selected objects.

Check the enabled filters carefully before deleting objects from your city.

### Movable launcher button

When the floating launcher is enabled and movement is allowed in the mod options:

1. Hold **Ctrl**.
2. Hold the **left mouse button** on the launcher.
3. Move the launcher to the desired position.
4. Release the left mouse button to save the position.

A normal left click only activates or deactivates the tool.

## Mod options

Permanent settings are available under the Area Bulldozer section in the game options.

### Display

- Tool-window scale
- Marker-background darkness
- Universal mod menu integration
- Movable floating launcher
- Reset launcher position

### Safety

- Include building sub-objects
- Include network sub-objects
- Protect objects assigned to another owner
- Confirm large selections
- Configure the large-selection threshold

### Key bindings

- Activate or deactivate Area Bulldozer
- Reset the configured key binding

## Browser UI preview

The interface can be previewed without starting the game.

Open a command prompt in the `UI` directory and run:

```cmd
npm install
npm run preview
```

The preview simulates the UI bindings and is intended for testing layout, scaling, colors, scrolling, sliders and filter buttons. Actual object selection and deletion can only be tested in the game.

## Building from source

### Requirements

- Cities: Skylines II
- Cities: Skylines II official modding toolchain
- .NET SDK required by the current game toolchain
- Node.js and npm
- Visual Studio or another compatible .NET development environment

### Build steps

1. Clone or download the repository.
2. Ensure the Cities: Skylines II modding environment variables are configured.
3. Open `AreaBulldozer.csproj`.
4. Build or rebuild the project.

The project automatically restores the UI dependencies and runs the UI build. The generated DLL, JavaScript module and stylesheet are copied to the Cities: Skylines II user mod directory.

Expected generated files include:

```text
AreaBulldozer.dll
AreaBulldozer.mjs
AreaBulldozer.css
```

## Compatibility

Area Bulldozer is an independent mod and can generally be used alongside other bulldozer or deletion-related mods. Using multiple deletion tools at the same time may cause overlapping controls or unexpected behavior. Only one deletion tool should be active at a time.

## Credits and inspiration

Area Bulldozer was inspired by the work of other Cities: Skylines II mod developers, especially:

- [Better Bulldozer](https://github.com/yenyang/BetterBulldozer) by yenyang
- [Radius Delete Mod](https://github.com/kurupted/Cities2_RadiusDeleteMod) by kurupted

Special thanks to both developers for their work and their contributions to the Cities: Skylines II modding community.

Area Bulldozer is an independent implementation and is not officially affiliated with these mods or their developers.

## Bug reports

When reporting a problem, include:

- A detailed description of what happened
- The selected object filters
- The selected safety settings
- Steps that reproduce the problem
- The relevant game or mod log output

## License

Copyright (c) 2026 franky0078

Area Bulldozer is licensed under the **MIT License**.

You may use, copy, modify, merge, publish, distribute, sublicense and/or sell copies of the software, provided that the copyright notice and permission notice are included in all copies or substantial portions of the software.

The software is provided **as is**, without warranty of any kind.

See the [`LICENSE`](LICENSE) file for the complete license text.
