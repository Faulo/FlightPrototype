using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Level {
    public class TilemapController : MonoBehaviour {
        public event Action<TilemapController> onRegenerateCollider;
        public event Action<GameObject> onTriggerEnter;
        public event Action<GameObject> onTriggerExit;

        [SerializeField, Expandable]
        public TilemapLayerAsset type = default;

        public void RegenerateCollider() {
            onRegenerateCollider?.Invoke(this);
        }

        void OnTriggerEnter2D(Collider2D collider) {
            onTriggerEnter?.Invoke(collider.gameObject);
        }
        void OnTriggerExit2D(Collider2D collider) {
            onTriggerExit?.Invoke(collider.gameObject);
        }
    }
}