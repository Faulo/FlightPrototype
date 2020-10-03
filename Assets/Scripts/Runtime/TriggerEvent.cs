using UnityEngine;

namespace TheCursedBroom {
    public class TriggerEvent : MonoBehaviour {
        [SerializeField]
        GameObjectEvent onTriggerEnter = default;
        [SerializeField]
        GameObjectEvent onTriggerStay = default;
        [SerializeField]
        GameObjectEvent onTriggerExit = default;

        void OnTriggerEnter2D(Collider2D collider) {
            onTriggerEnter?.Invoke(collider.gameObject);
        }
        void OnTriggerStay2D(Collider2D collider) {
            onTriggerStay?.Invoke(collider.gameObject);
        }
        void OnTriggerExit2D(Collider2D collider) {
            onTriggerExit?.Invoke(collider.gameObject);
        }
    }
}