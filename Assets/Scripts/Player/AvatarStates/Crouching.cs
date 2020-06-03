using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class Crouching : AvatarState {
        [Header("Crouching")]
        [SerializeField, Range(0, 100)]
        int minimumCrouchFrameCount = 1;

        int crouchDuration;
        public override void EnterState() {
            base.EnterState();

            crouchDuration = 0;
            avatar.attachedAnimator.Play(AvatarAnimations.Crouching);
            avatar.RechargeGlide();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();
            crouchDuration++;
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
            if (avatar.intendsJump) {
                return jumpingState;
            }
            if (!avatar.CalculateGrounded()) {
                return airborneState;
            }
            if (!avatar.intendsCrouch) {
                return idleState;
            }
            return this;
        }
    }
}