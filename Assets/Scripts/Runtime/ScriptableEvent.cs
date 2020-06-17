using UnityEngine;

namespace TheCursedBroom {
    public class ScriptableEvent : MonoBehaviour {
        [SerializeField]
        GameObject context = default;
        [SerializeField]
        GameObjectEvent onInvoke = default;

        public void Invoke() {
            onInvoke.Invoke(context);
        }
    }
}