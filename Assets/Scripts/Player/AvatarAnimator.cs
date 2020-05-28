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
            observedAnimator.SetBool("isFlying", observedAvatar.isFlying);
        }
    }
}