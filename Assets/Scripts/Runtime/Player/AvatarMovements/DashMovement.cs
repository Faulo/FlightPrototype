using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class DashMovement : AvatarMovement {
        enum VelocityMode {
            SetVelocity,
            AddVelocity
        }
        [Header("Dash")]
        [SerializeField, Tooltip("Round input to rotationDirections")]
        bool directionsNormalized = true;
        [SerializeField, Range(1, 360)]
        int directionRange = 8;
        [SerializeField, Range(0, 180)]
        int directionBaseAngle = 0;
        [SerializeField]
        VelocityMode speedMode = default;
        [SerializeField, Range(-100, 100)]
        float initialSpeed = 15;

        public override MovementCalculator CreateMovementCalculator(AvatarController avatar) {
            var velocity = avatar.velocity;
            var direction = avatar.intendedFlight == Vector2.zero
                ? avatar.forward
                : avatar.intendedFlight;

            float rotation = AngleUtil.DirectionalAngle(direction);

            if (directionsNormalized) {
                rotation += directionBaseAngle;
                rotation = Mathf.RoundToInt(rotation * directionRange / 360) * 360 / directionRange;
                rotation -= directionBaseAngle;
            }

            var dashRotation = Quaternion.Euler(0, 0, rotation);
            var dashVelocity = (Vector2)(dashRotation * Vector2.right * initialSpeed);

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