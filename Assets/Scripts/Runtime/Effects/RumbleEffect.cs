using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "Rumble_New", menuName = "Effects/Rumble")]
    public class RumbleEffect : Effect {
        [SerializeField, Range(0, 1), Tooltip("Speed of the low-frequency (left) motor.")]
        float lowIntensity = 1;
        [SerializeField, Range(0, 1), Tooltip("Speed of the high-frequency (right) motor.")]
        float highIntensity = 1;
        [SerializeField, Range(0, 10)]
        float duration = 1;

        public override void Invoke(GameObject context) {
            var avatar = AvatarController.instance;
            if (!avatar) {
                return;
            }
            if (avatar.rumblingLowIntensity < lowIntensity) {
                avatar.rumblingLowIntensity = lowIntensity;
            }
            if (avatar.rumblingHighIntensity < highIntensity) {
                avatar.rumblingHighIntensity = highIntensity;
            }
            if (avatar.rumblingDuration < duration) {
                avatar.rumblingDuration = duration;
            }
        }
    }
}