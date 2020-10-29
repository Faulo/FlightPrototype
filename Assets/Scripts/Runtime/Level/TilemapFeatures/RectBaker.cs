using System.Collections.Generic;
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

        HashSet<Vector3Int> positions;
        HashSet<TileBase> containedTilesSet;

        protected override void OnValidate() {
            base.OnValidate();
            if (!boxCollider) {
                boxCollider = GetComponent<BoxCollider2D>();
            }
        }
        void OnEnable() {
            positions = new HashSet<Vector3Int>();
            containedTilesSet = new HashSet<TileBase>(containedTiles);

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
            if (TilemapBounds.TryGetBounds(positions, out var offset, out var size)) {
                boxCollider.offset = offset;
                boxCollider.size = size;
            }
        }
    }
}