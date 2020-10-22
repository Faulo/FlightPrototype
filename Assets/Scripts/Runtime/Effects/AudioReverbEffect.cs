using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "AudioReverb_New", menuName = "Effects/Audio Reverb")]
    public class AudioReverbEffect : Effect {
        [SerializeField]
        AudioReverbPreset preset = AudioReverbPreset.Off;

        public override void Invoke(GameObject context) {
            var filter = FindObjectOfType<AudioReverbFilter>();
            if (filter) {
                filter.reverbPreset = preset;
            }
        }
    }
}
