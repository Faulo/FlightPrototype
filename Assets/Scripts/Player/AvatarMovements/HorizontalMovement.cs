using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class HorizontalMovement : AvatarMovement {
        [SerializeField]
        bool breakImmediately = true;
        [SerializeField, Range(0, 1)]
        float accelerationDuration = 1;
        [SerializeField]
        bool breakWhenNoInput = true;
        [SerializeField, Range(0, 100)]
        float maximumSpeed = 10;

        public override Func<Vector2> CreateVelocityCalculator(Avatar avatar) {
            float velocityX = avatar.velocity.x;

            return () => {
                float speed = avatar.velocity.x;

                if (breakImmediately) {
                    float minSpeed = avatar.isFacingRight
                        ? 0
                        : -avatar.maximumRunningSpeed;
                    float maxSpeed = avatar.isFacingRight
                        ? avatar.maximumRunningSpeed
                        : 0;
                    speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                }

                float targetSpeed = speed;
                if (avatar.intendedMovement.x == 0) {
                    if (breakWhenNoInput) {
                        targetSpeed = 0;
                    }
                } else {
                    if (maximumSpeed != 0) {
                        targetSpeed = avatar.intendedMovement.x * maximumSpeed;
                    }
                }
                speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocityX, accelerationDuration);


                return new Vector2(speed, avatar.velocity.y);
            };
        }
    }
}