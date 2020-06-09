using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class FlightState : AvatarState {
        [Header("Flight")]
        [SerializeField, Expandable]
        ParticleSystem updraftParticles = default;

        public override void EnterState() {
            base.EnterState();

            //avatar.attachedRigidbody.gravityScale = 0;
            //avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
            updraftParticles.Play();

            avatar.UpdateVelocity();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();
            var main = updraftParticles.main;

            main.startSpeed = new ParticleSystem.MinMaxCurve(avatar.intendedFlight.y, avatar.intendedFlight.y * 5);

            avatar.UpdateVelocity();
        }

        public override void ExitState() {
            base.ExitState();

            //avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //avatar.attachedRigidbody.rotation = 0;
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