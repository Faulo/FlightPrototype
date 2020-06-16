using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Audio;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu()]
    public class AudioEffect : Effect {
        [SerializeField, Expandable]
        AudioClip[] clips = default;

        [SerializeField, Expandable]
        AudioMixerGroup mixer = default;

        [SerializeField, Range(0, 2)]
        float mininumPitch = 1;
        [SerializeField, Range(0, 2)]
        float maxmimumPitch = 1;
        [SerializeField]
        bool loop = false;
        [SerializeField, Range(0, 10)]
        float timeOffset = 0;

        public override void Invoke(GameObject context) {
            var source = context.AddComponent<AudioSource>();
            source.playOnAwake = true;
            source.loop = loop;
            source.clip = clips.RandomElement();
            source.outputAudioMixerGroup = mixer;
            source.pitch = Random.Range(mininumPitch, maxmimumPitch);
            source.time = timeOffset;

            source.Play();
        }
    }
}
