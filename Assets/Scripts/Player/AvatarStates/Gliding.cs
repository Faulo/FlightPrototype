using System;
using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class Gliding : AvatarState {
        enum GlideMode {
            RotationControl,
            AngularVelocityControl
        }
        [Header("Gliding movement")]
        [SerializeField]
        GlideMode mode = GlideMode.RotationControl;
        [SerializeField, Range(0, 720)]
        float rotationSpeed = 360;
        [SerializeField, Range(0, 1)]
        float rotationLerp = 1;
        [SerializeField, Range(0, 1)]
        float glideEfficiency = 1;

        public override void EnterState() {
            base.EnterState();

            avatar.attachedRigidbody.gravityScale = 0;
            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
            //avatar.attachedSprite.transform.rotation = avatar.transform.rotation * Quaternion.Euler(0, 0, 90 * avatar.facingSign);
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            var velocity = avatar.attachedRigidbody.velocity;
            var currentRotation = avatar.currentRotation;
            var intendedRotation = avatar.intendedRotation;

            velocity = Vector2.Lerp(velocity, currentRotation * Vector2.right * velocity.magnitude * avatar.facingSign, glideEfficiency);
            velocity += Physics2D.gravity * gravity * Time.deltaTime;

            avatar.attachedRigidbody.velocity = velocity;

            float angularVelocity = 0;
            switch (mode) {
                case GlideMode.RotationControl:
                    if (currentRotation != intendedRotation) {
                        angularVelocity = Math.Sign((currentRotation * Quaternion.Inverse(intendedRotation)).eulerAngles.z - 180);
                    }
                    break;
                case GlideMode.AngularVelocityControl:
                    angularVelocity = avatar.intendedMovement.y * avatar.facingSign;
                    break;
            }
            avatar.attachedRigidbody.angularVelocity = Mathf.Lerp(avatar.attachedRigidbody.angularVelocity, rotationSpeed * angularVelocity, rotationLerp);
        }

        public override void ExitState() {
            base.ExitState();

            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            avatar.attachedRigidbody.rotation = 0;
            //avatar.attachedSprite.transform.rotation = avatar.transform.rotation;
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