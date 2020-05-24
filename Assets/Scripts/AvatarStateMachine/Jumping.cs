using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace AvatarStateMachine {
    public class Jumping : AvatarState {


        [Header("Jumping")]
        [SerializeField, Range(0, 10)]
        float maximumJumpHeight = 5;
        [SerializeField, Range(0.0001f, 1)]
        float minimumJumpDuration = 1;
        [SerializeField, Range(0.0001f, 1)]
        float maximumJumpDuration = 1;
        [SerializeField, Range(0, 10), Tooltip("The vertical speed where/when jumping stops")]
        float jumpStopSpeed = 0;
        Vector2 jumpStartVelocity {
            get {
                var up = Vector2.up * maximumJumpHeight;
                var down = Physics2D.gravity * avatar.attachedRigidbody.gravityScale * maximumJumpDuration * maximumJumpDuration / 2;
                return (up - down) / maximumJumpDuration;
            }
        }
        [Header("Movement")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        [SerializeField, Range(0, 1)]
        float backwardSpeedLerp = 0.1f;

        float jumpTimer = 0;
        public override void EnterState() {
            base.EnterState();

            avatar.isJumping = true;
            jumpTimer = 0;

            avatar.attachedRigidbody.velocity += jumpStartVelocity;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            jumpTimer += Time.deltaTime;

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
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (jumpTimer < minimumJumpDuration) {
                return this;
            }
            if (jumpTimer > maximumJumpDuration) {
                return airborneState;
            }
            if (!avatar.intendsJump) {
                return airborneState;
            }
            if (avatar.attachedRigidbody.velocity.y < jumpStopSpeed) {
                return airborneState;
            }
            return base.CalculateNextState();
        }
    }
}