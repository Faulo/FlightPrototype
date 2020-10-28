using System.Collections.Generic;
using System.Linq;
using TheCursedBroom.Lighting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.TilemapFeatures {
    public class TilemapShadowBaker : TilemapFeature {
        [SerializeField, Range(1, 1000)]
        int shapeCountMaximum = 10;
        [SerializeField]
        TileBase[] containedTiles = new TileBase[0];

        TilemapBounds bounds;
        HashSet<Vector3Int> positions;
        TileShape[] shapes;
        PolygonShadowCaster2D[] shadowCasters;

        void OnEnable() {
            bounds = LevelController.instance.shadowBounds;
            positions = new HashSet<Vector3Int>();
            shapes = new TileShape[shapeCountMaximum];
            shadowCasters = Enumerable.Range(0, shapeCountMaximum)
                .Select(i => CreateShadowCaster(i))
                .ToArray();

            observedComponent.onShadowChange += TilemapChangeListener;
        }
        void OnDisable() {
            observedComponent.onShadowChange -= TilemapChangeListener;
        }

        void TilemapChangeListener(TilemapChangeData data) {
            for (int i = 0; i < data.loadPositions.Count; i++) {
                if (containedTiles.Contains(data.loadTiles[i])) {
                    positions.Add(data.loadPositions[i]);
                }
            }
            for (int i = 0; i < data.discardPositions.Count; i++) {
                if (containedTiles.Contains(data.discardTiles[i])) {
                    positions.Remove(data.discardPositions[i]);
                }
            }
            RegenerateCollider();
        }
        PolygonShadowCaster2D CreateShadowCaster(int index) {
            var obj = new GameObject($"PolygonShadowCaster2D_{index}");
            obj.transform.parent = transform;
            var shadowCaster = obj.AddComponent<PolygonShadowCaster2D>();
            shadowCaster.enabled = false;
            shadowCaster.castsShadows = true;
            shadowCaster.selfShadows = true;
            shadowCaster.useRendererSilhouette = false;
            return shadowCaster;
        }
        void RegenerateCollider() {
            int shapeCount = TilemapBounds.TryGetShapes(positions, ref shapes);
            for (int i = 0; i < shapeCount; i++) {
                shadowCasters[i].SetShapePath(shapes[i].vertices);
                shadowCasters[i].enabled = true;
            }
            for (int i = shapeCount; i < shapeCountMaximum; i++) {
                shadowCasters[i].enabled = false;
            }
        }
    }
}