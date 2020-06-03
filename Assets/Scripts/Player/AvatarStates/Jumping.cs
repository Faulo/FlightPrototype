using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class Jumping : AvatarState {


        [Header("Jumping")]
        [SerializeField, Range(0, 10)]
        float maximumJumpHeight = 5;
        [SerializeField, Range(0, 100)]
        int minimumJumpFrameCount = 1;
        [SerializeField, Range(0, 100)]
        int maximumJumpFrameCount = 1;
        [SerializeField, Range(0, 10), Tooltip("The vertical speed where/when jumping stops")]
        float jumpStopSpeed = 0;
        Vector2 jumpStartVelocity {
            get {
                float maximumJumpDuration = maximumJumpFrameCount * Time.deltaTime;
                var up = Vector2.up * maximumJumpHeight;
                var down = Physics2D.gravity * avatar.attachedRigidbody.gravityScale * maximumJumpDuration * maximumJumpDuration / 2;
                var velocity = (up - down) / maximumJumpDuration;
                velocity.x += avatar.attachedRigidbody.velocity.x;
                return velocity;
            }
        }
        [Header("Movement")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        [SerializeField, Range(0, 1)]
        float backwardSpeedLerp = 0.1f;

        int jumpTimer = 0;
        public override void EnterState() {
            base.EnterState();

            avatar.isJumping = true;
            jumpTimer = 0;

            avatar.attachedRigidbody.velocity = jumpStartVelocity;
            avatar.attachedAnimator.Play(AvatarAnimations.Jumping);
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            jumpTimer++;

            var velocity = avatar.attachedRigidbody.velocity;
            if (Math.Sign(avatar.intendedMovement.x) == avatar.facingSign) {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, forwardSpeedLerp);
            } else {
                velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, backwardSpeedLerp);
            }
            //velocity.y += jumpAdditionalSpeed * Time.deltaTime;
            avatar.attachedRigidbody.velocity = velocity;
        }

        public override void ExitState() {
            avatar.isJumping = false;
            var velocity = avatar.attachedRigidbody.velocity;
            velocity.y = jumpStopSpeed;
            avatar.attachedRigidbody.velocity = velocity;
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (jumpTimer < minimumJumpFrameCount) {
                return this;
            }
            if (avatar.intendsGlide) {
                return glidingState;
            }
            if (jumpTimer > maximumJumpFrameCount) {
                return airborneState;
            }
            if (!avatar.intendsJump) {
                return airborneState;
            }
            if (avatar.attachedRigidbody.velocity.y < jumpStopSpeed) {
                return airborneState;
            }
            return this;
        }
    }
}