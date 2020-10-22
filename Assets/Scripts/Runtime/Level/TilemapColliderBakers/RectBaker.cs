using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    [RequireComponent(typeof(BoxCollider2D))]
    public class RectBaker : TilemapColliderBaker {
        [SerializeField, Expandable]
        BoxCollider2D boxCollider = default;

        protected override void OnValidate() {
            base.OnValidate();
            if (!boxCollider) {
                boxCollider = GetComponent<BoxCollider2D>();
            }
        }

        protected override void SetupCollider() {
            Assert.IsNotNull(boxCollider);
        }
        protected override void RegenerateCollider(ISet<Vector3Int> positions) {
            if (LevelController.instance.TryGetColliderBounds(positions, out var offset, out var size)) {
                boxCollider.offset = offset;
                boxCollider.size = size;
            }
        }
    }
}