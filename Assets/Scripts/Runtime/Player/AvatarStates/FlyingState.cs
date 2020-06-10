using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class FlyingState : AvatarState {
        [Header("Flying")]
        [SerializeField, Expandable]
        ParticleSystem updraftParticles = default;
        public override void EnterState() {
            base.EnterState();

            updraftParticles.Play();

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            var main = updraftParticles.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(avatar.intendedFlight.y, avatar.intendedFlight.y * 5);

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();

            updraftParticles.Stop();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState rejectsGlideState = default;
        public override AvatarState CalculateNextState() {
            if (!avatar.intendsGlide || !avatar.canGlide) {
                return rejectsGlideState;
            }
            return this;
        }
    }
}