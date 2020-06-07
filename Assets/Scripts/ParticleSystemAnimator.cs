using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom {
    public class ParticleSystemAnimator : MonoBehaviour {
        [SerializeField, Expandable]
        ParticleSystem particles = default;
        [SerializeField]
        bool isPlaying = false;

        ParticleSystem.EmissionModule emission;

        void OnEnable() {
            emission = particles.emission;
        }
        void Update() {
            emission.enabled = isPlaying;
        }
        void OnValidate() {
            if (!particles) {
                particles = GetComponent<ParticleSystem>();
            }
        }
    }
}