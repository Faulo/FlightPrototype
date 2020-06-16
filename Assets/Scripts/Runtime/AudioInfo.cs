using UnityEngine;
using UnityEngine.Audio;

namespace TheCursedBroom {
    public class AudioInfo {
        public AudioClip clip;
        public bool loop;
        public AudioMixerGroup mixer;
        public float pitch;
        public float timeOffset;
    }
}