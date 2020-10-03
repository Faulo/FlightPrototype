using System.Collections;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom {
    public class AudioManager : MonoBehaviour {
        public static AudioManager instance;

        [SerializeField, Expandable]
        AudioSource sourcePrefab = default;

        void OnEnable() {
            instance = this;
        }

        public void Play(AudioInfo audio) {
            var source = Instantiate(sourcePrefab, transform);
            source.loop = audio.loop;
            source.clip = audio.clip;
            source.outputAudioMixerGroup = audio.mixer;
            source.pitch = audio.pitch;
            source.time = audio.timeOffset;
            source.Play();

            if (audio.playDuration > 0) {
                StartCoroutine(CreateMuteRoutine(source, audio.playDuration));
            }
            if (!audio.loop) {
                StartCoroutine(CreateDestructionRoutine(source, source.clip.length));
            }
        }

        IEnumerator CreateMuteRoutine(AudioSource source, float duration) {
            yield return new WaitForSeconds(duration);
            source.mute = true;
        }

        IEnumerator CreateDestructionRoutine(AudioSource source, float duration) {
            yield return new WaitForSeconds(duration);
            Destroy(source.gameObject);
        }
    }
}