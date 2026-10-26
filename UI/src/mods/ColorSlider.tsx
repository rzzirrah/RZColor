// Slider do RZColor (-100 a +100, zero no meio), no visual do design do Figma:
// trilha clara arredondada + cursor redondo cinza escuro + valor à direita.
// Duplo clique no slider = volta para 0.
import { useRef } from "react";
import engine from "cohtml/cohtml";
import styles from "./rzcolor.module.scss";

type Props = {
    label: string;
    value: number;
    onChange: (v: number) => void;
    labelOnTop?: boolean; // Color Mixer: rótulo em cima da trilha
};

const MIN = -100;
const MAX = 100;

export const ColorSlider = ({ label, value, onChange, labelOnTop }: Props) => {
    // "rail" = área interna da trilha onde o centro do cursor pode andar
    // (fica recuada meio cursor de cada lado, para ele não sair da trilha).
    const railRef = useRef<HTMLDivElement | null>(null);

    const valueFromMouse = (clientX: number) => {
        const el = railRef.current;
        if (!el) return value;
        const rect = el.getBoundingClientRect();
        const t = Math.min(Math.max((clientX - rect.left) / rect.width, 0), 1);
        return Math.round(MIN + t * (MAX - MIN));
    };

    const onMouseDown = (e: React.MouseEvent) => {
        if (e.button !== 0) return;
        e.preventDefault();
        e.stopPropagation();
        onChange(valueFromMouse(e.clientX));

        const move = (ev: MouseEvent) => {
            onChange(valueFromMouse(ev.clientX));
            engine.trigger("audio.playSound", "drag-slider", 1);
        };
        const up = () => {
            document.removeEventListener("mousemove", move);
            document.removeEventListener("mouseup", up);
        };
        document.addEventListener("mousemove", move);
        document.addEventListener("mouseup", up);
    };

    const pct = ((value - MIN) / (MAX - MIN)) * 100;

    const slider = (
        <div className={styles.sliderLine}>
            <div className={styles.track} onMouseDown={onMouseDown} onDoubleClick={() => onChange(0)}>
                <div ref={railRef} className={styles.rail}>
                    <div className={styles.thumb} style={{ left: pct + "%" }} />
                </div>
            </div>
            <div className={styles.value}>{Math.round(value)}</div>
        </div>
    );

    if (labelOnTop) {
        return (
            <div className={styles.rowStacked}>
                <div className={styles.labelTop}>{label}</div>
                {slider}
            </div>
        );
    }

    return (
        <div className={styles.row}>
            <div className={styles.label}>{label}</div>
            {slider}
        </div>
    );
};
