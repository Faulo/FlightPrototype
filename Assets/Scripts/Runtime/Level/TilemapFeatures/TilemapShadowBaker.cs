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

        HashSet<Vector3Int> positions;
        HashSet<TileBase> containedTilesSet;
        TileShape[] shapes;
        PolygonShadowCaster2D[] shadowCasters;

        void OnEnable() {
            positions = new HashSet<Vector3Int>();
            containedTilesSet = new HashSet<TileBase>(containedTiles);
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
            for (int i = 0; i < data.loadCount; i++) {
                if (containedTilesSet.Contains(data.loadTiles[i])) {
                    positions.Add(data.loadPositions[i]);
                }
            }
            for (int i = 0; i < data.discardCount; i++) {
                if (containedTilesSet.Contains(data.discardTiles[i])) {
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