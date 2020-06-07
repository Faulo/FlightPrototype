using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class TransitionState : AvatarState {
        [Header("Transitional State")]
        [SerializeField]
        AvatarAnimations animationToPlay = default;
        [SerializeField, Range(0, 100)]
        int animationFrameCount = 1;

        int animationTimer;
        public override void EnterState() {
            base.EnterState();

            animationTimer = 0;
            avatar.currentAnimation = animationToPlay;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            animationTimer++;

            avatar.AlignFaceToIntend();
            avatar.UpdateVelocity();
        }

        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState nextState = default;
        public override AvatarState CalculateNextState() {
            if (animationTimer < animationFrameCount) {
                return this;
            }
            return nextState;
        }
    }
}