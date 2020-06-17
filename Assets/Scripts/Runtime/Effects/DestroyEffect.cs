using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "Destroy_New", menuName = "Effects/Destroy Self")]
    public class DestroyEffect : Effect {
        [SerializeField, Range(0, 10)]
        float delay = 0;

        public override void Invoke(GameObject context) {
            Destroy(context, delay);
        }
    }
}
