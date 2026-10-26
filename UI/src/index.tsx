// Ponto de entrada do painel: registra os componentes no jogo.
import { ModRegistrar } from "cs2/modding";
import { RZColorButton } from "mods/RZColorButton";
import { RZColorPanel } from "mods/RZColorPanel";

const register: ModRegistrar = (moduleRegistry) => {
    // Botão no canto superior esquerdo
    moduleRegistry.append("GameTopLeft", RZColorButton);
    // A janela fica na camada principal do jogo (posição livre, arrastável)
    moduleRegistry.append("Game", RZColorPanel);
};

export default register;
