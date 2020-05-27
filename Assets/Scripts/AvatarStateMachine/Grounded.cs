using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace AvatarStateMachine {
    public class Grounded : AvatarState {
        [Header("Grounded")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        [SerializeField, Range(0, 1)]
        float backwardSpeedLerp = 0.1f;
        public override void EnterState() {
            base.EnterState();

            avatar.isGrounded = true;
            avatar.RechargeDashes();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            var velocity = avatar.attachedRigidbody.velocity;

            if (Math.Sign(avatar.intendedMovement.x) == avatar.facingSign) {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, forwardSpeedLerp);
            } else {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, backwardSpeedLerp);
            }
            velocity.x = Mathf.Clamp(velocity.x, -avatar.maximumRunningSpeed, avatar.maximumRunningSpeed);

            avatar.attachedRigidbody.velocity = velocity;
            if (Math.Sign(avatar.intendedMovement.x) == Math.Sign(velocity.x)) {
                avatar.AlignFaceToIntend();
            }
        }

        public override void ExitState() {
            avatar.isGrounded = false;
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState jumpingState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsJump) {
                return jumpingState;
            }
            if (!avatar.CalculateGrounded()) {
                return airborneState;
            }
            return base.CalculateNextState();
        }
    }
}