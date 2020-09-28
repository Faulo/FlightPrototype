using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public class BroomParticles : ComponentFeature<ParticleSystem> {
        [SerializeField, Expandable]
        AvatarController avatar = default;

        [Header("Broom Particles")]
        [SerializeField]
        BroomState requiredState = default;

        ParticleSystem.EmissionModule emission;

        protected override void OnValidate() {
            base.OnValidate();
            if (!avatar) {
                avatar = GetComponentInParent<AvatarController>();
            }
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
        }
    }
}