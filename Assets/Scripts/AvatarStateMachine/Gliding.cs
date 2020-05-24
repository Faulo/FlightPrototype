using Slothsoft.UnityExtensions;
using UnityEngine;


namespace AvatarStateMachine {
    public class Gliding : AvatarState {
        [Header("Gliding movement")]
        [SerializeField, Range(0, 100)]
        float initialSpeed = 1;
        [SerializeField, Range(0, 100)]
        float rotationSpeed = 10;

        float glidingTimer;
        public override void EnterState() {
            base.EnterState();

            avatar.isGliding = true;

            var velocity = avatar.attachedRigidbody.velocity;
            float rotation = avatar.attachedRigidbody.rotation;

            rotation = avatar.intendedRotation;
            velocity = Quaternion.Euler(0, 0, rotation) * Vector2.up * (velocity.magnitude + initialSpeed);

            avatar.attachedRigidbody.velocity = velocity;
            avatar.attachedRigidbody.rotation = rotation;

            avatar.glidingParticlesEnabled = true;
            avatar.UseDashCharge();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            var velocity = avatar.attachedRigidbody.velocity;
            var rotation = Quaternion.Euler(0, 0, avatar.attachedRigidbody.rotation);

            rotation = Quaternion.RotateTowards(rotation, Quaternion.Euler(0, 0, avatar.intendedRotation), rotationSpeed);
            velocity = rotation * Vector2.up * velocity.magnitude;

            avatar.attachedRigidbody.velocity = velocity;
            avatar.attachedRigidbody.rotation = rotation.eulerAngles.z;
        }

        public override void ExitState() {
            base.ExitState();

            avatar.isGliding = false;
            avatar.glidingParticlesEnabled = false;
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (!avatar.intendsGlide) {
                return airborneState;
            }
            return base.CalculateNextState();
        }
    }
}