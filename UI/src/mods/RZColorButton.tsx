// Botão do RZColor no canto superior esquerdo do jogo. Abre e fecha a janela.
import { Button, Tooltip } from "cs2/ui";
import engine from "cohtml/cohtml";
import { useWindowOpen, setWindowOpen } from "../bindings";
import brushIcon from "../images/brush.svg";

export const RZColorButton = () => {
    const open = useWindowOpen();
    return (
        // Tooltip: ao passar o mouse aparece o nome do mod
        <Tooltip tooltip="RZColor">
            <Button
                variant="floating"
                src={brushIcon}
                selected={open}
                onSelect={() => {
                    engine.trigger("audio.playSound", "select-item", 1);
                    setWindowOpen(!open);
                }}
            />
        </Tooltip>
    );
};
