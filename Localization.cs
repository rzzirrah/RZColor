using System.Collections.Generic;
using Colossal;
using Game.SceneFlow;

namespace RZColor
{
    // Todas as traduções do RZColor num lugar só.
    // Idiomas: os oficiais do jogo que escolhemos. Se o jogo estiver em outro idioma,
    // ele mostra o inglês (en-US).
    //
    // Cada idioma é uma "tabela" (como uma table em Lua): chave -> texto.
    // As chaves "Panel.*" são do painel (React); as "Options.*" são da página em Opções.
    public static class RZLocalization
    {
        // Prefixo das chaves do painel. O React pede, por exemplo, "RZColor.Panel.Blacks".
        public const string PanelPrefix = "RZColor.Panel.";

        private static readonly Dictionary<string, Dictionary<string, string>> Texts =
            new Dictionary<string, Dictionary<string, string>>
        {
            ["en-US"] = new Dictionary<string, string>
            {
                ["Panel.ContrastParameters"] = "Contrast Parameters",
                ["Panel.Blacks"] = "Blacks",
                ["Panel.Whites"] = "Whites",
                ["Panel.Contrast"] = "Contrast",
                ["Panel.Gamma"] = "Gamma",
                ["Panel.WhiteBalance"] = "White Balance",
                ["Panel.Saturation"] = "Saturation",
                ["Panel.Temperature"] = "Temp",
                ["Panel.Tint"] = "Tint",
                ["Panel.ColorMixer"] = "Color Mixer",
                ["Panel.Hue"] = "Hue",
                ["Panel.ResetAll"] = "Reset all settings",
                ["Panel.On"] = "On",
                ["Panel.Off"] = "Off",

                ["Options.Tab"] = "Main",
                ["Options.Tools"] = "Tools",
                ["Options.Support"] = "Support",
                ["Options.ResetColors"] = "Reset all color settings",
                ["Options.ResetColorsDesc"] = "Sets every RZColor slider back to 0 (the game's original look).",
                ["Options.ResetColorsWarning"] = "Reset all RZColor settings?",
                ["Options.Compatibility"] = "Compatibility",
                ["Options.VolumePriority"] = "Volume priority",
                ["Options.VolumePriorityDesc"] = "Priority of the global HDRP volume named \"RZColor\". When RZColor and another mod (such as Lumina or Lumina NGX) change the same effect, the volume with the higher priority wins. Lumina uses 5000. Default: 4900.",
                ["Options.OpenLogFolder"] = "Open log folder",
                ["Options.OpenLogFolderDesc"] = "Opens the folder with RZColor.log. Attach this file when reporting a problem.",
                ["Options.OpenSettingsFolder"] = "Open settings folder",
                ["Options.OpenSettingsFolderDesc"] = "Opens the folder with RZColor.json, where your color settings are stored. Share this file to share your look. Your saves are never changed.",
            },

            ["pt-BR"] = new Dictionary<string, string>
            {
                ["Panel.ContrastParameters"] = "Parâmetros de contraste",
                ["Panel.Blacks"] = "Pretos",
                ["Panel.Whites"] = "Brancos",
                ["Panel.Contrast"] = "Contraste",
                ["Panel.Gamma"] = "Gama",
                ["Panel.WhiteBalance"] = "Balanço de branco",
                ["Panel.Saturation"] = "Saturação",
                ["Panel.Temperature"] = "Temp.",
                ["Panel.Tint"] = "Tonalidade",
                ["Panel.ColorMixer"] = "Mixer de cores",
                ["Panel.Hue"] = "Matiz",
                ["Panel.ResetAll"] = "Redefinir tudo",
                ["Panel.On"] = "Ligado",
                ["Panel.Off"] = "Desligado",

                ["Options.Tab"] = "Principal",
                ["Options.Tools"] = "Ferramentas",
                ["Options.Support"] = "Suporte",
                ["Options.ResetColors"] = "Redefinir todos os ajustes de cor",
                ["Options.ResetColorsDesc"] = "Volta todos os sliders do RZColor para 0 (o visual original do jogo).",
                ["Options.ResetColorsWarning"] = "Redefinir todos os ajustes do RZColor?",
                ["Options.Compatibility"] = "Compatibilidade",
                ["Options.VolumePriority"] = "Prioridade do volume",
                ["Options.VolumePriorityDesc"] = "Prioridade do volume global do HDRP chamado \"RZColor\". Quando o RZColor e outro mod (como o Lumina ou o Lumina NGX) alteram o mesmo efeito, vence o volume de prioridade maior. O Lumina usa 5000. Padrão: 4900.",
                ["Options.OpenLogFolder"] = "Abrir pasta de logs",
                ["Options.OpenLogFolderDesc"] = "Abre a pasta do RZColor.log. Envie este arquivo ao relatar um problema.",
                ["Options.OpenSettingsFolder"] = "Abrir pasta de ajustes",
                ["Options.OpenSettingsFolderDesc"] = "Abre a pasta do RZColor.json, onde ficam seus ajustes de cor. Compartilhe este arquivo para compartilhar seu visual. Seus saves nunca são alterados.",
            },

            ["fr-FR"] = new Dictionary<string, string>
            {
                ["Panel.ContrastParameters"] = "Paramètres de contraste",
                ["Panel.Blacks"] = "Noirs",
                ["Panel.Whites"] = "Blancs",
                ["Panel.Contrast"] = "Contraste",
                ["Panel.Gamma"] = "Gamma",
                ["Panel.WhiteBalance"] = "Balance des blancs",
                ["Panel.Saturation"] = "Saturation",
                ["Panel.Temperature"] = "Temp.",
                ["Panel.Tint"] = "Nuance",
                ["Panel.ColorMixer"] = "Mélangeur de couleurs",
                ["Panel.Hue"] = "Teinte",
                ["Panel.ResetAll"] = "Tout réinitialiser",
                ["Panel.On"] = "Activé",
                ["Panel.Off"] = "Désactivé",

                ["Options.Tab"] = "Général",
                ["Options.Tools"] = "Outils",
                ["Options.Support"] = "Assistance",
                ["Options.ResetColors"] = "Réinitialiser tous les réglages de couleur",
                ["Options.ResetColorsDesc"] = "Remet tous les curseurs de RZColor à 0 (l'apparence d'origine du jeu).",
                ["Options.ResetColorsWarning"] = "Réinitialiser tous les réglages de RZColor ?",
                ["Options.Compatibility"] = "Compatibilité",
                ["Options.VolumePriority"] = "Priorité du volume",
                ["Options.VolumePriorityDesc"] = "Priorité du volume HDRP global nommé « RZColor ». Lorsque RZColor et un autre mod (comme Lumina ou Lumina NGX) modifient le même effet, le volume ayant la priorité la plus élevée l'emporte. Lumina utilise 5000. Par défaut : 4900.",
                ["Options.OpenLogFolder"] = "Ouvrir le dossier des journaux",
                ["Options.OpenLogFolderDesc"] = "Ouvre le dossier contenant RZColor.log. Joignez ce fichier pour signaler un problème.",
                ["Options.OpenSettingsFolder"] = "Ouvrir le dossier des réglages",
                ["Options.OpenSettingsFolderDesc"] = "Ouvre le dossier contenant RZColor.json, où sont enregistrés vos réglages de couleur. Partagez ce fichier pour partager votre rendu. Vos sauvegardes ne sont jamais modifiées.",
            },

            ["de-DE"] = new Dictionary<string, string>
            {
                ["Panel.ContrastParameters"] = "Kontrastparameter",
                ["Panel.Blacks"] = "Schwarz",
                ["Panel.Whites"] = "Weiß",
                ["Panel.Contrast"] = "Kontrast",
                ["Panel.Gamma"] = "Gamma",
                ["Panel.WhiteBalance"] = "Weißabgleich",
                ["Panel.Saturation"] = "Sättigung",
                ["Panel.Temperature"] = "Temp.",
                ["Panel.Tint"] = "Tönung",
                ["Panel.ColorMixer"] = "Farbmischer",
                ["Panel.Hue"] = "Farbton",
                ["Panel.ResetAll"] = "Alles zurücksetzen",
                ["Panel.On"] = "An",
                ["Panel.Off"] = "Aus",

                ["Options.Tab"] = "Allgemein",
                ["Options.Tools"] = "Werkzeuge",
                ["Options.Support"] = "Support",
                ["Options.ResetColors"] = "Alle Farbeinstellungen zurücksetzen",
                ["Options.ResetColorsDesc"] = "Setzt alle RZColor-Regler auf 0 zurück (das ursprüngliche Aussehen des Spiels).",
                ["Options.ResetColorsWarning"] = "Alle RZColor-Einstellungen zurücksetzen?",
                ["Options.Compatibility"] = "Kompatibilität",
                ["Options.VolumePriority"] = "Volume-Priorität",
                ["Options.VolumePriorityDesc"] = "Priorität des globalen HDRP-Volumes namens „RZColor“. Wenn RZColor und ein anderer Mod (z. B. Lumina oder Lumina NGX) denselben Effekt ändern, gewinnt das Volume mit der höheren Priorität. Lumina verwendet 5000. Standard: 4900.",
                ["Options.OpenLogFolder"] = "Log-Ordner öffnen",
                ["Options.OpenLogFolderDesc"] = "Öffnet den Ordner mit RZColor.log. Füge diese Datei hinzu, wenn du ein Problem meldest.",
                ["Options.OpenSettingsFolder"] = "Einstellungsordner öffnen",
                ["Options.OpenSettingsFolderDesc"] = "Öffnet den Ordner mit RZColor.json, in dem deine Farbeinstellungen gespeichert sind. Teile diese Datei, um deinen Look zu teilen. Deine Spielstände werden nie verändert.",
            },

            ["es-ES"] = new Dictionary<string, string>
            {
                ["Panel.ContrastParameters"] = "Parámetros de contraste",
                ["Panel.Blacks"] = "Negros",
                ["Panel.Whites"] = "Blancos",
                ["Panel.Contrast"] = "Contraste",
                ["Panel.Gamma"] = "Gamma",
                ["Panel.WhiteBalance"] = "Balance de blancos",
                ["Panel.Saturation"] = "Saturación",
                ["Panel.Temperature"] = "Temp.",
                ["Panel.Tint"] = "Matiz",
                ["Panel.ColorMixer"] = "Mezclador de color",
                ["Panel.Hue"] = "Tono",
                ["Panel.ResetAll"] = "Restablecer todo",
                ["Panel.On"] = "Activado",
                ["Panel.Off"] = "Desactivado",

                ["Options.Tab"] = "Principal",
                ["Options.Tools"] = "Herramientas",
                ["Options.Support"] = "Soporte",
                ["Options.ResetColors"] = "Restablecer todos los ajustes de color",
                ["Options.ResetColorsDesc"] = "Devuelve todos los controles de RZColor a 0 (el aspecto original del juego).",
                ["Options.ResetColorsWarning"] = "¿Restablecer todos los ajustes de RZColor?",
                ["Options.Compatibility"] = "Compatibilidad",
                ["Options.VolumePriority"] = "Prioridad del volumen",
                ["Options.VolumePriorityDesc"] = "Prioridad del volumen HDRP global llamado «RZColor». Cuando RZColor y otro mod (como Lumina o Lumina NGX) cambian el mismo efecto, gana el volumen con mayor prioridad. Lumina usa 5000. Predeterminado: 4900.",
                ["Options.OpenLogFolder"] = "Abrir carpeta de registros",
                ["Options.OpenLogFolderDesc"] = "Abre la carpeta con RZColor.log. Adjunta este archivo al informar de un problema.",
                ["Options.OpenSettingsFolder"] = "Abrir carpeta de ajustes",
                ["Options.OpenSettingsFolderDesc"] = "Abre la carpeta con RZColor.json, donde se guardan tus ajustes de color. Comparte este archivo para compartir tu estilo. Tus partidas guardadas nunca se modifican.",
            },

            ["zh-HANS"] = new Dictionary<string, string>
            {
                ["Panel.ContrastParameters"] = "对比度参数",
                ["Panel.Blacks"] = "黑色",
                ["Panel.Whites"] = "白色",
                ["Panel.Contrast"] = "对比度",
                ["Panel.Gamma"] = "伽马",
                ["Panel.WhiteBalance"] = "白平衡",
                ["Panel.Saturation"] = "饱和度",
                ["Panel.Temperature"] = "色温",
                ["Panel.Tint"] = "色调",
                ["Panel.ColorMixer"] = "混色器",
                ["Panel.Hue"] = "色相",
                ["Panel.ResetAll"] = "全部重置",
                ["Panel.On"] = "开",
                ["Panel.Off"] = "关",

                ["Options.Tab"] = "主要",
                ["Options.Tools"] = "工具",
                ["Options.Support"] = "支持",
                ["Options.ResetColors"] = "重置所有颜色设置",
                ["Options.ResetColorsDesc"] = "将 RZColor 的所有滑块恢复为 0（游戏原始画面）。",
                ["Options.ResetColorsWarning"] = "要重置所有 RZColor 设置吗？",
                ["Options.Compatibility"] = "兼容性",
                ["Options.VolumePriority"] = "Volume 优先级",
                ["Options.VolumePriorityDesc"] = "名为“RZColor”的全局 HDRP Volume 的优先级。当 RZColor 与其他模组（如 Lumina 或 Lumina NGX）修改同一效果时，优先级较高的 Volume 生效。Lumina 使用 5000。默认值：4900。",
                ["Options.OpenLogFolder"] = "打开日志文件夹",
                ["Options.OpenLogFolderDesc"] = "打开 RZColor.log 所在的文件夹。报告问题时请附上此文件。",
                ["Options.OpenSettingsFolder"] = "打开设置文件夹",
                ["Options.OpenSettingsFolderDesc"] = "打开 RZColor.json 所在的文件夹，其中保存了你的颜色设置。分享此文件即可分享你的画面风格。你的存档永远不会被修改。",
            },

            ["ko-KR"] = new Dictionary<string, string>
            {
                ["Panel.ContrastParameters"] = "대비 설정",
                ["Panel.Blacks"] = "검정",
                ["Panel.Whites"] = "흰색",
                ["Panel.Contrast"] = "대비",
                ["Panel.Gamma"] = "감마",
                ["Panel.WhiteBalance"] = "화이트 밸런스",
                ["Panel.Saturation"] = "채도",
                ["Panel.Temperature"] = "색온도",
                ["Panel.Tint"] = "색조",
                ["Panel.ColorMixer"] = "컬러 믹서",
                ["Panel.Hue"] = "색상",
                ["Panel.ResetAll"] = "모두 초기화",
                ["Panel.On"] = "켜기",
                ["Panel.Off"] = "끄기",

                ["Options.Tab"] = "기본",
                ["Options.Tools"] = "도구",
                ["Options.Support"] = "지원",
                ["Options.ResetColors"] = "모든 색상 설정 초기화",
                ["Options.ResetColorsDesc"] = "RZColor의 모든 슬라이더를 0(게임 원래 화면)으로 되돌립니다.",
                ["Options.ResetColorsWarning"] = "RZColor 설정을 모두 초기화할까요?",
                ["Options.Compatibility"] = "호환성",
                ["Options.VolumePriority"] = "볼륨 우선순위",
                ["Options.VolumePriorityDesc"] = "\"RZColor\"라는 이름의 전역 HDRP 볼륨 우선순위입니다. RZColor와 다른 모드(Lumina, Lumina NGX 등)가 같은 효과를 변경하면 우선순위가 높은 볼륨이 적용됩니다. Lumina는 5000을 사용합니다. 기본값: 4900.",
                ["Options.OpenLogFolder"] = "로그 폴더 열기",
                ["Options.OpenLogFolderDesc"] = "RZColor.log가 있는 폴더를 엽니다. 문제를 신고할 때 이 파일을 첨부하세요.",
                ["Options.OpenSettingsFolder"] = "설정 폴더 열기",
                ["Options.OpenSettingsFolderDesc"] = "색상 설정이 저장된 RZColor.json 폴더를 엽니다. 이 파일을 공유하면 화면 스타일을 공유할 수 있습니다. 세이브 파일은 절대 변경되지 않습니다.",
            },
        };

