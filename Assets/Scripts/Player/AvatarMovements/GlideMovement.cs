using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class GlideMovement : AvatarMovement {
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

        public override MovementCalculator CreateMovementCalculator(Avatar avatar) {
            return () => {
                var velocity = avatar.attachedRigidbody.velocity;
                var currentRotation = avatar.currentRotation;
                var intendedRotation = avatar.intendedRotation;
                float speed = velocity.magnitude;

                if (avatar.intendedFacing == avatar.facingSign) {
                    speed += Math.Abs(avatar.intendedFlight.x) * glideAcceleration;
                }

                velocity = Vector2.Lerp(velocity, currentRotation * Vector2.right * speed * avatar.facingSign, glideEfficiency);
                velocity += Physics2D.gravity * avatar.gravityScale * Time.deltaTime;

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

                float rotation = avatar.attachedRigidbody.rotation;
                if (!allowLoopings) {
                    while (rotation >= 360) {
                        rotation -= 360;
                    }
                    avatar.attachedRigidbody.rotation = avatar.isFacingRight
                        ? Mathf.Clamp(rotation, -90, 90)
                        : Mathf.Clamp(rotation, -90, 90);
                }

                return (velocity, rotation);
            };
        }
    }
}