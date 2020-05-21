using System;
using UnityEngine;

namespace AvatarStateMachine {
    public class Jumping : AvatarStateBehaviour {
        [Header("Jumping")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        [SerializeField, Range(0, 1)]
        float backwardSpeedLerp = 0.1f;

        [SerializeField, Range(0, 100)]
        float jumpInitialSpeed = 10;
        [SerializeField, Range(0, 100)]
        float jumpAdditionalSpeed = 10;
        [SerializeField, Range(-10, 10), Tooltip("The vertical speed where jumping stops")]
        float jumpStopSpeed = 0;
        public override void EnterState(Avatar avatar) {
            base.EnterState(avatar);

            var velocity = avatar.attachedRigidbody.velocity;
            velocity.y = jumpInitialSpeed;
            avatar.attachedRigidbody.velocity = velocity;
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
            velocity.y += jumpAdditionalSpeed * Time.deltaTime;
            avatar.attachedRigidbody.velocity = velocity;
        }

        public override void ExitState(Avatar avatar) {
            base.ExitState(avatar);
        }

        public override bool ShouldTransitionToGliding(Avatar avatar) {
            return false;
        }
        public override bool ShouldTransitionToJumping(Avatar avatar) {
            return false;
        }
        public override bool ShouldTransitionToAirborne(Avatar avatar) {
            return !avatar.intendsJump || avatar.attachedRigidbody.velocity.y < jumpStopSpeed;
        }
        public override bool ShouldTransitionToGrounded(Avatar avatar) {
            return false;
        }
    }
}