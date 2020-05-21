using UnityEngine;


namespace AvatarStateMachine {
    public class Gliding : AvatarStateBehaviour {
        [Header("Gliding movement")]
        [SerializeField, Range(0, 100)]
        float initialSpeed = 1;
        [SerializeField, Range(0, 100)]
        float rotationSpeed = 10;

        float glidingTimer;
        public override void EnterState(Avatar avatar) {
            base.EnterState(avatar);

            var velocity = avatar.attachedRigidbody.velocity;
            float rotation = avatar.attachedRigidbody.rotation;

            rotation = avatar.intendedRotation;
            velocity = Quaternion.Euler(0, 0, rotation) * Vector2.up * (velocity.magnitude + initialSpeed);

            avatar.attachedRigidbody.velocity = velocity;
            avatar.attachedRigidbody.rotation = rotation;

            avatar.glidingParticlesEnabled = true;
            avatar.UseDashCharge();
        }
        public override void FixedUpdateState(Avatar avatar) {
            base.FixedUpdateState(avatar);

            var velocity = avatar.attachedRigidbody.velocity;
            var rotation = Quaternion.Euler(0, 0, avatar.attachedRigidbody.rotation);

            rotation = Quaternion.RotateTowards(rotation, Quaternion.Euler(0, 0, avatar.intendedRotation), rotationSpeed);
            velocity = rotation * Vector2.up * velocity.magnitude;

            avatar.attachedRigidbody.velocity = velocity;
            avatar.attachedRigidbody.rotation = rotation.eulerAngles.z;
        }

        public override void ExitState(Avatar avatar) {
            base.ExitState(avatar);

            avatar.glidingParticlesEnabled = false;
        }

        public override bool ShouldTransitionToGliding(Avatar avatar) {
            return false;
        }
        public override bool ShouldTransitionToJumping(Avatar avatar) {
            return false;
        }
        public override bool ShouldTransitionToAirborne(Avatar avatar) {
            return !avatar.intendsGlide;
        }
        public override bool ShouldTransitionToGrounded(Avatar avatar) {
            return false;
        }
    }
}