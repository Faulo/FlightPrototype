using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class CrouchingState : AvatarState {
        [Header("Crouching")]
        [SerializeField, Range(0, 100)]
        int minimumCrouchFrameCount = 1;
        [SerializeField, Range(0, 100)]
        int startJumpFrameCount = 1;
        [SerializeField, Range(0, 100)]
        int coyoteTimeFrameCount = 1;

        bool intendsJump;
        int crouchDuration;

        public override void EnterState() {
            base.EnterState();

            crouchDuration = 0;
            intendsJump = avatar.intendsJumpStart;

            avatar.broom.isFlying = false;
            avatar.broom.canBoost = true;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            crouchDuration++;
            if (avatar.intendsJumpStart) {
                intendsJump = true;
            }

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState intendsGlideState = default;
        [SerializeField, Expandable]
        AvatarState intendsJumpState = default;
        [SerializeField, Expandable]
        AvatarState notGroundedState = default;
        [SerializeField, Expandable]
        AvatarState rejectsCrouchState = default;
        public override AvatarState CalculateNextState() {
            if (!avatar.isGrounded && avatar.intendsGlide) {
                return intendsGlideState;
            }
            if (crouchDuration >= startJumpFrameCount && intendsJump) {
                return intendsJumpState;
            }
            if (crouchDuration >= coyoteTimeFrameCount && !avatar.isGrounded) {
                return notGroundedState;
            }
            if (crouchDuration >= minimumCrouchFrameCount && !avatar.intendsCrouch) {
                return rejectsCrouchState;
            }
            return this;
        }
    }
}