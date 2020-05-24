using System;
using System.Runtime.Remoting.Messaging;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace AvatarStateMachine {
    public class Airborne : AvatarState {

        [Header("Airborne")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        public override void EnterState() {
            base.EnterState();

            avatar.isAirborne = true;
            avatar.attachedRigidbody.rotation = 0;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

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
        public override void ExitState() {
            avatar.isAirborne = false;
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState groundedState = default;
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.CalculateGrounded()) {
                return groundedState;
            }
            if (avatar.intendsGlide && avatar.canGlide) {
                return glidingState;
            }
            return base.CalculateNextState();
        }
    }
}