using System.Diagnostics;
using System.IO;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using RZColor.Systems;
using UnityEngine;

namespace RZColor
{
    // Página do RZColor em Opções do jogo (aparece na lista da esquerda, como SKYVE etc.).
    // Os ajustes de cor continuam no painel e no RZColor.json.
    // Aqui ficam só as ferramentas e a prioridade do volume (salva em ModsSettings\RZColor).
    [FileLocation("ModsSettings/RZColor/RZColor")]
    [SettingsUIGroupOrder(kToolsGroup, kCompatGroup, kSupportGroup)]
    [SettingsUIShowGroupName(kToolsGroup, kCompatGroup, kSupportGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kToolsGroup = "Tools";
        public const string kSupportGroup = "Support";
        public const string kCompatGroup = "Compatibility";

        // Prioridade padrão do volume. O Lumina usa 5000, então por padrão ele vence
        // quando os dois mexem no mesmo efeito.
        public const int DefaultPriority = 4900;

        private int m_VolumePriority = DefaultPriority;

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

        // Slider: prioridade do volume "RZColor". Muda na hora e fica salvo.
        // Volume de prioridade maior vence quando dois mods mexem no mesmo efeito.
        [SettingsUISlider(min = 0, max = 10000, step = 100, scalarMultiplier = 1, unit = "integer")]
        [SettingsUISection(kSection, kCompatGroup)]
        public int VolumePriority
        {
            get => m_VolumePriority;
            set
            {
                m_VolumePriority = value;
                ColorVolumeSystem.Instance?.SetPriority(value);
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

        // Valores padrão das opções salvas.
        public override void SetDefaults()
        {
            m_VolumePriority = DefaultPriority;
        }
    }
}
