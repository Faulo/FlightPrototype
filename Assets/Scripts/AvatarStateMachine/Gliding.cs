using System;
using Slothsoft.UnityExtensions;
using UnityEngine;


namespace AvatarStateMachine {
    public class Gliding : AvatarState {
        [Header("Gliding movement")]
        [SerializeField, Range(0, 720)]
        float rotationSpeed = 360;
        [SerializeField, Range(0, 1)]
        float rotationLerp = 1;

        [Header("Sub-components")]
        [SerializeField, Expandable]
        ParticleSystem particles = default;
        bool particlesEnabled {
            set {
                if (value) {
                    particles.Play();
                } else {
                    particles.Stop();
                }
            }
        }

        public override void EnterState() {
            base.EnterState();

            avatar.isGliding = true;
            particlesEnabled = true;

            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            var velocity = avatar.attachedRigidbody.velocity;
            var currentRotation = avatar.currentRotation;
            var intendedRotation = avatar.intendedRotation;

            velocity = currentRotation * Vector2.up * velocity.magnitude;

            avatar.attachedRigidbody.velocity = velocity;

            float angularVelocity = 0;
            if (currentRotation != intendedRotation) {
                angularVelocity = Math.Sign((currentRotation * Quaternion.Inverse(intendedRotation)).eulerAngles.z - 180);
            }
            avatar.attachedRigidbody.angularVelocity = Mathf.Lerp(avatar.attachedRigidbody.angularVelocity, rotationSpeed * angularVelocity, rotationLerp);
        }

        public override void ExitState() {
            base.ExitState();

            avatar.isGliding = false;
            particlesEnabled = false;

            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            avatar.attachedRigidbody.rotation = 0;
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