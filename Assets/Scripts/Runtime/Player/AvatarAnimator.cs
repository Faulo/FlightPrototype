using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public class AvatarAnimator : MonoBehaviour {
        [SerializeField, Expandable]
        AvatarController avatar = default;
        [SerializeField, Expandable]
        Animator attachedAnimator = default;
        [SerializeField, Range(0, 1)]
        float walkSpeedDeadZone = 0;

        float walkSpeed => Math.Abs(avatar.velocity.x) > walkSpeedDeadZone
            ? 1
            : 0;

        void Awake() {
            OnValidate();
        }
        void OnValidate() {
            if (!avatar) {
                avatar = GetComponentInParent<AvatarController>();
            }
            if (!attachedAnimator) {
                attachedAnimator = GetComponentInParent<Animator>();
            }
        }


        void Start() {
            events = avatar
                .GetComponentsInChildren<ScriptableEvent>()
                .ToDictionary(script => script.gameObject.name);
        }

        void Update() {
            attachedAnimator.SetFloat(nameof(walkSpeed), walkSpeed);
        }

        public void Play(AvatarAnimations state) {
            attachedAnimator.Play(state.ToString(), 0);
        }

        public void InstantiatePrefab(GameObject prefab) {
            Instantiate(prefab, transform);
        }

        public void InvokeEffect(Effect effect) {
            effect.Invoke(avatar.gameObject);
        }
        Dictionary<string, ScriptableEvent> events;
        public void InvokeScriptableEvent(string name) {
            Assert.IsTrue(events.ContainsKey(name));
            events[name].Invoke();
        }
    }
}