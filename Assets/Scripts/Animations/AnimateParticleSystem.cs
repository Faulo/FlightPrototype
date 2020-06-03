using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Animations {
    public class AnimateParticleSystem : StateMachineBehaviour {
        enum ActivationMode {
            OnStateEnter,
            WhileStateUpdates,
            OnStateExit
        }
        [SerializeField, Expandable]
        ParticleSystem prefab = default;
        [SerializeField]
        ActivationMode mode = ActivationMode.WhileStateUpdates;
        ParticleSystem instance;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            switch (mode) {
                case ActivationMode.OnStateEnter:
                case ActivationMode.WhileStateUpdates:
                    instance = Instantiate(prefab, animator.transform);
                    break;
                case ActivationMode.OnStateExit:
                    break;
                default:
                    break;
            }
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            switch (mode) {
                case ActivationMode.OnStateEnter:
                    break;
                case ActivationMode.WhileStateUpdates:
                    instance.Stop();
                    break;
                case ActivationMode.OnStateExit:
                    instance = Instantiate(prefab, animator.transform);
                    break;
                default:
                    break;
            }
        }
    }
}