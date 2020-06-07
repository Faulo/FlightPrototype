using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarAnimator : MonoBehaviour {
        [SerializeField, Expandable]
        Avatar observedAvatar = default;
        [SerializeField, Expandable]
        Animator observedAnimator = default;
        protected virtual void OnValidate() {
            if (observedAvatar == null) {
                observedAvatar = GetComponentInParent<Avatar>();
            }
            if (observedAnimator == null) {
                observedAnimator = GetComponentInParent<Animator>();
            }
        }

        void Update() {
            transform.rotation = observedAvatar.facingRotation;
        }

        public void Play(AvatarAnimations state) {
            observedAnimator.speed = 1;
            observedAnimator.Play(state.ToString(), 0);
        }
    }
}