using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarAnimator : MonoBehaviour {
        [SerializeField, Expandable]
        AvatarController observedAvatar = default;
        [SerializeField, Expandable]
        Animator observedAnimator = default;

        float walkSpeed => Math.Abs(observedAvatar.velocity.x);

        void OnValidate() {
            if (observedAvatar == null) {
                observedAvatar = GetComponentInParent<AvatarController>();
            }
            if (observedAnimator == null) {
                observedAnimator = GetComponentInParent<Animator>();
            }
        }

        void Update() {
            observedAnimator.SetFloat("walkSpeed", walkSpeed);
        }

        public void Play(AvatarAnimations state) {
            observedAnimator.Play(state.ToString(), 0);
        }

        public void InstantiatePrefab(GameObject prefab) {
            Instantiate(prefab, transform);
        }
    }
}