using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public class AvatarAnimator : MonoBehaviour {
        [SerializeField, Expandable]
        AvatarController observedAvatar = default;
        [SerializeField, Expandable]
        Animator observedAnimator = default;
        [SerializeField, Range(0, 1)]
        float walkSpeedDeadZone = 0;

        float walkSpeed => Math.Abs(observedAvatar.velocity.x) > walkSpeedDeadZone
            ? 1
            : 0;

        void OnValidate() {
            if (observedAvatar == null) {
                observedAvatar = GetComponentInParent<AvatarController>();
            }
            if (observedAnimator == null) {
                observedAnimator = GetComponentInParent<Animator>();
            }
        }

        void Start() {
            events = observedAvatar
                .GetComponentsInChildren<ScriptableEvent>()
                .ToDictionary(script => script.gameObject.name);
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

        public void InvokeEffect(Effect effect) {
            effect.Invoke(observedAvatar.gameObject);
        }
        IDictionary<string, ScriptableEvent> events;
        public void InvokeScriptableEvent(string name) {
            Assert.IsTrue(events.ContainsKey(name));
            events[name].Invoke();
        }
    }
}