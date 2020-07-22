using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "ChangeBackgroundColor_New", menuName = "Effects/ChangeBackgroundColor")]
    public class ChangeBackgroundColorEffect : Effect {
        [SerializeField]
        Color targetColor = Color.white;
        [SerializeField, Range(0, 60)]
        float changeDuration = 10;

        public override void Invoke(GameObject context) {
            if (!CameraController.instance) {
                return;
            }
            CameraController.instance.ChangeBackgroundColorTo(targetColor, changeDuration);
        }
    }
}