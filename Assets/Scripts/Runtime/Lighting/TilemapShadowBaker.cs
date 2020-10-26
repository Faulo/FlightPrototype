using System.Collections.Generic;
using System.Linq;
using TheCursedBroom.Level;
using UnityEngine;

namespace TheCursedBroom.Lighting {
    public class TilemapShadowBaker : TilemapColliderBaker {
        [SerializeField, Range(1, 1000)]
        int shapeCountMaximum = 10;

        TileShape[] shapes;
        PolygonShadowCaster2D[] shadowCaster;

        protected override void SetupCollider(TilemapBounds bounds) {
            shapes = new TileShape[shapeCountMaximum];
            shadowCaster = Enumerable.Range(0, shapeCountMaximum)
                .Select(i => CreateShadowCaster(i))
                .ToArray();
        }
        PolygonShadowCaster2D CreateShadowCaster(int index) {
            var obj = new GameObject($"PolygonShadowCaster2D_{index}");
            obj.transform.parent = transform;
            var shadowCaster = obj.AddComponent<PolygonShadowCaster2D>();
            shadowCaster.enabled = false;
            return shadowCaster;
        }
        protected override void RegenerateCollider(TilemapBounds bounds, ISet<Vector3Int> positions) {
            int shapeCount = bounds.TryGetShapes(positions, ref shapes);
            for (int i = 0; i < shapeCount; i++) {
                shadowCaster[i].shapePath = shapes[i].vertices.OfType<Vector3>().ToArray();
            }
        }
    }
}