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


        public override Func<Vector2> CreateVelocityCalculator(Avatar avatar) {
            float velocityX = avatar.velocity.x;

            return () => {
                float speed = avatar.velocity.x;

                if (turnAroundImmediately) {
                    float minSpeed = avatar.isFacingRight
                        ? 0
                        : -maximumSpeed;
                    float maxSpeed = avatar.isFacingRight
                        ? maximumSpeed
                        : 0;
                    speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                }

                if (breakWhenNoInput && avatar.intendedMovement == 0) {
                    speed = 0;
                }

                float targetSpeed = avatar.intendedMovement * maximumSpeed;

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
    }
}