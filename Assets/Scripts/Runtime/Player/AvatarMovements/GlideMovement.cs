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
            var gravityRotation = Quaternion.Inverse(Quaternion.LookRotation(Physics2D.gravity, Vector3.forward));
            var angularVelocity = Quaternion.identity;
            return () => {
                var currentRotation = avatar.currentRotation;
                var intendedRotation = avatar.intendedMovementRotation;

                var rotation = QuaternionUtil.SmoothDamp(currentRotation, intendedRotation, ref angularVelocity, Time.deltaTime, rotationSpeed);
                var velocity = avatar.attachedRigidbody.velocity;

                var dragRotation = rotation * gravityRotation;

                Debug.Log(dragRotation.eulerAngles);

                /*


                float speed = velocity.magnitude;

                if (avatar.intendedFacing == avatar.facingSign) {
                    speed += Math.Abs(avatar.intendedLook.x) * glideAcceleration;
                }

                velocity = Vector2.Lerp(velocity, currentRotation * Vector2.right * speed * avatar.facingSign, glideEfficiency);
                velocity += Physics2D.gravity * avatar.gravityScale * Time.deltaTime;

                avatar.attachedRigidbody.velocity = velocity;

                switch (mode) {
                    case GlideMode.RotationControl:
                        if (currentRotation != intendedRotation) {
                            angularVelocity = avatar.intendedLook.magnitude * Math.Sign((currentRotation * Quaternion.Inverse(intendedRotation)).eulerAngles.z - 180);
                        }
                        break;
                    case GlideMode.AngularVelocityControl:
                        angularVelocity = avatar.intendedLook.y * avatar.facingSign;
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
                //*/
                return (velocity, rotation.eulerAngles.z);
            };
        }
    }
}