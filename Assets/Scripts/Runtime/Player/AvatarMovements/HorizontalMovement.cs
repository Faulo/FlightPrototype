using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class HorizontalMovement : AvatarMovement {
        [SerializeField]
        AnimationCurve intentionFilter = default;
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


        public override MovementCalculator CreateMovementCalculator(Avatar avatar) {
            float velocityX = avatar.velocity.x;

            return () => {
                var velocity = avatar.velocity;

                if (turnAroundImmediately) {
                    float minSpeed = avatar.isFacingRight
                        ? 0
                        : -maximumSpeed;
                    float maxSpeed = avatar.isFacingRight
                        ? maximumSpeed
                        : 0;
                    velocity.x = Mathf.Clamp(velocity.x, minSpeed, maxSpeed);
                }

                if (breakWhenNoInput && avatar.intendedMovement == 0) {
                    velocity.x = 0;
                }

                float intention = Math.Sign(avatar.intendedMovement) * intentionFilter.Evaluate(Math.Abs(avatar.intendedMovement));
                float targetSpeed = intention * maximumSpeed;

                bool isAccelerating = Math.Abs(targetSpeed) > Math.Abs(velocity.x);

                float duration = isAccelerating
                    ? accelerationDuration
                    : decelerationDuration;

                if (useGroundFriction && avatar.isGrounded) {
                    duration /= isAccelerating
                        ? avatar.groundStaticFriction
                        : avatar.groundKinematicFriction;
                }

                velocity.x = Mathf.SmoothDamp(velocity.x, targetSpeed, ref velocityX, duration);

                velocity += avatar.gravityScale * Physics2D.gravity * Time.deltaTime;

                return (velocity, 0);
            };
        }
    }
}