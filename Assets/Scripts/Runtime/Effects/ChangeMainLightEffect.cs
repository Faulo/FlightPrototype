using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "ChangeMainLight_New", menuName = "Effects/ChangeMainLight")]
    public class ChangeMainLightEffect : Effect {
        [SerializeField]
        Color targetColor = Color.white;
        [SerializeField, Range(0, 10)]
        float targetIntensity = 1;
        [SerializeField, Range(0, 60)]
        float changeDuration = 10;

        public override void Invoke(GameObject context) {
            if (!CameraController.instance) {
                return;
            }
            CameraController.instance.ChangeMainLightTo(targetIntensity, targetColor, changeDuration);
        }
    }
}