using System;
using UnityEngine;

namespace TheCursedBroom.Player {
    [Serializable]
    public struct RumbleSettings {
        [SerializeField, Range(0, 1), Tooltip("Speed of the low-frequency (left) motor.")]
        public float lowIntensity;
        [SerializeField, Range(0, 1), Tooltip("Speed of the high-frequency (right) motor.")]
        public float highIntensity;
    }
}