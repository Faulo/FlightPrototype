using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "CloseCanvas", menuName = "Effects/Close Canvas")]
    public class CloseCanvas : Effect {
        [SerializeField, Range(0, 1)]
        float waitDuration = 0;
        public override void Invoke(GameObject context) {
            var canvas = context.GetComponentInParent<Canvas>();
            if (canvas) {
                Destroy(canvas.gameObject, waitDuration);
            }
        }
    }
}