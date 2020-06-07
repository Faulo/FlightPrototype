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
            avatar.currentAnimation = AvatarAnimations.Crouching;
            avatar.RechargeGlide();
            intendsJump = avatar.intendsJump;

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
        AvatarState jumpingState = default;
        [SerializeField, Expandable]
        AvatarState idleState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (crouchDuration < minimumCrouchFrameCount) {
                return this;
            }
            if (intendsJump || avatar.intendsJump) {
                return jumpingState;
            }
            if (!avatar.isGrounded) {
                return airborneState;
            }
            if (!avatar.intendsCrouch) {
                return idleState;
            }
            return this;
        }
    }
}