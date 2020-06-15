using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class TransitionState : AvatarState {
        [Header("Transitional State")]
        [SerializeField, Range(0, 100)]
        int transitionFrameCount = 1;

        int transitionTimer;
        public override void EnterState() {
            base.EnterState();

            transitionTimer = 0;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            transitionTimer++;

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState nextState = default;
        public override AvatarState CalculateNextState() {
            if (transitionTimer < transitionFrameCount) {
                return this;
            }
            return nextState;
        }
    }
}