using System.Diagnostics;
using System.IO;
using Game.Modding;
using Game.Settings;
using RZColor.Systems;
using UnityEngine;

namespace RZColor
{
    // Página do RZColor em Opções do jogo (aparece na lista da esquerda, como SKYVE etc.).
    // Só tem botões: os ajustes de cor continuam no painel e no RZColor.json.
    [SettingsUIGroupOrder(kToolsGroup, kSupportGroup)]
    [SettingsUIShowGroupName(kToolsGroup, kSupportGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kToolsGroup = "Tools";
        public const string kSupportGroup = "Support";

        public Setting(IMod mod) : base(mod) { }

        // Botão: zera todos os ajustes (pede confirmação antes).
        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kToolsGroup)]
        public bool ResetColors
        {
            set
            {
                ColorVolumeSystem.Instance?.ReplaceSettings(new ColorSettings());
                ColorVolumeSystem.Instance?.MarkDirty(saveToDisk: true);
                Mod.Log.Info("All color settings reset from the Options page.");
            }
        }

        // Botão: abre a pasta de logs no Explorer (para mandar o RZColor.log quando algo der errado).
        [SettingsUIButton]
        [SettingsUISection(kSection, kSupportGroup)]
        public bool OpenLogFolder
        {
            set { OpenFolder(Path.Combine(Application.persistentDataPath, "Logs")); }
        }

        // Botão: abre a pasta onde fica o RZColor.json.
        [SettingsUIButton]
        [SettingsUISection(kSection, kSupportGroup)]
        public bool OpenSettingsFolder
        {
            set { OpenFolder(Path.GetDirectoryName(ColorSettings.FilePath)); }
        }

        private static void OpenFolder(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                Process.Start(path); // no Windows, abre a pasta no Explorer
            }
            catch (System.Exception e)
            {
                Mod.Log.Warn($"Could not open folder {path}: {e.Message}");
            }
        }

        // Nada a restaurar: a página só tem botões.
        public override void SetDefaults() { }
    }
}
