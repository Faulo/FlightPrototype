using Slothsoft.UnityExtensions;
using UnityEngine;

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
        observedAnimator.SetBool("isFlying", observedAvatar.isFlying);
    }
}
