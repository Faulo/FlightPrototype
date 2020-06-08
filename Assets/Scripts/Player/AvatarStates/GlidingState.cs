using System;
using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class GlidingState : AvatarState {
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
        [SerializeField, Range(0, 1)]
        float glideAcceleration = 1;
        [SerializeField]
        bool allowLoopings = false;
        [SerializeField]
        bool useGlideCharges = false;

        public override void EnterState() {
            base.EnterState();

            avatar.attachedRigidbody.gravityScale = 0;
            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            if (useGlideCharges) {
                avatar.UseGlideCharge();
            }

            var velocity = avatar.attachedRigidbody.velocity;
            var currentRotation = avatar.currentRotation;
            var intendedRotation = avatar.intendedRotation;
            float speed = velocity.magnitude;

            if (avatar.intendedFacing == avatar.facingSign) {
                speed += Math.Abs(avatar.intendedFlight.x) * glideAcceleration;
            }

            velocity = Vector2.Lerp(velocity, currentRotation * Vector2.right * speed * avatar.facingSign, glideEfficiency);
            velocity += Physics2D.gravity * gravity * Time.deltaTime;

            avatar.attachedRigidbody.velocity = velocity;

            float angularVelocity = 0;
            switch (mode) {
                case GlideMode.RotationControl:
                    if (currentRotation != intendedRotation) {
                        // TODO: make this work again
                        int rotationOffset = -180;
                        Debug.Log((currentRotation * Quaternion.Inverse(intendedRotation)).eulerAngles.z + rotationOffset);
                        angularVelocity = Math.Sign((currentRotation * Quaternion.Inverse(intendedRotation)).eulerAngles.z + rotationOffset);
                    }
                    break;
                case GlideMode.AngularVelocityControl:
                    angularVelocity = avatar.intendedFlight.y * avatar.facingSign;
                    break;
            }
            avatar.attachedRigidbody.angularVelocity = Mathf.Lerp(avatar.attachedRigidbody.angularVelocity, rotationSpeed * angularVelocity, rotationLerp);
            if (!allowLoopings) {
                float rotation = avatar.attachedRigidbody.rotation;
                while (rotation >= 360) {
                    rotation -= 360;
                }
                avatar.attachedRigidbody.rotation = avatar.isFacingRight
                    ? Mathf.Clamp(rotation, -90, 90)
                    : Mathf.Clamp(rotation, -90, 90);
            }
        }

        public override void ExitState() {
            base.ExitState();

            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            avatar.attachedRigidbody.rotation = 0;
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