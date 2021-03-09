using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class TransitionState : AvatarState {
        enum DissolveMode {
            Disabled,
            Disappearing,
            Reappearing,
        }
        [Header("Transitional State")]
        [SerializeField, Range(0, 100)]
        int transitionFrameCount = 1;
        [SerializeField]
        DissolveMode dissolve = DissolveMode.Disabled;

        int transitionTimer;

        public override void EnterState() {
            base.EnterState();

            transitionTimer = 0;

            switch (dissolve) {
                case DissolveMode.Disabled:
                    break;
                case DissolveMode.Disappearing:
                    avatar.dissolveAmount = 0;
                    break;
                case DissolveMode.Reappearing:
                    avatar.dissolveAmount = 1;
                    break;
                default:
                    break;
            }

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            transitionTimer++;

            switch (dissolve) {
                case DissolveMode.Disabled:
                    break;
                case DissolveMode.Disappearing:
                    avatar.dissolveAmount = (float)transitionTimer / transitionFrameCount;
                    break;
                case DissolveMode.Reappearing:
                    avatar.dissolveAmount = 1 - ((float)transitionTimer / transitionFrameCount);
                    break;
                default:
                    break;
            }

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();

            switch (dissolve) {
                case DissolveMode.Disabled:
                    break;
                case DissolveMode.Disappearing:
                    avatar.dissolveAmount = 1;
                    break;
                case DissolveMode.Reappearing:
                    avatar.dissolveAmount = 0;
                    break;
                default:
                    break;
            }
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