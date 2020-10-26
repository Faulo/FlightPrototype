using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public abstract class TilemapFeature : ComponentFeature<TilemapController> {
        enum BoundsType {
            Collider,
            Renderer,
        }
        [SerializeField]
        BoundsType boundsType = BoundsType.Collider;
        [SerializeField]
        TileBase[] containedTiles = new TileBase[0];

        TilemapBounds bounds;
        ISet<Vector3Int> positions;
        bool isDirty;

        void OnEnable() {
            bounds = GetBounds();

            positions = new HashSet<Vector3Int>();
            for (int i = 0; i < containedTiles.Length; i++) {
                bounds.onLoadTile += TileAddedListener;
                bounds.onDiscardTile += TileDiscardedListener;
            }
            observedComponent.onRegenerateCollider += RegenerateColliderListener;

            SetupCollider(bounds);
        }
        void OnDisable() {
            for (int i = 0; i < containedTiles.Length; i++) {
                observedComponent.RemoveTileColliderAddedListener(containedTiles[i], TileAddedListener);
                observedComponent.RemoveTileColliderRemovedListener(containedTiles[i], TileRemovedListener);
            }
            observedComponent.onRegenerateCollider -= RegenerateColliderListener;
            positions = null;
        }

        TilemapBounds GetBounds() {
            switch (boundsType) {
                case BoundsType.Collider:
                    return observedComponent.colliderBounds;
                case BoundsType.Renderer:
                    return observedComponent.rendererBounds;
                default:
                    throw new NotImplementedException(boundsType.ToString());
            }
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
                RegenerateCollider(bounds, positions);
            }
        }

        protected abstract void LoadTile(Vector3Int position, TileBase tile);
        protected abstract void DiscardTile(Vector3Int position, TileBase tile);
        protected abstract void SetupCollider(TilemapBounds bounds);
        protected abstract void RegenerateCollider(TilemapBounds bounds, ISet<Vector3Int> positions);
    }
}