        // Registra todos os idiomas no jogo. Chamado uma vez no OnLoad.
        public static void Register(Setting setting)
        {
            var manager = GameManager.instance.localizationManager;
            var english = Texts["en-US"];

            foreach (var pair in Texts)
            {
                // Começa do inglês e sobrescreve com o idioma: se faltar alguma chave, fica em inglês.
                var t = new Dictionary<string, string>(english);
                foreach (var kv in pair.Value) t[kv.Key] = kv.Value;

                manager.AddSource(pair.Key, new LocaleSource(BuildEntries(setting, t)));
            }
        }

        // Converte as chaves curtas nas chaves que o jogo espera.
        private static Dictionary<string, string> BuildEntries(Setting s, Dictionary<string, string> t)
        {
            var e = new Dictionary<string, string>
            {
                // Página de Opções
                { s.GetSettingsLocaleID(), "RZColor" },
                { s.GetOptionTabLocaleID(Setting.kSection), t["Options.Tab"] },
                { s.GetOptionGroupLocaleID(Setting.kToolsGroup), t["Options.Tools"] },
                { s.GetOptionGroupLocaleID(Setting.kSupportGroup), t["Options.Support"] },
                { s.GetOptionGroupLocaleID(Setting.kCompatGroup), t["Options.Compatibility"] },

                { s.GetOptionLabelLocaleID(nameof(Setting.VolumePriority)), t["Options.VolumePriority"] },
                { s.GetOptionDescLocaleID(nameof(Setting.VolumePriority)), t["Options.VolumePriorityDesc"] },

                { s.GetOptionLabelLocaleID(nameof(Setting.ResetColors)), t["Options.ResetColors"] },
                { s.GetOptionDescLocaleID(nameof(Setting.ResetColors)), t["Options.ResetColorsDesc"] },
                { s.GetOptionWarningLocaleID(nameof(Setting.ResetColors)), t["Options.ResetColorsWarning"] },

                { s.GetOptionLabelLocaleID(nameof(Setting.OpenLogFolder)), t["Options.OpenLogFolder"] },
                { s.GetOptionDescLocaleID(nameof(Setting.OpenLogFolder)), t["Options.OpenLogFolderDesc"] },

                { s.GetOptionLabelLocaleID(nameof(Setting.OpenSettingsFolder)), t["Options.OpenSettingsFolder"] },
                { s.GetOptionDescLocaleID(nameof(Setting.OpenSettingsFolder)), t["Options.OpenSettingsFolderDesc"] },
            };

            // Painel: "Panel.Blacks" vira "RZColor.Panel.Blacks"
            foreach (var kv in t)
            {
                if (kv.Key.StartsWith("Panel."))
                    e[PanelPrefix + kv.Key.Substring("Panel.".Length)] = kv.Value;
            }
            return e;
        }

        // "Fonte" de textos que o jogo consulta. Só devolve a tabela pronta.
        private class LocaleSource : IDictionarySource
        {
            private readonly Dictionary<string, string> m_Entries;
            public LocaleSource(Dictionary<string, string> entries) { m_Entries = entries; }

            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
                => m_Entries;

            public void Unload() { }
        }
    }
}
