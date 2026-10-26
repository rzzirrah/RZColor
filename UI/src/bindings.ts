// Ligações com o C# (RZColorUISystem.cs).
// O nome do grupo vem do mod.json ("RZColor") e precisa ser igual ao do C#.
import { bindValue, trigger } from "cs2/api";
import { useEffect, useState } from "react";
import mod from "mod.json";

// Todos os ajustes, como texto JSON (o C# manda sempre que algo muda).
export const state$ = bindValue<string>(mod.id, "state", "{}");

// Funções que chamam o C#
export const setValue = (key: string, value: number) => trigger(mod.id, "setValue", key, value);
export const setEnabled = (on: boolean) => trigger(mod.id, "setEnabled", on);
export const resetAll = () => trigger(mod.id, "resetAll");

// Formato dos ajustes (mesmos nomes dos campos em ColorSettings.cs)
export type ColorState = { Enabled: boolean } & Record<string, number>;

// ------------------------------------------------------------------
// "Janela aberta ou fechada?" — guardado só aqui no painel (não vai para o C#).
// Um mini "quadro de avisos": o botão e o painel escutam a mesma variável.
// ------------------------------------------------------------------
let windowOpen = false;
const listeners = new Set<() => void>();

export function setWindowOpen(open: boolean) {
    windowOpen = open;
    listeners.forEach((l) => l());
}

export function useWindowOpen(): boolean {
    const [, forceRender] = useState(0);
    useEffect(() => {
        const l = () => forceRender((n) => n + 1);
        listeners.add(l);
        return () => { listeners.delete(l); };
    }, []);
    return windowOpen;
}
