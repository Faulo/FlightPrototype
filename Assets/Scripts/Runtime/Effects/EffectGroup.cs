using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "EG_New", menuName = "Effects/Effect Group")]
    public class EffectGroup : Effect {
        [SerializeField]
        Effect[] effects = new Effect[0];

        public override void Invoke(GameObject context) {
            for (int i = 0; i < effects.Length; i++) {
                effects[i].Invoke(context);
            }
        }
    }
}