using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom {
    public class ScriptableEvent : MonoBehaviour {
        [SerializeField]
        GameObject context = default;
        [SerializeField]
        GameObjectEvent onInvoke = default;

        void Invoke() {
            onInvoke.Invoke(context);
        }
    }
}