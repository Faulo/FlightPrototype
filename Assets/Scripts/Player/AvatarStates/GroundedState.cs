using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class GroundedState : AvatarState {
        public override void EnterState() {
            base.EnterState();

            avatar.RechargeGlide();
            avatar.currentAnimation = AvatarAnimations.Idling;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }
        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState crouchingState = default;
        [SerializeField, Expandable]
        AvatarState jumpingState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsJump) {
                return jumpingState;
            }
            if (avatar.intendsCrouch) {
                return crouchingState;
            }
            if (!avatar.isGrounded) {
                return airborneState;
            }
            return this;
        }
    }
}