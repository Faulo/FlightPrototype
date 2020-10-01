using Slothsoft.UnityExtensions;
using TheCursedBroom.Collisions;
using UnityEngine;

namespace TheCursedBroom.Level {
    public class TilemapController : MonoBehaviour {
        [SerializeField, Expandable]
        public TilemapLayerAsset type = default;
        [SerializeField, Expandable]
        TilemapColliderBaker tilemapColliderBaker = default;
        [SerializeField, Expandable]
        CompositeCollider2D compositeCollider = default;

        void Awake() {
            OnValidate();
        }
        internal void OnValidate() {
            tilemapColliderBaker = GetComponent<TilemapColliderBaker>();
            compositeCollider = GetComponent<CompositeCollider2D>();
        }

        public void RegenerateCollider() {
            if (compositeCollider) {
                compositeCollider.GenerateGeometry();
            }
            if (tilemapColliderBaker) {
                tilemapColliderBaker.BakeCollider();
            }
        }
    }
}