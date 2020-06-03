using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class HangTime : AvatarState {

        [Header("Hang Time")]
        [SerializeField, Range(0, 1)]
        float hangDuration = 1;
        [Header("Movement")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        [SerializeField, Range(0, 1)]
        float backwardSpeedLerp = 0.1f;

        float hangTimer;
        public override void EnterState() {
            base.EnterState();

            hangTimer = 0;
            avatar.isAirborne = true;
            avatar.attachedAnimator.Play(AvatarAnimations.Hanging);
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            hangTimer += Time.deltaTime;

            var velocity = avatar.attachedRigidbody.velocity;
            if (Math.Sign(avatar.intendedMovement.x) == avatar.facingSign) {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, forwardSpeedLerp);
            } else {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, backwardSpeedLerp);
            }
            avatar.attachedRigidbody.velocity = velocity;
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
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.CalculateGrounded()) {
                return groundedState;
            }
            if (avatar.intendsGlide && avatar.canGlide) {
                return glidingState;
            }
            if (hangTimer > hangDuration) {
                return airborneState;
            }
            return this;
        }
    }
}