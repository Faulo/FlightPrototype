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
            avatar.attachedAnimator.Play(AvatarAnimations.Hanging);

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
        AvatarState groundedState = default;
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.CalculateGrounded()) {
                return groundedState;
            }
            if (avatar.intendsGlide && avatar.canGlide) {
                return glidingState;
            }
            if (hangTimer < maximumHangFrameCount) {
                return this;
            }
            return airborneState;
        }
    }
}