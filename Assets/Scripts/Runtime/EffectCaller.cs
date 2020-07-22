using UnityEngine;

namespace TheCursedBroom {
    public class EffectCaller : MonoBehaviour {
        public void CallEffect(Effect effect) {
            effect.Invoke(gameObject);
        }
    }
}