using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public abstract class TilemapColliderBaker : ComponentFeature<TilemapController> {
        [SerializeField]
        TileBase[] containedTiles = new TileBase[0];

        ISet<Vector3Int> positions;
        bool isDirty;

        void OnEnable() {
            positions = new HashSet<Vector3Int>();
            for (int i = 0; i < containedTiles.Length; i++) {
                observedComponent.AddTileAddedListener(containedTiles[i], TileAddedListener);
                observedComponent.AddTileRemovedListener(containedTiles[i], TileRemovedListener);
            }
            observedComponent.onRegenerateCollider += RegenerateColliderListener;

            SetupCollider();
        }
        void OnDisable() {
            for (int i = 0; i < containedTiles.Length; i++) {
                observedComponent.RemoveTileAddedListener(containedTiles[i], TileAddedListener);
                observedComponent.RemoveTileRemovedListener(containedTiles[i], TileRemovedListener);
            }
            observedComponent.onRegenerateCollider -= RegenerateColliderListener;
            positions = null;
        }

        void TileAddedListener(Vector3Int position) {
            positions.Add(position);
            isDirty = true;
        }
        void TileRemovedListener(Vector3Int position) {
            positions.Remove(position);
            isDirty = true;
        }
        void RegenerateColliderListener(TilemapController tilemap) {
            if (isDirty) {
                isDirty = false;
                RegenerateCollider(positions);
            }
        }

        protected abstract void SetupCollider();
        protected abstract void RegenerateCollider(ISet<Vector3Int> positions);
    }
}