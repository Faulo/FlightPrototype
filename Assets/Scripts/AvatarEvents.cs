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
    AvatarEvent onJump = default;
    [SerializeField]
    AvatarEvent onDash = default;

    void OnEnable() {
        observedAvatar.onJump += onJump.Invoke;
        observedAvatar.onDash += onDash.Invoke;
    }
    void OnDisable() {
        observedAvatar.onJump -= onJump.Invoke;
        observedAvatar.onDash -= onDash.Invoke;
    }
}
