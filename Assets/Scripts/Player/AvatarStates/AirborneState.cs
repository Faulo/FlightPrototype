using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class AirborneState : AvatarState {
        public override void EnterState() {
            base.EnterState();

            avatar.currentAnimation = AvatarAnimations.Falling;

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
        AvatarState groundedState = default;
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.isGrounded) {
                return groundedState;
            }
            if (avatar.intendsGlide && avatar.canGlide) {
                return glidingState;
            }
            return this;
        }
    }
}