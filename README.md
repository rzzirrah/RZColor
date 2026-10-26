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

HDRP bakes all color grading into a single LUT, which is only rebuilt when a grading setting changes, so there's no extra post-processing pass. The color mixer bands are centered for ACEScg, the space HDRP grades in when the ACES tonemapper is active. With other tonemappers (Neutral, None or an external LUT), HDRP grades in linear Rec.709 and the bands may land slightly off their colors. Sliders left at 0 switch their parameter off.

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
