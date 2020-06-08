using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class CrouchingState : AvatarState {
        [Header("Crouching")]
        [SerializeField, Range(0, 100)]
        int minimumCrouchFrameCount = 1;

        bool intendsJump;

        int crouchDuration;
        public override void EnterState() {
            base.EnterState();

            crouchDuration = 0;
            avatar.RechargeGlide();
            intendsJump = avatar.intendsJumpStart;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();
            crouchDuration++;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }

        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState intendsJumpState = default;
        [SerializeField, Expandable]
        AvatarState notGroundedState = default;
        [SerializeField, Expandable]
        AvatarState rejectsCrouchState = default;
        public override AvatarState CalculateNextState() {
            if (crouchDuration < minimumCrouchFrameCount) {
                return this;
            }
            if (intendsJump || avatar.intendsJumpStart) {
                return intendsJumpState;
            }
            if (!avatar.isGrounded) {
                return notGroundedState;
            }
            if (!avatar.intendsCrouch) {
                return rejectsCrouchState;
            }
            return this;
        }
    }
}