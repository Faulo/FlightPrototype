using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class HorizontalMovement : AvatarMovement {
        [SerializeField, Range(0, 100)]
        float maximumSpeed = 10;
        [SerializeField, Range(0, 1)]
        float accelerationDuration = 1;
        [SerializeField, Range(0, 1)]
        float decelerationDuration = 1;
        [SerializeField]
        bool turnAroundImmediately = true;
        [SerializeField]
        bool breakWhenNoInput = true;
        [SerializeField]
        bool useGroundFriction = true;
        [Header("Input")]
        [SerializeField, Range(0, 1)]
        float inputDeadZone = 0;
        [SerializeField, Tooltip("Normalize to input range [0, 1] instead of [deadzone, 1]")]
        bool normalizeInput = true;
        [SerializeField, Range(1, 360), Tooltip("How many different input values are possible")]
        int inputRange = 360;


        public override Func<Vector2> CreateVelocityCalculator(Avatar avatar) {
            float velocityX = avatar.velocity.x;

            return () => {
                float speed = avatar.velocity.x;

                var input = ProcessInput(avatar.intendedMovement);

                if (turnAroundImmediately) {
                    float minSpeed = avatar.isFacingRight
                        ? 0
                        : -maximumSpeed;
                    float maxSpeed = avatar.isFacingRight
                        ? maximumSpeed
                        : 0;
                    speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                }

                if (breakWhenNoInput && input.x == 0) {
                    speed = 0;
                }

                float targetSpeed = input.x * maximumSpeed;

                bool isAccelerating = Mathf.Abs(targetSpeed) > Mathf.Abs(speed);

                float duration = isAccelerating
                    ? accelerationDuration
                    : decelerationDuration;

                if (useGroundFriction && avatar.isGrounded) {
                    duration /= isAccelerating
                        ? avatar.groundStaticFriction
                        : avatar.groundKinematicFriction;
                }

                speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocityX, duration);

                return new Vector2(speed, avatar.velocity.y);
            };
        }

        Vector2 ProcessInput(Vector2 intention) {
            if (Mathf.Abs(intention.x) <= inputDeadZone) {
                intention.x = 0;
            }

            if (normalizeInput) {
                intention.x = (Mathf.Abs(intention.x) - inputDeadZone) * Math.Sign(intention.x) / (1 - inputDeadZone);
            }

            intention.x = intention.x > 0
                ? Mathf.Ceil(intention.x * inputRange) / inputRange
                : Mathf.Floor(intention.x * inputRange) / inputRange;

            return intention;
        }
    }
}