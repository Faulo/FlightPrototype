using TheCursedBroom.Extensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class DashMovement : AvatarMovement {
        enum DashDirection {
            IntendedRotation,
            CurrentVelocity
        }
        enum VelocityMode {
            SetVelocity,
            AddVelocity
        }
        [Header("Dash")]
        [SerializeField]
        DashDirection directionMode = default;
        [SerializeField, Range(1, 360)]
        int dashDirections = 8;
        [SerializeField, Range(0, 1)]
        float directionEfficiency = 1;
        [SerializeField]
        VelocityMode speedMode = default;
        [SerializeField, Range(-100, 100)]
        float initialSpeed = 15;

        public override MovementCalculator CreateMovementCalculator(Avatar avatar) {
            var velocity = avatar.velocity;

            var direction = avatar.currentForward;
            switch (directionMode) {
                case DashDirection.IntendedRotation:
                    if (avatar.intendedFlight != Vector2.zero) {
                        direction = avatar.intendedFlight;
                    }
                    break;
                case DashDirection.CurrentVelocity:
                    if (velocity != Vector2.zero) {
                        direction = velocity.normalized;
                    }
                    break;
                default:
                    break;
            }
            int facing = avatar.intendedFacing;
            float rotation = direction.As2DRotation(facing);
            rotation = Mathf.RoundToInt(rotation * dashDirections / 360) * 360 / dashDirections;

            var currentRotation = Quaternion.Euler(0, 0, avatar.rotation);
            var targetRotation = Quaternion.Euler(0, 0, rotation);
            var dashRotation = Quaternion.Lerp(currentRotation, targetRotation, directionEfficiency);

            Vector2 dashVelocity = dashRotation * Vector2.right * facing * initialSpeed;

            switch (speedMode) {
                case VelocityMode.SetVelocity:
                    velocity = dashVelocity;
                    break;
                case VelocityMode.AddVelocity:
                    velocity += dashVelocity;
                    break;
            }

            return () => {
                return (facing, velocity, rotation);
            };
        }
    }
}