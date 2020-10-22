using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonBaker : TilemapColliderBaker {
        [SerializeField, Expandable]
        PolygonCollider2D polygonCollider = default;
        [SerializeField, Range(1, 1000)]
        int shapeCountMaximum = 100;

        TileShape[] shapes;

        protected override void OnValidate() {
            base.OnValidate();
            if (!polygonCollider) {
                polygonCollider = GetComponent<PolygonCollider2D>();
            }
            shapes = new TileShape[shapeCountMaximum];
        }

        protected override void SetupCollider() {
            Assert.IsNotNull(polygonCollider);
        }
        protected override void RegenerateCollider(ISet<Vector3Int> positions) {
            int shapeCount = LevelController.instance.TryGetColliderShapes(positions, ref shapes);
            polygonCollider.pathCount = shapeCount;
            for (int i = 0; i < shapeCount; i++) {
                polygonCollider.SetPath(i, shapes[i].vertices);
            }
        }
    }
}