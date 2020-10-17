using UnityEngine;
using UnityEngine.Audio;

namespace TheCursedBroom {
    public class AudioInfo {
        public Vector3 position;
        public AudioClip clip;
        public bool loop;
        public AudioMixerGroup mixer;
        public float spatialBlend;
        public float pitch;
        public float timeOffset;
        public float playDuration;
    }
}