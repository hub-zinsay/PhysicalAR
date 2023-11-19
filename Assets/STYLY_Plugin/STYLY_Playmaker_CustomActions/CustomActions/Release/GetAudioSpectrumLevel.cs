#if PLAYMAKER

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    [ActionCategory("STYLY")]
    [HelpUrl("https://styly.cc/ja/tips/audiovisualizer_discont_playmaker/")]
    public class GetAudioSpectrumLevel : FsmStateAction
    {

        AudioSpectrum spectrum;

        [Tooltip("Index of Band")]
        public FsmInt Index;

        public enum BarType { Realtime, PeakLevel, MeanLevel };
        public BarType barType;
        [HasFloatSlider(1f, 300f)]
        public float multiplier = 100;

        [ActionSection("Results")]
        public FsmFloat StoreValue;

        public override void Awake()
        {
            base.Awake();
        }

        public override void OnEnter()
        {
            spectrum = UnityEngine.Object.FindObjectOfType<AudioSpectrum>();
            if (null == spectrum)
            {
                ActionHelpers.DebugLog(Fsm, LogLevel.Error, "AudioSpectrum not found.", true);
                Finish();
            }
        }

        public override void OnUpdate()
        {
            StoreValue.Value = multiplier * GetLevel(Index.Value);
        }

        public override void Reset()
        {
            base.Reset();
            StoreValue = null;
            Index = null;
            barType = BarType.Realtime;
            multiplier = 100;
        }

        float GetLevel(int index)
        {
            float level = 0.0f;
            if (index < spectrum.Levels.Length)
            {
                switch (barType)
                {
                    case BarType.Realtime:
                        level = spectrum.Levels[index];
                        break;
                    case BarType.PeakLevel:
                        level = spectrum.PeakLevels[index];
                        break;
                    case BarType.MeanLevel:
                        level = spectrum.MeanLevels[index];
                        break;
                }
            }
            return level;
        }
    }
}
#endif