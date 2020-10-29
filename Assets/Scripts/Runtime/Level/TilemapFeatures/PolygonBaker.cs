using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.TilemapFeatures {
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonBaker : TilemapFeature {
        [SerializeField, Expandable]
        PolygonCollider2D polygonCollider = default;
        [SerializeField, Expandable]
        CompositeCollider2D compositeCollider = default;
        [SerializeField, Range(1, 1000)]
        int shapeCountMaximum = 100;
        [SerializeField]
        TileBase[] containedTiles = new TileBase[0];

        HashSet<Vector3Int> positions;
        HashSet<TileBase> containedTilesSet;
        TileShape[] shapes;


        protected override void OnValidate() {
            base.OnValidate();
            if (!polygonCollider) {
                polygonCollider = GetComponent<PolygonCollider2D>();
            }
            if (!compositeCollider) {
                compositeCollider = GetComponent<CompositeCollider2D>();
            }
        }
        void OnEnable() {
            positions = new HashSet<Vector3Int>();
            containedTilesSet = new HashSet<TileBase>(containedTiles);
            shapes = new TileShape[shapeCountMaximum];

            observedComponent.onColliderChange += TilemapChangeListener;
        }
        void OnDisable() {
            observedComponent.onColliderChange -= TilemapChangeListener;
        }

        void TilemapChangeListener(TilemapChangeData data) {
            for (int i = 0; i < data.loadPositions.Count; i++) {
                if (containedTilesSet.Contains(data.loadTiles[i])) {
                    positions.Add(data.loadPositions[i]);
                }
            }
            for (int i = 0; i < data.discardPositions.Count; i++) {
                if (containedTilesSet.Contains(data.discardTiles[i])) {
                    positions.Remove(data.discardPositions[i]);
                }
            }
            RegenerateCollider();
        }

        void RegenerateCollider() {
            int shapeCount = TilemapBounds.TryGetShapes(positions, ref shapes);
            polygonCollider.pathCount = shapeCount;
            for (int i = 0; i < shapeCount; i++) {
                polygonCollider.SetPath(i, shapes[i].vertices);
            }
            if (compositeCollider) {
                compositeCollider.GenerateGeometry();
            }
        }
    }
}