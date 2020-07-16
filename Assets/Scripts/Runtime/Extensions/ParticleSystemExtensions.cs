using UnityEngine;

namespace TheCursedBroom.Extensions {
    public static class ParticleSystemExtensions {
        public static void SetParticleColor(this ParticleSystem particles, Color color) {
            var main = particles.main;
            main.startColor = color;
        }
        public static void SetStartSpeedMultiplier(this ParticleSystem particles, float speed) {
            var main = particles.main;
            main.startSpeedMultiplier = speed;
        }
        public static void SetParticleCount(this ParticleSystem particles, int count) {
            var emission = particles.emission;
            emission.rateOverTime = count;
        }
    }
}