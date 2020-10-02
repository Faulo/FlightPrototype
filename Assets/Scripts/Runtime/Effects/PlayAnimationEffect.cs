using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "PlayAnimation_New", menuName = "Effects/PlayAnimation")]
    public class PlayAnimationEffect : Effect {
        [SerializeField]
        string stateName = "";
        [SerializeField, Range(0, 10)]
        int layer = 0;

        public override void Invoke(GameObject context) {
            var animator = context.GetComponentInChildren<Animator>();
            if (!animator) {
                return;
            }
            animator.Play(stateName, layer);
        }
    }
}