using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class HangTimeState : AvatarState {
        [Header("Hang Time")]
        [SerializeField, Range(0, 100)]
        int maximumHangFrameCount = 1;

        int hangTimer;
        public override void EnterState() {
            base.EnterState();

            hangTimer = 0;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            hangTimer++;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }
        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState isGroundedState = default;
        [SerializeField, Expandable]
        AvatarState intendsGlideState = default;
        [SerializeField, Expandable]
        AvatarState rejectsGlideState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.isGrounded) {
                return isGroundedState;
            }
            if (avatar.intendsGlide && avatar.canGlide) {
                return intendsGlideState;
            }
            if (hangTimer < maximumHangFrameCount) {
                return this;
            }
            return rejectsGlideState;
        }
    }
}