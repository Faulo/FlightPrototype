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
        [SerializeField, Range(0, 1)]
        float transitionDuration = 0;
        [SerializeField]
        DissolveMode dissolve = DissolveMode.Disabled;

        float duration;

        public override void EnterState() {
            base.EnterState();

            duration = 0;

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

            duration += Time.deltaTime;

            switch (dissolve) {
                case DissolveMode.Disabled:
                    break;
                case DissolveMode.Disappearing:
                    avatar.dissolveAmount = duration / transitionDuration;
                    break;
                case DissolveMode.Reappearing:
                    avatar.dissolveAmount = 1 - (duration / transitionDuration);
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
            if (duration < transitionDuration) {
                return this;
            }
            return nextState;
        }
    }
}