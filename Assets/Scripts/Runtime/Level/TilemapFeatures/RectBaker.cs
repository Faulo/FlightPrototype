using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.TilemapFeatures {
    [RequireComponent(typeof(BoxCollider2D))]
    public class RectBaker : TilemapFeature {
        [SerializeField, Expandable]
        BoxCollider2D boxCollider = default;
        [SerializeField]
        TileBase[] containedTiles = new TileBase[0];

        TilemapBounds bounds;
        HashSet<Vector3Int> positions;

        protected override void OnValidate() {
            base.OnValidate();
            if (!boxCollider) {
                boxCollider = GetComponent<BoxCollider2D>();
            }
        }
        void OnEnable() {
            bounds = LevelController.instance.colliderBounds;
            positions = new HashSet<Vector3Int>();

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
            if (bounds.TryGetBounds(positions, out var offset, out var size)) {
                boxCollider.offset = offset;
                boxCollider.size = size;
            }
        }
    }
}