using System.Reflection;
using Colossal.UI.Binding;
using Game.UI;
using UnityEngine;

namespace RZColor.Systems
{
    // "Ponte" entre o C# e o painel em React.
    //
    // Dois tipos de ligação (binding):
    //   - ValueBinding  -> o C# MANDA um valor para o painel (aqui: todos os ajustes, em JSON)
    //   - TriggerBinding -> o painel CHAMA uma função do C# (ex.: "slider mudou")
    //
    // Pense como um walkie-talkie: o painel escuta o canal "state" e fala nos canais "setValue",
    // "setEnabled" e "resetAll".
    public partial class RZColorUISystem : UISystemBase
    {
        // Nome do "grupo" das ligações. Precisa ser igual ao usado no React (mod.json -> id).
        private const string Group = "RZColor";

        // Espera meio segundo depois do último movimento do slider antes de gravar no disco
        // (evita salvar o arquivo 60 vezes por segundo enquanto você arrasta).
        private const float SaveDelay = 0.5f;

        private float m_SaveAt = -1f;

        protected override void OnCreate()
        {
            base.OnCreate();

            // O painel lê "state". GetterValueBinding checa a cada frame se o texto mudou
            // e só envia quando muda — inclusive se você editar o JSON na mão.
            AddUpdateBinding(new GetterValueBinding<string>(Group, "state", GetStateJson));

            // O painel chama estas funções:
            AddBinding(new TriggerBinding<string, float>(Group, "setValue", SetValue));
            AddBinding(new TriggerBinding<bool>(Group, "setEnabled", SetEnabled));
            AddBinding(new TriggerBinding(Group, "resetAll", ResetAll));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate(); // importante: é aqui que as ligações são atualizadas

            // Salvamento atrasado (ver SaveDelay)
            if (m_SaveAt > 0f && UnityEngine.Time.unscaledTime >= m_SaveAt)
            {
                m_SaveAt = -1f;
                ColorVolumeSystem.Instance?.MarkDirty(saveToDisk: true);
            }
        }

        // O jogo pergunta este valor a cada frame. Para não gerar texto novo à toa
        // (o que criaria "lixo" na memória), só refazemos o JSON quando a versão muda.
        private string m_CachedJson = "{}";
        private int m_CachedVersion = -1;

        private string GetStateJson()
        {
            var sys = ColorVolumeSystem.Instance;
            if (sys?.Settings == null) return "{}";
            if (sys.Version != m_CachedVersion)
            {
                m_CachedVersion = sys.Version;
                m_CachedJson = JsonUtility.ToJson(sys.Settings);
            }
            return m_CachedJson;
        }

        // Muda um ajuste pelo nome do campo (ex.: "Saturation", "Greens_Hue").
        private void SetValue(string key, float value)
        {
            var sys = ColorVolumeSystem.Instance;
            if (sys?.Settings == null) return;

            // Reflection: procura o campo com esse nome dentro de ColorSettings.
            // Equivale a settings[key] = value em Lua.
            FieldInfo field = typeof(ColorSettings).GetField(key);
            if (field == null || field.FieldType != typeof(float))
            {
                Mod.Log.Warn($"Unknown setting received from the panel: {key}");
                return;
            }

            field.SetValue(sys.Settings, Mathf.Clamp(value, -100f, 100f));
            Changed(sys);
        }

        private void SetEnabled(bool enabled)
        {
            var sys = ColorVolumeSystem.Instance;
            if (sys?.Settings == null) return;
            sys.Settings.Enabled = enabled;
            Changed(sys);
        }

        // Zera tudo (mantém o mod ligado).
        private void ResetAll()
        {
            var sys = ColorVolumeSystem.Instance;
            if (sys == null) return;
            sys.ReplaceSettings(new ColorSettings());
            m_SaveAt = UnityEngine.Time.unscaledTime; // salva já
        }

        private void Changed(ColorVolumeSystem sys)
        {
            sys.MarkDirty(saveToDisk: false);           // aplica na tela agora
            m_SaveAt = UnityEngine.Time.unscaledTime + SaveDelay;   // grava no disco daqui a pouco
        }
    }
}
