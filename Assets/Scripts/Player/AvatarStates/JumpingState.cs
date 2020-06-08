﻿using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class JumpingState : AvatarState {
        [Header("Jumping")]
        [SerializeField, Range(0, 10)]
        float maximumJumpHeight = 5;
        [SerializeField, Range(0, 100)]
        int minimumJumpFrameCount = 1;
        [SerializeField, Range(0, 100)]
        int maximumJumpFrameCount = 1;
        float jumpStartSpeed {
            get {
                float maximumJumpDuration = maximumJumpFrameCount * Time.fixedDeltaTime;
                float up = maximumJumpHeight;
                float down = Physics2D.gravity.y * avatar.gravityScale * maximumJumpDuration * maximumJumpDuration / 2;
                return (up - down) / maximumJumpDuration;
            }
        }
        Vector2 jumpStartVelocity => new Vector2(avatar.velocity.x, jumpStartSpeed);

        [SerializeField, Range(0, 10), Tooltip("The vertical speed where/when jumping stops")]
        float jumpStopSpeed = 0;
        Vector2 jumpStopVelocity => new Vector2(avatar.velocity.x, jumpStopSpeed);

        int jumpTimer = 0;
        public override void EnterState() {
            base.EnterState();

            avatar.intendsJumpStart = false;
            jumpTimer = 0;

            avatar.AlignFaceToIntend();
            avatar.velocity = jumpStartVelocity;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            jumpTimer++;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }

        public override void ExitState() {
            avatar.velocity = jumpStopVelocity;
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState intendsGlideState = default;
        [SerializeField, Expandable]
        AvatarState rejectsGlideState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsGlide && avatar.canGlide) {
                return intendsGlideState;
            }
            if (jumpTimer < minimumJumpFrameCount) {
                return this;
            }
            if (!avatar.intendsJump) {
                return rejectsGlideState;
            }
            if (avatar.velocity.y < jumpStopSpeed) {
                return rejectsGlideState;
            }
            if (jumpTimer < maximumJumpFrameCount) {
                return this;
            }
            return rejectsGlideState;
        }
    }
}