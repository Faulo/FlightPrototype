using System;
using UnityEngine;

namespace TheCursedBroom {
    public class PhysicsEventEmitter : MonoBehaviour {
        public event Action<Collision2D> onCollisionExit;
        public event Action<Collision2D> onCollisionEnter;
        public event Action<Collider2D> onTriggerExit;
        public event Action<Collider2D> onTriggerEnter;

        void OnCollisionEnter2D(Collision2D collision) {
            onCollisionEnter?.Invoke(collision);
        }
        void OnCollisionExit2D(Collision2D collision) {
            onCollisionExit?.Invoke(collision);
        }

        void OnTriggerEnter2D(Collider2D collider) {
            onTriggerEnter?.Invoke(collider);
        }

        void OnTriggerExit2D(Collider2D collider) {
            onTriggerExit?.Invoke(collider);
        }
    }
}