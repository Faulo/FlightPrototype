using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class JumpingState : AvatarState {
        [Header("Jumping")]
        [SerializeField, Range(0, 10)]
        float maximumJumpHeight = 5;
        [SerializeField, Range(0, 1)]
        float minimumJumpDuration = 0;
        [SerializeField, Range(0, 1)]
        float maximumJumpDuration = 0;
        [SerializeField, Range(0, 2)]
        float jumpBoostMultiplier = 1;
        float jumpStartSpeed {
            get {
                float up = maximumJumpHeight;
                float down = Physics2D.gravity.y * avatar.gravityScale * maximumJumpDuration * maximumJumpDuration / 2;
                return (up - down) / maximumJumpDuration;
            }
        }
        Vector2 jumpStartVelocity => new Vector2(avatar.velocity.x * jumpBoostMultiplier, jumpStartSpeed);

        [SerializeField, Range(-10, 10), Tooltip("The vertical speed where jumping stops")]
        float jumpAbortSpeed = 0;
        Vector2 jumpStopVelocity => new Vector2(avatar.velocity.x, jumpResetSpeed);

        [SerializeField, Range(0, 10)]
        float rejectJumpGravity = 1;
        [SerializeField]
        bool resetVelocityOnExit = false;
        [SerializeField, Range(-10, 10), Tooltip("The vertical speed when jumping stops")]
        float jumpResetSpeed = 0;

        float jumpDuration;
        public override void EnterState() {
            base.EnterState();

            jumpDuration = 0;

            avatar.velocity = jumpStartVelocity;
            avatar.intendsJumpStart = false;

            avatar.broom.isFlying = false;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            jumpDuration += Time.deltaTime;
            if (!avatar.intendsJump) {
                avatar.gravityScale = rejectJumpGravity;
            }

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();

            if (resetVelocityOnExit) {
                avatar.velocity = jumpStopVelocity;
            }
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState rejectsJumpState = default;
        [SerializeField, Expandable]
        AvatarState rejectsGlideState = default;
        [SerializeField, Expandable]
        AvatarState intendsGlideState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsGlide && avatar.broom.canBoost) {
                return intendsGlideState;
            }
            if (jumpDuration < minimumJumpDuration) {
                return this;
            }
            if (!avatar.intendsJump) {
                return rejectsJumpState;
            }
            if (avatar.velocity.y < jumpAbortSpeed) {
                return rejectsGlideState;
            }
            if (jumpDuration < maximumJumpDuration) {
                return this;
            }
            return rejectsGlideState;
        }
    }
}