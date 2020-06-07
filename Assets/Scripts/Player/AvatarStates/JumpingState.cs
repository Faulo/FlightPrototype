using Slothsoft.UnityExtensions;
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
                float down = Physics2D.gravity.y * avatar.attachedRigidbody.gravityScale * maximumJumpDuration * maximumJumpDuration / 2;
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

            jumpTimer = 0;
            avatar.currentAnimation = AvatarAnimations.Jumping;

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
            avatar.intendsJump = false;
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
            if (!avatar.intendsJump) {
                return airborneState;
            }
            if (avatar.velocity.y < jumpStopSpeed) {
                return airborneState;
            }
            if (jumpTimer < maximumJumpFrameCount) {
                return this;
            }
            return airborneState;
        }
    }
}