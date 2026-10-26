using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using RZColor.Systems;

namespace RZColor
{
    // Classe principal do mod. O jogo procura uma classe que implemente IMod
    // e chama OnLoad quando o mod é carregado — como um "on_load" em Lua.
    public class Mod : IMod
    {
        public const string ModName = "RZColor";
        public const string Version = "1.2";

        // Logger: as mensagens vão para ...\Cities Skylines II\Logs\RZColor.log
        public static readonly ILog Log = LogManager.GetLogger(ModName).SetShowsErrorsInUI(false);

        private Setting m_Setting;

        // Acesso global às opções (o ColorVolumeSystem lê a prioridade daqui).
        public static Setting Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Garante que o RZColor.log seja gravado mesmo quando o nível de log do jogo está baixo.
            Log.effectivenessLevel = Level.Info;

            Log.Info($"RZColor {Version} loading...");

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                Log.Info($"Mod file: {asset.path}");

            // Página em Opções do jogo + textos de todos os idiomas (Localization.cs)
            m_Setting = new Setting(this);
            // Carrega as opções salvas (ex.: prioridade do volume) do arquivo
            // ModsSettings\RZColor\RZColor.coc. Fica fora dos saves, como o RZColor.json.
            AssetDatabase.global.LoadSettings(ModName, m_Setting, new Setting(this));
            Settings = m_Setting;
            m_Setting.RegisterInOptionsUI();
            RZLocalization.Register(m_Setting);

            // Sistema que aplica as cores. PreCulling = todo frame, antes de renderizar.
            updateSystem.UpdateAt<ColorVolumeSystem>(SystemUpdatePhase.PreCulling);

            // "Ponte" com o painel React. UIUpdate = fase da interface.
            updateSystem.UpdateAt<RZColorUISystem>(SystemUpdatePhase.UIUpdate);

            Log.Info("RZColor loaded.");
        }

        public void OnDispose()
        {
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
            Settings = null;
            Log.Info("RZColor unloaded.");
        }
    }
}
