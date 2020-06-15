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
        [SerializeField, Tooltip("Round input to rotationDirections")]
        bool directionsNormalized = true;
        [SerializeField, Range(1, 360)]
        int directionRange = 8;
        [SerializeField]
        bool instantRotation = true;
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
            float rotation = AngleUtil.DirectionalAngle(direction);

            if (directionsNormalized) {
                rotation = Mathf.RoundToInt(rotation * directionRange / 360) * 360 / directionRange;
            }

            var dashRotation = Quaternion.Euler(0, 0, rotation);
            if (!instantRotation) {
                var currentRotation = Quaternion.Euler(0, 0, avatar.rotation);
                dashRotation = Quaternion.Lerp(currentRotation, dashRotation, directionEfficiency);
            }

            Vector2 dashVelocity = dashRotation * Vector2.right * initialSpeed;

            switch (speedMode) {
                case VelocityMode.SetVelocity:
                    velocity = dashVelocity;
                    break;
                case VelocityMode.AddVelocity:
                    velocity += dashVelocity;
                    break;
            }

            return () => {
                return (velocity, rotation);
            };
        }
    }
}