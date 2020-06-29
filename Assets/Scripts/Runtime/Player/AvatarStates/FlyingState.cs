using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class FlyingState : AvatarState {
        [Header("Flying")]
        [SerializeField, Expandable]
        ParticleSystem updraftParticles = default;

        bool hasCollided;
        public override void EnterState() {
            base.EnterState();

            updraftParticles.Play();

            hasCollided = false;
            avatar.physics.onCollisionEnter += CollisionListener;

            avatar.UpdateMovement();
        }

        public override void FixedUpdateState() {
            base.FixedUpdateState();

            //updraftParticles.transform.rotation = avatar.intendedLookRotation;

            var main = updraftParticles.main;
            //main.startSpeed = new ParticleSystem.MinMaxCurve(avatar.intendedFlight.y, avatar.intendedFlight.y * 5);

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();

            avatar.physics.onCollisionEnter -= CollisionListener;

            updraftParticles.Stop();
        }


        void CollisionListener(Collision2D collision) {
            hasCollided = true;
        }


        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState rejectsGlideState = default;
        [SerializeField, Expandable]
        AvatarState wallCollisionState = default;
        public override AvatarState CalculateNextState() {
            if (hasCollided) {
                return wallCollisionState;
            }
            if (!avatar.intendsGlide || !avatar.canGlide) {
                return rejectsGlideState;
            }
            return this;
        }
    }
}