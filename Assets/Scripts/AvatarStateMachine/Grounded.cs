using System;
using UnityEngine;

namespace AvatarStateMachine {
    public class Grounded : AvatarStateBehaviour {
        [Header("Grounded")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        [SerializeField, Range(0, 1)]
        float backwardSpeedLerp = 0.1f;
        public override void EnterState(Avatar avatar) {
            base.EnterState(avatar);

            avatar.RechargeDashes();
            avatar.attachedRigidbody.rotation = 0;
        }
        public override void FixedUpdateState(Avatar avatar) {
            base.FixedUpdateState(avatar);

            var velocity = avatar.attachedRigidbody.velocity;

            if (Math.Sign(avatar.intendedMovement.x) == avatar.facingSign) {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, forwardSpeedLerp);
            } else {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, backwardSpeedLerp);
            }
            velocity.x = Mathf.Clamp(velocity.x, -avatar.maximumRunningSpeed, avatar.maximumRunningSpeed);

            avatar.attachedRigidbody.velocity = velocity;
            if (avatar.intendedMovement.x != 0 && Math.Sign(avatar.intendedMovement.x) == Math.Sign(velocity.x)) {
                avatar.isFacingRight = avatar.intendedMovement.x > 0;
            }
        }

        public override void ExitState(Avatar avatar) {
            base.ExitState(avatar);
        }

        public override bool ShouldTransitionToGliding(Avatar avatar) {
            return false;
        }
        public override bool ShouldTransitionToJumping(Avatar avatar) {
            return avatar.intendsJump;
        }
        public override bool ShouldTransitionToAirborne(Avatar avatar) {
            return !avatar.CalculateGrounded();
        }

        public override bool ShouldTransitionToGrounded(Avatar avatar) {
            return false;
        }
    }
}