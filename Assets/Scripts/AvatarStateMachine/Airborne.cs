using System;
using UnityEngine;

namespace AvatarStateMachine {
    public class Airborne : AvatarStateBehaviour {
        [Header("Airborne")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        public override void EnterState(Avatar avatar) {
            base.EnterState(avatar);

            avatar.attachedRigidbody.rotation = 0;
        }
        public override void FixedUpdateState(Avatar avatar) {
            base.FixedUpdateState(avatar);

            switch (Math.Sign(avatar.intendedMovement.x)) {
                case -1:
                    avatar.isFacingRight = false;
                    break;
                case 1:
                    avatar.isFacingRight = true;
                    break;
            }
            var velocity = avatar.attachedRigidbody.velocity;
            velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, forwardSpeedLerp);
            avatar.attachedRigidbody.velocity = velocity;
            /*
            if (intendsGlide) {
                if (canGlide) {
                    currentGlideCharges--;
                    glidingTimer = glidingDuration;
                    state = MoveState.Gliding;
                    switch (glidingMode) {
                        case GlidingMode.Dash:
                            var direction = intendedMovement.magnitude > 0
                                ? intendedMovement.normalized
                                : Vector2.right * facingSign;
                            velocity += glidingSpeed * direction;
                            rotation = Vector2.SignedAngle(Vector2.up, direction);
                            break;
                        case GlidingMode.Sail:
                            velocity += glidingSailBoost * Vector2.right * velocity.x;
                            rotation = -90 * facingSign;
                            break;
                    }
                    velocity.y += glidingUpdrift;
                    goto case MoveState.Gliding;
                } else {
                    intendsGlide = false;
                }
            }
            velocity.x = Mathf.Lerp(velocity.x, intendedMovement.x * jumpingSpeed, jumpingSpeedLerp);
            if (intendsJump && velocity.y > cutoffSpeed) {
                velocity.y += risingSpeed * Time.deltaTime;
                gravity = jumpingGravity;
                drag = jumpingDrag;
            } else {
                gravity = defaultGravity;
                drag = defaultDrag;
            }
            rotation = 0;
            //*/
        }
        public override void ExitState(Avatar avatar) {
            base.ExitState(avatar);
        }

        public override bool ShouldTransitionToGliding(Avatar avatar) {
            return avatar.intendsGlide && avatar.canGlide;
        }
        public override bool ShouldTransitionToJumping(Avatar avatar) {
            return false;
        }
        public override bool ShouldTransitionToAirborne(Avatar avatar) {
            return false;
        }
        public override bool ShouldTransitionToGrounded(Avatar avatar) {
            return avatar.CalculateGrounded();
        }
    }
}