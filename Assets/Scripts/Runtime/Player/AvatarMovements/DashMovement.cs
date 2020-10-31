using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu(fileName = "AM_Dash_New", menuName = "Avatar Movements/Dash")]
    public class DashMovement : AvatarMovement {
        enum VelocityMode {
            SetVelocity,
            AddVelocity
        }
        enum DirectionMode {
            Intention,
            WallJump
        }
        [Header("Dash")]
        [SerializeField]
        DirectionMode directionSource = default;
        [SerializeField, Tooltip("Round input to directionRange")]
        bool directionsNormalized = true;
        [SerializeField, Range(1, 360)]
        int directionRange = 8;
        [SerializeField, Range(0, 180)]
        int directionBaseAngle = 0;
        [SerializeField, Tooltip("Whether straight up or down is possible")]
        bool allowVerticalDirection = true;
        [SerializeField, Tooltip("Whether straight left or right is possible")]
        bool allowHorizontalDirection = true;
        [SerializeField]
        VelocityMode speedMode = default;
        [SerializeField, Range(-100, 100)]
        float initialSpeed = 15;

        public override MovementCalculator CreateMovementCalculator(AvatarController avatar) {
            var velocity = avatar.velocity;
            var direction = avatar.intendedFlight == Vector2.zero
                ? avatar.forward
                : avatar.intendedFlight;

            int facing = 0;

            switch (directionSource) {
                case DirectionMode.Intention:
                    facing = avatar.intendedFacing;
                    break;
                case DirectionMode.WallJump:
                    facing = -avatar.wallFacing;
                    break;
            }

            avatar.isFacingRight = facing == -1;

            Assert.IsTrue(facing == 1 || facing == -1, $"facing must be -1 or 1, but was: {facing}");

            direction.x = Mathf.Abs(direction.x) * facing;

            float angle = AngleUtil.DirectionalAngle(direction);
            int rotation = Mathf.RoundToInt(angle);

            if (directionsNormalized) {
                rotation += directionBaseAngle;
                rotation = Mathf.RoundToInt((float)rotation * directionRange / 360) * 360 / directionRange;
                rotation -= directionBaseAngle;
                while (rotation < 0) {
                    rotation += 360;
                }
                rotation %= 360;
                if (!allowVerticalDirection) {
                    if (rotation == 90) {
                        rotation -= facing * 360 / directionRange;
                    }
                    if (rotation == 270) {
                        rotation += facing * 360 / directionRange;
                    }
                }
                if (!allowHorizontalDirection) {
                    if (rotation == 0) {
                        rotation += facing * 360 / directionRange * (angle <= 180 ? 1 : -1);
                    }
                    if (rotation == 180) {
                        rotation += facing * 360 / directionRange * (angle <= 180 ? 1 : -1);
                    }
                }
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