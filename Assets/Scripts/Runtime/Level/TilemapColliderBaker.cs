using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public abstract class TilemapColliderBaker : ComponentFeature<TilemapController> {
        [SerializeField]
        TileBase[] containedTiles = new TileBase[0];

        TilemapBounds colliderBounds;
        ISet<Vector3Int> positions;
        bool isDirty;

        void OnEnable() {
            positions = new HashSet<Vector3Int>();
            for (int i = 0; i < containedTiles.Length; i++) {
                observedComponent.AddTileColliderAddedListener(containedTiles[i], TileAddedListener);
                observedComponent.AddTileColliderRemovedListener(containedTiles[i], TileRemovedListener);
            }
            observedComponent.onRegenerateCollider += RegenerateColliderListener;
            colliderBounds = LevelController.instance.colliderBounds;

            SetupCollider(colliderBounds);
        }
        void OnDisable() {
            for (int i = 0; i < containedTiles.Length; i++) {
                observedComponent.RemoveTileColliderAddedListener(containedTiles[i], TileAddedListener);
                observedComponent.RemoveTileColliderRemovedListener(containedTiles[i], TileRemovedListener);
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
                RegenerateCollider(colliderBounds, positions);
            }
        }

        protected abstract void SetupCollider(TilemapBounds bounds);
        protected abstract void RegenerateCollider(TilemapBounds bounds, ISet<Vector3Int> positions);
    }
}