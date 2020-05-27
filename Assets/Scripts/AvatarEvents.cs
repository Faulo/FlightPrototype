using System;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Events;

public class AvatarEvents : MonoBehaviour {
    [Serializable]
    class AvatarEvent : UnityEvent<Avatar> { }

    [SerializeField, Expandable]
    Avatar observedAvatar = default;

    [SerializeField]
    AvatarEvent onColliderChange = default;

    void OnEnable() {
        observedAvatar.onColliderChange += onColliderChange.Invoke;
    }
    void OnDisable() {
        observedAvatar.onColliderChange -= onColliderChange.Invoke;
    }
}
