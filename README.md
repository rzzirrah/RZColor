# RZColor

Lightweight in-game color grading for **Cities: Skylines II**: blacks, whites, contrast, gamma, saturation, temperature, tint, and a selective color mixer (hue and saturation for cyans, magentas, yellows, reds, greens and blues).

**Download:** [RZColor on PDX Mods](https://mods.paradoxplaza.com/mods/164660/Windows)

## How it works

RZColor creates its own global HDRP volume (priority 4900) and only uses three color grading components:

| Component | Controls |
|---|---|
| `LiftGammaGain` | Blacks, gamma, whites |
| `ColorCurves` | Saturation, contrast, color mixer (Hue vs Hue / Hue vs Sat curves) |
| `ChannelMixer` | Temperature, tint |

HDRP bakes all color grading into a LUT every frame, so there's no extra post-processing pass. The color mixer bands are centered on where each color sits in HDRP's ACEScg grading space. Sliders left at 0 switch their parameter off.

Settings live in `%USERPROFILE%\AppData\LocalLow\Colossal Order\Cities Skylines II\ModsData\RZColor\RZColor.json` and are never written to save files. The file is reloaded automatically when it changes, so looks can be shared by copying it.

### Compatibility with Lumina

Lumina uses `ColorAdjustments`, `WhiteBalance`, `ShadowsMidtonesHighlights` and `Tonemapping`. RZColor doesn't touch any of those, so both mods stack.

## Project structure

```
Mod.cs                     Entry point, registers systems and the Options page
ColorSettings.cs           Settings model + JSON load/save
Localization.cs            UI and Options texts (en, pt-BR, fr, de, es, zh-HANS, ko)
Setting.cs                 Options page (reset, open log/settings folders)
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
