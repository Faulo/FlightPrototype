using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheCursedBroom.Player.AvatarStates {
    public class TransitionState : AvatarState {
        enum DissolveMode {
            Disabled,
            Disappearing,
            Reappearing,
        }
        enum ActionMode {
            Transition,
            Save,
            Load,
        }
        [Header("Transitional State")]
        [SerializeField, FormerlySerializedAs("onStateSuccess")]
        GameObjectEvent onTransitionSuccess = new GameObjectEvent();
        [SerializeField]
        GameObjectEvent onTransitionAbort = new GameObjectEvent();
        [SerializeField, Range(0, 1)]
        float transitionDuration = 0;
        [SerializeField]
        DissolveMode dissolve = DissolveMode.Disabled;
        [SerializeField]
        ActionMode action = ActionMode.Transition;

        float duration;
        bool isAborted;

        public override void EnterState() {
            base.EnterState();

            duration = 0;
            isAborted = false;
            UpdateAborted();

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
            UpdateAborted();

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

            if (isAborted) {
                onTransitionAbort.Invoke(avatar.gameObject);
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
            } else {
                onTransitionSuccess.Invoke(avatar.gameObject);
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
        }

        void UpdateAborted() {
            switch (action) {
                case ActionMode.Transition:
                    break;
                case ActionMode.Save:
                    if (!avatar.intendsSave) {
                        isAborted = true;
                    }
                    break;
                case ActionMode.Load:
                    if (!avatar.intendsLoad) {
                        isAborted = true;
                    }
                    break;
                default:
                    break;
            }
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState nextState = default;
        [SerializeField, Expandable]
        AvatarState abortState = default;
        public override AvatarState CalculateNextState() {
            if (isAborted) {
                return abortState;
            }
            if (duration < transitionDuration) {
                return this;
            }
            return nextState;
        }
    }
}