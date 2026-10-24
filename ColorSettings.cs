using System;
using System.IO;
using UnityEngine;

namespace RZColor
{
    // Todos os ajustes do RZColor. Cada slider vai de -100 a +100; 0 = neutro (jogo original).
    // [Serializable] permite que o Unity converta isso para JSON e de volta.
    // Pense nisso como uma "table" de Lua com campos fixos.
    [Serializable]
    public class ColorSettings
    {
        public int Version = 1;
        public bool Enabled = true;

        // --- Ajustes globais ---
        public float Saturation;   // saturação geral
        public float Contrast;     // contraste
        public float Blacks;       // pretos (lift)
        public float Gamma;        // gama (meios-tons)
        public float Whites;       // brancos (gain)
        public float Temperature;  // + quente / - frio
        public float Tint;         // + magenta / - verde

        // --- Cor seletiva: matiz (Hue) e saturação (Sat) por faixa ---
        public float Reds_Hue, Reds_Sat;
        public float Yellows_Hue, Yellows_Sat;
        public float Greens_Hue, Greens_Sat;
        public float Cyans_Hue, Cyans_Sat;
        public float Blues_Hue, Blues_Sat;
        public float Magentas_Hue, Magentas_Sat;

        // Lê o matiz da faixa i (0=vermelho ... 5=magenta). Facilita os loops no sistema.
        public float GetHue(int i)
        {
            switch (i)
            {
                case 0: return Reds_Hue;
                case 1: return Yellows_Hue;
                case 2: return Greens_Hue;
                case 3: return Cyans_Hue;
                case 4: return Blues_Hue;
                default: return Magentas_Hue;
            }
        }

        // Lê a saturação da faixa i.
        public float GetSat(int i)
        {
            switch (i)
            {
                case 0: return Reds_Sat;
                case 1: return Yellows_Sat;
                case 2: return Greens_Sat;
                case 3: return Cyans_Sat;
                case 4: return Blues_Sat;
                default: return Magentas_Sat;
            }
        }

        // ------------------------------------------------------------------
        // Salvar / carregar
        // O arquivo fica FORA dos saves, em:
        // %USERPROFILE%\AppData\LocalLow\Colossal Order\Cities Skylines II\ModsData\RZColor\RZColor.json
        // Por isso desinstalar o mod nunca afeta nenhuma cidade salva.
        // ------------------------------------------------------------------
        public static string FilePath =>
            Path.Combine(Application.persistentDataPath, "ModsData", "RZColor", "RZColor.json");

        public static ColorSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var data = JsonUtility.FromJson<ColorSettings>(File.ReadAllText(FilePath));
                    if (data != null)
                    {
                        data.ClampAll();
                        return data;
                    }
                }
            }
            catch (Exception e)
            {
                // Arquivo corrompido ou editado errado: avisa no log e usa o neutro.
                Mod.Log.Warn($"Could not read {FilePath}: {e.Message}. Using neutral values.");
            }

            var fresh = new ColorSettings();
            fresh.Save(); // cria o arquivo na primeira vez
            return fresh;
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, JsonUtility.ToJson(this, true)); // true = formatado, fácil de ler
            }
            catch (Exception e)
            {
                Mod.Log.Warn($"Could not save {FilePath}: {e.Message}");
            }
        }

        // Garante que nenhum valor passe de -100..100 (protege contra edições manuais exageradas).
        public void ClampAll()
        {
            Saturation = C(Saturation); Contrast = C(Contrast);
            Blacks = C(Blacks); Gamma = C(Gamma); Whites = C(Whites);
            Temperature = C(Temperature); Tint = C(Tint);
            Reds_Hue = C(Reds_Hue); Reds_Sat = C(Reds_Sat);
            Yellows_Hue = C(Yellows_Hue); Yellows_Sat = C(Yellows_Sat);
            Greens_Hue = C(Greens_Hue); Greens_Sat = C(Greens_Sat);
            Cyans_Hue = C(Cyans_Hue); Cyans_Sat = C(Cyans_Sat);
            Blues_Hue = C(Blues_Hue); Blues_Sat = C(Blues_Sat);
            Magentas_Hue = C(Magentas_Hue); Magentas_Sat = C(Magentas_Sat);
        }

        private static float C(float v) => Mathf.Clamp(v, -100f, 100f);
    }
}
