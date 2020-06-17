using UnityEngine;

namespace TheCursedBroom {
    public abstract class Effect : ScriptableObject {
        public abstract void Invoke(GameObject context);
    }
}