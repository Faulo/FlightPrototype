using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class AirborneState : AvatarState {
        public override void EnterState() {
            base.EnterState();

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
        AvatarState intendsGlideState = default;
        [SerializeField, Expandable]
        AvatarState isGroundedState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsGlide && avatar.canGlide) {
                return intendsGlideState;
            }
            if (avatar.isGrounded) {
                return isGroundedState;
            }
            return this;
        }
    }
}