using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.TilemapFeatures {
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonBaker : TilemapFeature {
        [SerializeField, Expandable]
        PolygonCollider2D polygonCollider = default;
        [SerializeField, Range(1, 1000)]
        int shapeCountMaximum = 100;
        [SerializeField]
        TileBase[] containedTiles = new TileBase[0];

        TilemapBounds bounds;
        HashSet<Vector3Int> positions;
        TileShape[] shapes;


        protected override void OnValidate() {
            base.OnValidate();
            if (!polygonCollider) {
                polygonCollider = GetComponent<PolygonCollider2D>();
            }
        }
        void OnEnable() {
            bounds = LevelController.instance.colliderBounds;
            positions = new HashSet<Vector3Int>();
            shapes = new TileShape[shapeCountMaximum];

            observedComponent.onRendererChange += TilemapChangeListener;
        }
        void OnDisable() {
            observedComponent.onRendererChange -= TilemapChangeListener;
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

        void RegenerateCollider() {
            int shapeCount = TilemapBounds.TryGetShapes(positions, ref shapes);
            polygonCollider.pathCount = shapeCount;
            for (int i = 0; i < shapeCount; i++) {
                polygonCollider.SetPath(i, shapes[i].vertices);
            }
        }
    }
}