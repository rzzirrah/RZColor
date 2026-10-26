// Janela principal do RZColor — segue o design do Figma (frame "Painel", node 6255:2).
// Ordem: título + Reset all → Contrast Parameters → White Balance → Color Mixer.
import { useMemo, useState } from "react";
import { useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import engine from "cohtml/cohtml";
import {
    state$, setValue, setEnabled, resetAll,
    ColorState, useWindowOpen, setWindowOpen,
} from "../bindings";
import { ColorSlider } from "./ColorSlider";
import styles from "./rzcolor.module.scss";

// As 6 cores do Color Mixer, na ordem do design (C, M, Y, R, G, B).
// "key" = início do nome do campo no C# (Cyans_Hue, Cyans_Sat...). "color" = cor da bolinha (Figma).
const BANDS = [
    { key: "Cyans", label: "Cyans", color: "#00f6ff" },
    { key: "Magentas", label: "Magentas", color: "#ff0077" },
    { key: "Yellows", label: "Yellows", color: "#ffd000" },
    { key: "Reds", label: "Reds", color: "#ff0000" },
    { key: "Greens", label: "Greens", color: "#00ff08" },
    { key: "Blues", label: "Blues", color: "#001eff" },
];

export const RZColorPanel = () => {
    const open = useWindowOpen();
    // Textos no idioma do jogo (traduções em Localization.cs). Se faltar, usa o inglês.
    const { translate } = useLocalization();
    const T = (key: string, english: string) => translate("RZColor.Panel." + key, english) ?? english;
    const json = useValue(state$);

    // Texto JSON vindo do C# -> objeto. useMemo = só refaz quando o texto muda.
    const s = useMemo<ColorState>(() => {
        try { return JSON.parse(json); } catch { return { Enabled: true } as ColorState; }
    }, [json]);

    const [band, setBand] = useState(1);                  // cor selecionada (começa no magenta, como no design)
    const [pos, setPos] = useState({ x: 120, y: 90 });    // posição da janela na tela

    if (!open) return null;

    const v = (key: string) => (typeof s[key] === "number" ? s[key] : 0);
    const set = (key: string) => (val: number) => setValue(key, val);
    const click = () => engine.trigger("audio.playSound", "select-item", 1);
    const stop = (e: React.MouseEvent) => e.stopPropagation(); // botões não iniciam o arrasto
    const b = BANDS[band];

    // Arrastar a janela segurando o título.
    // Mesmo método dos sliders: "segura" o mouse para o painel, senão o jogo
    // pega o clique para mexer a câmera e o arrasto não acontece.
    const startDrag = (e: React.MouseEvent) => {
        if (e.button !== 0) return;
        e.preventDefault();
        e.stopPropagation();
        const offX = e.clientX - pos.x;
        const offY = e.clientY - pos.y;

        const move = (ev: MouseEvent) => {
            // não deixa a janela sair da tela (sobra sempre um pedaço de 60px visível)
            const maxX = window.innerWidth - 60;
            const maxY = window.innerHeight - 60;
            setPos({
                x: Math.min(Math.max(ev.clientX - offX, -280), maxX),
                y: Math.min(Math.max(ev.clientY - offY, 0), maxY),
            });
        };
        const up = () => {
            document.removeEventListener("mousemove", move);
            document.removeEventListener("mouseup", up);
        };
        document.addEventListener("mousemove", move);
        document.addEventListener("mouseup", up);
    };

    return (
        <div className={styles.window} style={{ left: pos.x + "px", top: pos.y + "px" }}>
            {/* Fechar (canto superior direito) */}
            <div className={styles.close} onMouseDown={stop} onClick={() => { click(); setWindowOpen(false); }}>
                <div className={styles.closeGlyph} style={{ maskImage: "url(Media/Glyphs/Close.svg)" }} />
            </div>

            {/* Título + controles. Segure aqui para arrastar a janela. */}
            <div
                className={styles.header}
                onMouseDown={startDrag}
            >
                <div className={styles.title}>
                    <span className={styles.titleBold}>RZ</span>
                    <span className={styles.titleLight}>Color</span>
                </div>
                <div className={styles.headerButtons}>
                    {/* Liga/desliga (comparar antes/depois) */}
                    <div
                        className={s.Enabled ? styles.pill : styles.pill + " " + styles.pillOff}
                        onMouseDown={stop}
                        onClick={() => { click(); setEnabled(!s.Enabled); }}
                    >
                        {s.Enabled ? T("On", "On") : T("Off", "Off")}
                    </div>
                    <div className={styles.pill} onMouseDown={stop} onClick={() => { click(); resetAll(); }}>
                        {T("ResetAll", "Reset all settings")}
                    </div>
                </div>
            </div>

            <div className={s.Enabled ? styles.body : styles.body + " " + styles.dimmed}>
                <div className={styles.sectionTitle}>{T("ContrastParameters", "Contrast Parameters")}</div>
                <ColorSlider label={T("Blacks", "Blacks")} value={v("Blacks")} onChange={set("Blacks")} />
                <ColorSlider label={T("Whites", "Whites")} value={v("Whites")} onChange={set("Whites")} />
                <ColorSlider label={T("Contrast", "Contrast")} value={v("Contrast")} onChange={set("Contrast")} />
                <ColorSlider label={T("Gamma", "Gamma")} value={v("Gamma")} onChange={set("Gamma")} />

                <div className={styles.sectionTitle}>{T("WhiteBalance", "White Balance")}</div>
                <ColorSlider label={T("Saturation", "Saturation")} value={v("Saturation")} onChange={set("Saturation")} />
                <ColorSlider label={T("Temperature", "Temp")} value={v("Temperature")} onChange={set("Temperature")} />
                <ColorSlider label={T("Tint", "Tint")} value={v("Tint")} onChange={set("Tint")} />

                <div className={styles.sectionTitle}>{T("ColorMixer", "Color Mixer")}</div>
                <div className={styles.mixer}>
                    <div className={styles.swatches}>
                        {BANDS.map((bd, i) => (
                            <div
                                key={bd.key}
                                className={i === band ? styles.swatch + " " + styles.swatchSelected : styles.swatch}
                                style={{ backgroundColor: bd.color }}
                                onClick={() => { click(); setBand(i); }}
                            />
                        ))}
                    </div>
                    <ColorSlider labelOnTop label={T("Hue", "Hue")} value={v(b.key + "_Hue")} onChange={set(b.key + "_Hue")} />
                    <ColorSlider labelOnTop label={T("Saturation", "Saturation")} value={v(b.key + "_Sat")} onChange={set(b.key + "_Sat")} />
                </div>
            </div>
        </div>
    );
};
