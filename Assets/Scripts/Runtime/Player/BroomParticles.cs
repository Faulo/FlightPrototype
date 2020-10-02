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

        protected override void OnValidate() {
            base.OnValidate();
            if (!avatar) {
                avatar = GetComponentInParent<AvatarController>();
            }
            UpdateColor();
        }

        void Start() {
            Assert.IsNotNull(observedComponent);
            Assert.IsNotNull(avatar);
            emission = observedComponent.emission;
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
                var main = observedComponent.main;
                main.startColor = particleColor.color;
            }
        }
    }
}