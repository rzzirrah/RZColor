# RZColor

Lightweight in-game color grading for **Cities: Skylines II**: blacks, whites, contrast, gamma, saturation, temperature, tint, and a selective color mixer (hue and saturation for cyans, magentas, yellows, reds, greens and blues).

**Download:** [RZColor on PDX Mods](https://mods.paradoxplaza.com/mods/164660/Windows)

## How it works

RZColor creates its own global HDRP volume, named `RZColor` (default priority 4900, adjustable in Options), and only uses three color grading components:

| Component | Controls |
|---|---|
| `LiftGammaGain` | Blacks, gamma, whites |
| `ColorCurves` | Saturation, contrast, color mixer (Hue vs Hue / Hue vs Sat curves) |
| `ChannelMixer` | Temperature, tint |

HDRP bakes all color grading into a single 3D LUT, so there's no extra full-screen pass. The LUT is normally rebuilt only when a grading setting changes, but HDRP skips that check while any `ColorCurves` parameter is overridden, so it rebuilds the LUT every frame whenever saturation, contrast or the color mixer is in use. That rebuild is a small compute pass over the LUT, not the screen, so the cost stays very low. Sliders left at 0 switch their curves off. HDRP grades in ACEScg when the ACES tonemapper is active (the game's default) and in linear Rec.709 with any other tonemapper (Neutral, None or an external LUT). RZColor checks the active tonemapper on the main camera's volume stack and moves the color mixer bands to match, so each band stays on its color in either space. Sliders left at 0 switch their parameter off.

Settings live in `%USERPROFILE%\AppData\LocalLow\Colossal Order\Cities Skylines II\ModsData\RZColor\RZColor.json` and are never written to save files. The file is reloaded automatically when it changes, so looks can be shared by copying it.

### Compatibility with Lumina

Classic Lumina uses `ColorAdjustments`, `WhiteBalance`, `ShadowsMidtonesHighlights` and `Tonemapping` on a volume with priority 5000. RZColor doesn't touch any of those, so both mods stack.

**Lumina NGX** can edit any component on any volume, including RZColor's (it lists volumes by name, so look for `RZColor`). When two volumes change the same component, the one with the higher priority wins. Use **Options → RZColor → Volume priority** to choose: below the volumes you edit in NGX to let them win, above to let RZColor win.

If NGX removes RZColor's components, they come back the next time you change an RZColor slider. RZColor assigns its profile through `Volume.profile`, so tools that read `volume.profile` edit the same profile instead of a copy.

## Project structure

```
Mod.cs                     Entry point, registers systems and the Options page
ColorSettings.cs           Settings model + JSON load/save
Localization.cs            UI and Options texts (en, pt-BR, fr, de, es, zh-HANS, ko)
Setting.cs                 Options page (reset, volume priority, open log/settings folders)
Systems/ColorVolumeSystem.cs  HDRP volume and color math
Systems/RZColorUISystem.cs    Bindings between C# and the React panel
UI/                        In-game panel (React + SCSS)
```

## Building

1. Install the Cities: Skylines II modding toolchain (in-game: Options → Modding).
2. Run `npm install` once inside the `UI` folder.
3. Open `RZColor.csproj` in Visual Studio 2022 and build. The mod is deployed to your local `Mods` folder.

## Credits

Inspired by [Lumina](https://github.com/NyokoDev/LuminaCS2) by Nyoko and Sun Glasses by Cmyna. UI designed in Figma by Rinaldo Zirrah. Code written with guidance from Claude.

## License

MIT
