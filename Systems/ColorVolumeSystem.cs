using System;
using System.IO;
using Game;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace RZColor.Systems
{
    // Sistema que cria um "Volume" do HDRP só do RZColor e aplica os ajustes de cor nele.
    //
    // POR QUE ESSES COMPONENTES?
    // O Lumina clássico usa ColorAdjustments, WhiteBalance, ShadowsMidtonesHighlights e Tonemapping.
    // O RZColor usa SOMENTE componentes que o Lumina clássico não toca:
    //   - LiftGammaGain -> pretos, gama, brancos
    //   - ColorCurves   -> saturação, contraste e cor seletiva (CMYRGB)
    //   - ChannelMixer  -> temperatura e tint
    // Assim os dois mods não disputam o mesmo parâmetro: os efeitos se SOMAM.
    //
    // E O LUMINA NGX?
    // O NGX consegue editar QUALQUER componente de QUALQUER volume (inclusive o nosso, pelo nome
    // "RZColor"). Se dois volumes mexem no mesmo efeito, vence o de prioridade maior — por isso a
    // prioridade é ajustável em Opções. Se o NGX remover nossos componentes, eles voltam na
    // próxima vez que o jogador mexer num ajuste do RZColor.
    //
    // POR QUE É SEGURO PARA OS SAVES?
    // Nada aqui é salvo dentro da cidade. O Volume é um objeto temporário que só existe
    // enquanto o jogo está aberto, e os ajustes ficam num arquivo JSON separado.
    public partial class ColorVolumeSystem : GameSystemBase
    {
        // Nome do objeto/volume. É assim que o Lumina NGX o mostra na lista de volumes.
        public const string VolumeName = "RZColor";

        // Prioridade atual: vem das Opções (padrão 4900; o Lumina usa 5000).
        private static float CurrentPriority =>
            Mod.Settings != null ? Mod.Settings.VolumePriority : Setting.DefaultPriority;

        // Acesso global para o painel (etapa 2) conseguir ler e mudar os ajustes.
        public static ColorVolumeSystem Instance { get; private set; }

        public ColorSettings Settings { get; private set; }

        private GameObject m_Root;
        private Volume m_Volume;
        private VolumeProfile m_Profile;
        private LiftGammaGain m_LGG;
        private ColorCurves m_Curves;
        private ChannelMixer m_Mixer;

        // "Número da versão" dos ajustes: sobe toda vez que algo muda.
        // O painel usa isso para só gerar o JSON de novo quando necessário.
        public int Version { get; private set; }

        // Últimos valores aplicados nas curvas (para não recriar curva que não mudou).
        private float m_LastSat = float.NaN;
        private float m_LastContrast = float.NaN;
        private string m_LastSelective;

        private bool m_Dirty = true;            // "precisa reaplicar?"
        private DateTime m_LastFileWrite;       // para detectar edição manual do JSON
        private float m_NextFileCheck;          // cronômetro da checagem

        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            Settings = ColorSettings.Load();
            m_LastFileWrite = SafeWriteTime();
            Mod.Log.Info($"Settings file: {ColorSettings.FilePath}");
        }

        // Roda a cada frame.
        protected override void OnUpdate()
        {
            if (m_Volume == null)
                CreateVolume();

            CheckFileForChanges();

            if (m_Dirty)
            {
                Apply(Settings);
                m_Dirty = false;
            }
        }

        protected override void OnDestroy()
        {
            // Libera as texturas das curvas e o perfil, além do objeto do volume.
            if (m_Curves != null)
            {
                m_Curves.satVsSat.Release();
                m_Curves.master.Release();
                m_Curves.hueVsHue.Release();
                m_Curves.hueVsSat.Release();
            }
            if (m_Profile != null)
                UnityEngine.Object.Destroy(m_Profile);
            if (m_Root != null)
                UnityEngine.Object.Destroy(m_Root);
            Instance = null;
            base.OnDestroy();
        }

        // Chamado pelo painel quando um slider muda (etapa 2).
        public void MarkDirty(bool saveToDisk)
        {
            m_Dirty = true;
            Version++;
            if (saveToDisk)
            {
                Settings.Save();
                m_LastFileWrite = SafeWriteTime();
            }
        }

        // Muda a prioridade do volume na hora (slider em Opções).
        public void SetPriority(float priority)
        {
            if (m_Volume != null)
                m_Volume.priority = priority;
        }

        // Troca todos os ajustes de uma vez (usado pelo botão de reset).
        public void ReplaceSettings(ColorSettings fresh)
        {
            Settings = fresh;
            m_Dirty = true;
            Version++;
        }

        // ------------------------------------------------------------------
        // Criação do volume
        // ------------------------------------------------------------------
        private void CreateVolume()
        {
            m_Root = new GameObject(VolumeName);
            UnityEngine.Object.DontDestroyOnLoad(m_Root); // sobrevive à troca de cena (menu <-> cidade)

            m_Volume = m_Root.AddComponent<Volume>();
            m_Volume.isGlobal = true;               // vale para a tela inteira
            m_Volume.priority = CurrentPriority;

            // Usamos "profile" (e não "sharedProfile"): assim, quando outro mod (ex.: Lumina NGX)
            // lê volume.profile, ele recebe ESTE mesmo perfil, e não uma cópia que deixaria
            // nossos sliders sem efeito.
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            profile.name = VolumeName;
            m_Volume.profile = profile;
            m_Profile = profile;

            m_LGG = profile.Add<LiftGammaGain>();
            m_Curves = profile.Add<ColorCurves>();
            m_Mixer = profile.Add<ChannelMixer>();

            ResetCurveCache();
            m_Dirty = true;
            Mod.Log.Info($"RZColor volume created (priority {m_Volume.priority}).");
        }

        // ------------------------------------------------------------------
        // Edição ao vivo do JSON (ferramenta de teste da etapa 1).
        // A cada 1 segundo, vê se o arquivo mudou; se mudou, recarrega.
        // ------------------------------------------------------------------
        private void CheckFileForChanges()
        {
            if (UnityEngine.Time.unscaledTime < m_NextFileCheck)
                return;
            m_NextFileCheck = UnityEngine.Time.unscaledTime + 1f;

            var t = SafeWriteTime();
            if (t != m_LastFileWrite)
            {
                m_LastFileWrite = t;
                Settings = ColorSettings.Load();
                m_Dirty = true;
                Version++;
                Mod.Log.Info("Settings file changed, reloaded.");
            }
        }

        private static DateTime SafeWriteTime()
        {
            try { return File.GetLastWriteTimeUtc(ColorSettings.FilePath); }
            catch { return DateTime.MinValue; }
        }

        // ------------------------------------------------------------------
        // Aplicação dos ajustes
        // Regra de ouro: se o slider está em 0, o parâmetro fica DESLIGADO (overrideState = false).
        // Assim, valores neutros não interferem em nada do jogo nem de outros mods.
        // ------------------------------------------------------------------
        private void Apply(ColorSettings s)
        {
            if (m_Volume == null) return;

            m_Volume.enabled = s.Enabled;
            if (!s.Enabled) return;

            EnsureComponents();

            ApplyLiftGammaGain(s);
            ApplyChannelMixer(s);
            ApplyCurves(s);
        }

        // Confere se nossos componentes ainda estão no perfil do volume.
        // Outro mod (ex.: Lumina NGX) pode ter removido algum ou trocado o perfil:
        // nesse caso, pegamos o que existe ou adicionamos de novo.
        private void EnsureComponents()
        {
            var profile = m_Volume.profile;
            bool changed = profile != m_Profile;
            m_Profile = profile;

            m_LGG = GetOrAdd(profile, m_LGG, ref changed);
            m_Curves = GetOrAdd(profile, m_Curves, ref changed);
            m_Mixer = GetOrAdd(profile, m_Mixer, ref changed);

            if (changed)
            {
                ResetCurveCache(); // força recriar as curvas no componente atual
                Mod.Log.Info("RZColor volume components were changed by another mod; restored.");
            }
        }

        private static T GetOrAdd<T>(VolumeProfile profile, T current, ref bool changed) where T : VolumeComponent
        {
            if (current != null && profile.components.Contains(current))
                return current;
            changed = true;
            if (profile.TryGet<T>(out var found))
                return found;
            return profile.Add<T>();
        }

        // Esquece os últimos valores das curvas, para elas serem recriadas no próximo Apply.
        private void ResetCurveCache()
        {
            m_LastSat = float.NaN;
            m_LastContrast = float.NaN;
            m_LastSelective = null;
        }

        private static bool IsZero(float v) => Mathf.Abs(v) < 0.01f;

        // Pretos / Gama / Brancos
        // O HDRP usa só o 4º número (w) como "intensidade"; x,y,z = 1 mantém a cor neutra.
        private void ApplyLiftGammaGain(ColorSettings s)
        {
            // Pretos: soma um pequeno valor nas sombras. ±100 -> ±0.1 (forte o bastante, sem lavar a imagem)
            SetVec(m_LGG.lift, s.Blacks, 0.10f);
            // Gama: expoente 1/(1+w). +100 clareia os meios-tons, -100 escurece.
            SetVec(m_LGG.gamma, s.Gamma, 0.50f);
            // Brancos: multiplicador 1+w. ±100 -> 0.5x a 1.5x
            SetVec(m_LGG.gain, s.Whites, 0.50f);
        }

        private static void SetVec(Vector4Parameter p, float slider, float scale)
        {
            p.Override(new Vector4(1f, 1f, 1f, slider / 100f * scale));
            p.overrideState = !IsZero(slider);
        }

        // Temperatura e Tint via Channel Mixer (valores em %; 100 = neutro).
        private void ApplyChannelMixer(ColorSettings s)
        {
            float t = s.Temperature / 100f * 15f;   // até ±15% no vermelho/azul
            float g = s.Tint / 100f * 10f;          // até ±10% no verde

            bool tempOn = !IsZero(s.Temperature);
            bool tintOn = !IsZero(s.Tint);

            m_Mixer.redOutRedIn.Override(100f + t);
            m_Mixer.redOutRedIn.overrideState = tempOn;

            m_Mixer.blueOutBlueIn.Override(100f - t);
            m_Mixer.blueOutBlueIn.overrideState = tempOn;

            // Tint positivo = magenta (tira verde), como no Lightroom/Camera Raw
            m_Mixer.greenOutGreenIn.Override(100f - g);
            m_Mixer.greenOutGreenIn.overrideState = tintOn;
        }

        // Saturação, contraste e cor seletiva — tudo via curvas.
        private void ApplyCurves(ColorSettings s)
        {
            // --- Saturação global: curva "Sat vs Sat" constante. 0.5 = neutro, 1.0 = 2x.
            if (s.Saturation != m_LastSat)
            {
                m_LastSat = s.Saturation;
                float sat = 0.5f * (1f + s.Saturation / 100f);
                SetCurve(m_Curves.satVsSat,
                    new TextureCurve(new[] { new Keyframe(0f, sat, 0f, 0f), new Keyframe(1f, sat, 0f, 0f) }, 0.5f, false, new Vector2(0f, 1f)),
                    !IsZero(s.Saturation));
            }

            // --- Contraste: curva em "S" passando por um ponto de cinza médio.
            // Inclinação maior no meio = mais contraste; menor = menos.
            if (s.Contrast != m_LastContrast)
            {
                m_LastContrast = s.Contrast;
                const float pivot = 0.18f;
                float k = s.Contrast / 100f * 0.6f;
                SetCurve(m_Curves.master,
                    new TextureCurve(new[]
                    {
                        new Keyframe(0f, 0f, 1f - k, 1f - k),
                        new Keyframe(pivot, pivot, 1f + k, 1f + k),
                        new Keyframe(1f, 1f, 1f - k, 1f - k),
                    }, 0f, false, new Vector2(0f, 1f)),
                    !IsZero(s.Contrast));
            }

            // --- Cor seletiva (CMYRGB), no estilo do Photoshop.
            // "Assinatura" com os 12 valores: se nada mudou, não recria as curvas.
            var sig = new System.Text.StringBuilder();
            for (int i = 0; i < 6; i++) sig.Append(s.GetHue(i)).Append(';').Append(s.GetSat(i)).Append(';');
            string key = sig.ToString();
            if (key != m_LastSelective)
            {
                m_LastSelective = key;
                BuildSelectiveCurves(s);
            }
        }

        // Centro de cada faixa, em graus: vermelho, amarelo, verde, ciano, azul e magenta.
        // O HDRP corrige as cores no espaço ACEScg, onde a roda fica "torta" em relação ao
        // Photoshop (ex.: o verde puro cai em ~103°, não em 120°). Esses são os valores convertidos.
        private static readonly float[] HueCenters = { 5f, 62f, 103f, 185f, 242f, 283f };

        // Largura das faixas, em graus:
        // até BandFull de distância do centro = efeito 100%;
        // de BandFull até BandEdge = vai sumindo suavemente; depois disso = 0%.
        private const float BandFull = 20f;
        private const float BandEdge = 60f;

        // Quantos pontos usamos para desenhar a curva (um a cada 5°).
        private const int CurveSamples = 72;

        // Quanto cada faixa "pesa" num determinado matiz (0 a 1).
        private static float BandWeight(float hueDeg, float centerDeg)
        {
            // distância circular: 350° e 10° estão a 20° um do outro
            float d = Mathf.Abs(Mathf.DeltaAngle(hueDeg, centerDeg));
            if (d <= BandFull) return 1f;
            if (d >= BandEdge) return 0f;
            float t = 1f - (d - BandFull) / (BandEdge - BandFull);
            return t * t * (3f - 2f * t); // smoothstep: transição suave
        }

        // Desenha as curvas "Hue vs Hue" e "Hue vs Sat" somando o efeito das 6 faixas.
        // Pense como várias "lanternas" na roda de cores: cada slider acende a sua,
        // e onde duas se sobrepõem os efeitos se somam.
        private void BuildSelectiveCurves(ColorSettings s)
        {
            bool anyHue = false, anySat = false;
            for (int i = 0; i < 6; i++)
            {
                anyHue |= !IsZero(s.GetHue(i));
                anySat |= !IsZero(s.GetSat(i));
            }

            var hueVals = new float[CurveSamples];
            var satVals = new float[CurveSamples];

            for (int k = 0; k < CurveSamples; k++)
            {
                float deg = k * 360f / CurveSamples;
                float hueShift = 0f; // em "% do máximo"
                float satShift = 0f;

                for (int i = 0; i < 6; i++)
                {
                    float w = BandWeight(deg, HueCenters[i]);
                    hueShift += s.GetHue(i) / 100f * w;
                    satShift += s.GetSat(i) / 100f * w;
                }

                // Matiz: ±100 -> desloca até ±30° (30/360 = 0.0833). 0.5 = neutro.
                hueVals[k] = 0.5f + Mathf.Clamp(hueShift, -1f, 1f) * 0.0833f;
                // Saturação: ±100 -> 0x a 2x. 0.5 = neutro.
                satVals[k] = 0.5f * (1f + Mathf.Clamp(satShift, -1f, 1f));
            }

            SetCurve(m_Curves.hueVsHue, MakeLoopCurve(hueVals), anyHue);
            SetCurve(m_Curves.hueVsSat, MakeLoopCurve(satVals), anySat);
        }

        // Converte a lista de valores numa curva suave que "dá a volta" (360° = 0°).
        private static TextureCurve MakeLoopCurve(float[] v)
        {
            int n = v.Length;
            float step = 1f / n;
            var keys = new Keyframe[n];
            for (int k = 0; k < n; k++)
            {
                // inclinação calculada pelos vizinhos, para a curva não ficar "em degraus"
                float prev = v[(k - 1 + n) % n];
                float next = v[(k + 1) % n];
                float tangent = (next - prev) / (2f * step);
                keys[k] = new Keyframe(k * step, v[k], tangent, tangent);
            }
            return new TextureCurve(keys, 0.5f, true, new Vector2(0f, 1f));
        }

        // Troca a curva antiga pela nova e libera a textura da antiga da memória da placa de vídeo.
        private static void SetCurve(TextureCurveParameter p, TextureCurve curve, bool on)
        {
            var old = p.value;
            p.Override(curve);
            p.overrideState = on;
            old?.Release();
        }
    }
}
