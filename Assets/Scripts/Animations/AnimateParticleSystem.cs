using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Animations {
    public class AnimateParticleSystem : StateMachineBehaviour {
        [SerializeField, Expandable]
        ParticleSystem prefab = default;
        ParticleSystem instance;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            instance = Instantiate(prefab, animator.transform);
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            instance.Stop();
        }
    }
}