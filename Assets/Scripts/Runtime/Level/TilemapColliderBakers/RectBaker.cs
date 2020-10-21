using System.Collections.Generic;
using System.Linq;
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
            if (positions.Count > 0) {
                (boxCollider.offset, boxCollider.size) = CalculateBounds(positions);
            }
        }
        (Vector3, Vector3) CalculateBounds(ISet<Vector3Int> positions) {
            var bottomLeft = (Vector3)positions.First();
            var topRight = bottomLeft;
            foreach (var position in positions) {
                if (topRight.x < position.x) {
                    topRight.x = position.x;
                }
                if (topRight.y < position.y) {
                    topRight.y = position.y;
                }
                if (bottomLeft.x > position.x) {
                    bottomLeft.x = position.x;
                }
                if (bottomLeft.y > position.y) {
                    bottomLeft.y = position.y;
                }
            }
            return ((topRight + bottomLeft + Vector3.one) / 2, topRight - bottomLeft + Vector3.one);
        }
    }
}