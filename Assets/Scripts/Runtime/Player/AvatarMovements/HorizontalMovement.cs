using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu(fileName = "AM_Horizontal_New", menuName = "Avatar Movements/Horizontal")]
    public class HorizontalMovement : AvatarMovement {
        [SerializeField]
        AnimationCurve intentionFilter = default;
        [SerializeField, Range(0, 100)]
        float maximumSpeed = 10;
        [SerializeField, Range(0, 100)]
        float kinematicCutoffSpeed = 1;
        [SerializeField, Range(0, 10)]
        float accelerationDuration = 1;
        [SerializeField, Range(0, 10)]
        float decelerationDuration = 1;
        [SerializeField]
        bool turnAroundImmediately = true;
        [SerializeField]
        bool breakWhenNoInput = true;
        [SerializeField]
        bool useGroundFriction = true;
        [SerializeField]
        bool allowTurning = true;


        public override MovementCalculator CreateMovementCalculator(AvatarController avatar) {
            float acceleration = avatar.velocity.x;

            return () => {
                var velocity = avatar.velocity;

                if (turnAroundImmediately && avatar.intendedFacing != avatar.facing) {
                    velocity.x = 0;
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

                bool isKinematic = Math.Abs(velocity.x) > kinematicCutoffSpeed;
                if (useGroundFriction && avatar.isGrounded) {
                    duration /= isKinematic
                        ? avatar.groundKinematicFriction
                        : avatar.groundStaticFriction;
                }

                velocity.x = Mathf.SmoothDamp(velocity.x, targetSpeed, ref acceleration, duration);

                velocity += avatar.gravityScale * Physics2D.gravity * Time.deltaTime;

                return (velocity, AngleUtil.HorizontalAngle(allowTurning ? avatar.intendedFacing : avatar.facing));
            };
        }
    }
}