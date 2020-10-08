using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonBaker : TilemapColliderBaker {
        [SerializeField, Expandable]
        PolygonCollider2D polygonCollider = default;

        protected override void OnValidate() {
            base.OnValidate();
            if (!polygonCollider) {
                polygonCollider = GetComponent<PolygonCollider2D>();
            }
        }

        protected override void SetupCollider() {
            Assert.IsNotNull(polygonCollider);
        }
        protected override void RegenerateCollider(ISet<Vector3Int> positions) {
            var shapes = LevelController.instance.GetTileShapes(positions);
            polygonCollider.pathCount = shapes.Count;
            for (int i = 0; i < shapes.Count; i++) {
                polygonCollider.SetPath(i, shapes[i].vertices);
            }
        }
    }
}