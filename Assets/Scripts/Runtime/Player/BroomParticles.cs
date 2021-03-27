using Slothsoft.UnityExtensions;
using TheCursedBroom.Assets;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public class BroomParticles : ComponentFeature<ParticleSystem> {
        [SerializeField, Expandable]
        AvatarController avatar = default;

        [Header("Broom Particles")]
        [SerializeField]
        BroomState requiredState = default;
        [SerializeField, Expandable]
        ColorAsset particleColor = default;

        ParticleSystem.EmissionModule emission;
        ParticleSystem.MainModule main;

        protected override void OnValidate() {
            base.OnValidate();
            if (!avatar) {
                avatar = GetComponentInParent<AvatarController>();
            }
        }

        void Start() {
            Assert.IsTrue(observedComponent);
            Assert.IsTrue(avatar);
            emission = observedComponent.emission;
            main = observedComponent.main;
            UpdateColor();
        }

        void Update() {
            bool enabled = requiredState == avatar.broom.state;
            if (emission.enabled != enabled) {
                emission.enabled = enabled;
            }
#if UNITY_EDITOR
            UpdateColor();
#endif
        }

        void UpdateColor() {
            if (observedComponent && particleColor) {
                main.startColor = particleColor.color;
            }
        }
    }
}