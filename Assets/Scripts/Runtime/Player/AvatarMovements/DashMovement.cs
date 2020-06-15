using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class DashMovement : AvatarMovement {
        enum DashDirection {
            IntendedMovement,
            IntendedLook,
            CurrentVelocity
        }
        enum VelocityMode {
            SetVelocity,
            AddVelocity
        }
        [Header("Dash")]
        [SerializeField]
        DashDirection direction = default;
        [SerializeField, Range(1, 360)]
        int dashDirections = 8;
        [SerializeField, Range(0, 1)]
        float directionEfficiency = 1;
        [SerializeField, Range(-180, 180)]
        float rotationOffset = 90;
        [SerializeField]
        VelocityMode speedMode = default;
        [SerializeField, Range(-100, 100)]
        float initialSpeed = 15;

        public override MovementCalculator CreateMovementCalculator(Avatar avatar) {
            var velocity = avatar.velocity;
            float rotation = 0;

            switch (direction) {
                case DashDirection.IntendedMovement:
                    rotation = avatar.intendedMovementRotation.eulerAngles.z;
                    break;
                case DashDirection.IntendedLook:
                    rotation = avatar.intendedLookRotation.eulerAngles.z;
                    break;
                case DashDirection.CurrentVelocity:
                    var input = velocity.magnitude > 0
                        ? velocity
                        : avatar.currentForward;

                    rotation = Vector2.SignedAngle(Vector2.up, input);
                    rotation += rotationOffset * avatar.facingSign;
                    break;
                default:
                    break;
            }
            rotation = Mathf.RoundToInt(rotation * dashDirections / 360) * 360 / dashDirections;

            var dashRotation = Quaternion.Lerp(avatar.currentRotation, Quaternion.Euler(0, 0, rotation), directionEfficiency);

            velocity = dashRotation * Vector2.right * avatar.facingSign * velocity.magnitude;

            var dashVelocity = velocity * initialSpeed / velocity.magnitude;

            switch (speedMode) {
                case VelocityMode.SetVelocity:
                    velocity = dashVelocity;
                    break;
                case VelocityMode.AddVelocity:
                    velocity += dashVelocity;
                    break;
            }

            rotation = Vector2.SignedAngle(Vector2.up, velocity);
            rotation += rotationOffset * avatar.facingSign;

            return () => {
                return (velocity, rotation);
            };
        }
    }
}