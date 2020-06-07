﻿using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarAnimator : MonoBehaviour {
        [SerializeField, Expandable]
        Avatar observedAvatar = default;
        [SerializeField, Expandable]
        Animator observedAnimator = default;

        void OnValidate() {
            if (observedAvatar == null) {
                observedAvatar = GetComponentInParent<Avatar>();
            }
            if (observedAnimator == null) {
                observedAnimator = GetComponentInParent<Animator>();
            }
        }

        void Update() {
            transform.rotation = observedAvatar.facingRotation;
            observedAnimator.SetFloat("walkSpeed", observedAvatar.walkSpeed);
        }

        public void Play(AvatarAnimations state) {
            observedAnimator.Play(state.ToString(), 0);
        }

        public void InstantiatePrefab(GameObject prefab) {
            Instantiate(prefab, transform);
        }
    }
}