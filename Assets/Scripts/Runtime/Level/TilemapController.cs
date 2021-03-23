using System;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapController : MonoBehaviour {
        public event Action<TilemapChangeData> onColliderChange;
        public event Action<TilemapChangeData> onRendererChange;
        public event Action<TilemapChangeData> onShadowChange;

        [SerializeField, Expandable]
        public TilemapLayerAsset type = default;
        [SerializeField, Expandable]
        LevelController ownerLevel = default;

        Tilemap tilemap {
            get {
                if (!m_tilemap) {
                    TryGetComponent(out m_tilemap);
                }
                return m_tilemap;
            }
        }
        Tilemap m_tilemap;
        TileComparer tileComparer {
            get {
                if (m_tileComparer == null) {
                    m_tileComparer = type.CreateTileComparer();
                }
                return m_tileComparer;
            }
        }
        TileComparer m_tileComparer;

        TileBase[][] storage;

        readonly TilemapChangeData colliderChange = new TilemapChangeData();
        readonly TilemapChangeData rendererChange = new TilemapChangeData();
        readonly TilemapChangeData shadowChange = new TilemapChangeData();

        void Awake() {
            OnValidate();
            if (ownerLevel) {
                storage = ownerLevel.CreateTilemapStorage(type);
            }
        }
        void OnValidate() {
            if (!ownerLevel) {
                ownerLevel = GetComponentInParent<LevelController>();
            }
        }
        void OnEnable() {
            ownerLevel.colliderBounds.onLoadTiles += LoadColliderTile;
            ownerLevel.colliderBounds.onDiscardTiles += DiscardColliderTile;
            ownerLevel.rendererBounds.onLoadTiles += LoadRendererTile;
            ownerLevel.rendererBounds.onDiscardTiles += DiscardRendererTile;
            ownerLevel.shadowBounds.onLoadTiles += LoadShadowTile;
            ownerLevel.shadowBounds.onDiscardTiles += DiscardShadowTile;
        }
        void OnDisable() {
            ownerLevel.colliderBounds.onLoadTiles -= LoadColliderTile;
            ownerLevel.colliderBounds.onDiscardTiles -= DiscardColliderTile;
            ownerLevel.rendererBounds.onLoadTiles -= LoadRendererTile;
            ownerLevel.rendererBounds.onDiscardTiles -= DiscardRendererTile;
            ownerLevel.shadowBounds.onLoadTiles -= LoadShadowTile;
            ownerLevel.shadowBounds.onDiscardTiles -= DiscardShadowTile;
        }

        public void RegenerateTilemap() {
            if (colliderChange.TryFinish()) {
                onColliderChange?.Invoke(colliderChange);
            }
            if (rendererChange.TryFinish()) {
                onRendererChange?.Invoke(rendererChange);
            }
            if (shadowChange.TryFinish()) {
                onShadowChange?.Invoke(shadowChange);
            }
        }

        void LoadColliderTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                colliderChange.AddLoad(position, tile);
            }
        }
        void DiscardColliderTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                colliderChange.AddDiscard(position, tile);
            }
        }
        void LoadRendererTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                rendererChange.AddLoad(position, tile);
            }
        }
        void DiscardRendererTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                rendererChange.AddDiscard(position, tile);
            }
        }
        void LoadShadowTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                shadowChange.AddLoad(position, tile);
            }
        }
        void DiscardShadowTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                shadowChange.AddDiscard(position, tile);
            }
        }

        public bool IsTile(Vector3Int position, TileBase tile, ITilemap tilemapOverride = null) {
            return TryGetTileFromStorage(position, out var otherTile, tilemapOverride)
                ? tileComparer.IsSynonym(otherTile, tile)
                : false;
        }

        bool TryGetTileFromStorage(Vector3Int position, out TileBase tile, ITilemap tilemapOverride = null) {
            if (storage == null) {
                tile = tilemapOverride == null
                    ? tilemap.GetTile(position)
                    : tilemapOverride.GetTile(position);
            } else {
                if (position.y < 0 || position.y >= storage.Length) {
                    tile = null;
                    return false;
                }
                while (position.x < 0) {
                    position.x += storage[0].Length;
                }
                position.x %= storage[0].Length;
                tile = storage[position.y][position.x];
            }
            return tile != null;
        }
    }
}