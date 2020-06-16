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

            if (!audio.loop) {
                StartCoroutine(CreateDestructionRoutine(source));
            }
        }

        IEnumerator CreateDestructionRoutine(AudioSource source) {
            yield return new WaitForSeconds(source.clip.length);
            Destroy(source.gameObject);
        }
    }
}