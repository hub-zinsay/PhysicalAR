#if PLAYMAKER
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    [ActionCategory("STYLY")]
    [HelpUrl("https://styly.cc/ja/tips/audiovisualizer_discont_playmaker/")]
    public class InitAudioSpectrum : FsmStateAction
    {
        public enum NumberOfSamples
        {
            sampling_256 = 256,
            sampling_512 = 512,
            sampling_1024 = 1024,
            sampling_2048 = 2048,
            sampling_4096 = 4096
        }

        public enum FrequencyBandType
        {
            FourBand,
            FourBandVisual,
            EightBand,
            TenBand,
            TwentySixBand,
            ThirtyOneBand
        }

        public NumberOfSamples numberOfSamples = NumberOfSamples.sampling_1024;

        public FrequencyBandType frequencyBandType = FrequencyBandType.TenBand;

        [HasFloatSlider(0.01f, 0.5f)]
        public FsmFloat fallSpeed = 0.08f;

        [HasFloatSlider(1f, 20f)]
        public FsmFloat sensibility = 8.0f;

        public override void Awake()
        {
            AudioSpectrum audioSpectrum = Owner.AddComponent<AudioSpectrum>();
            audioSpectrum.numberOfSamples = (int)numberOfSamples;
            audioSpectrum.bandType = (AudioSpectrum.BandType)System.Enum.ToObject(typeof(AudioSpectrum.BandType), (int)frequencyBandType);
            audioSpectrum.fallSpeed = fallSpeed.Value;
            audioSpectrum.sensibility = sensibility.Value;
        }

        public override void Reset()
        {
            base.Reset();
            numberOfSamples = NumberOfSamples.sampling_1024;
            frequencyBandType = FrequencyBandType.TenBand;
            fallSpeed = 0.08f;
            sensibility = 8.0f;
        }
    }
}
#endif