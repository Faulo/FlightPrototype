using System;
using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapController : MonoBehaviour {
        public event Action<Vector3Int, TileBase> onLoadColliderTile;
        public event Action<Vector3Int, TileBase> onDiscardColliderTile;
        public event Action<Vector3Int, TileBase> onLoadRendererTile;
        public event Action<Vector3Int, TileBase> onDiscardRendererTile;
        public event Action<TilemapController> onRegenerateCollider;

        Dictionary<TileBase,  PositionDelegate> tileColliderAddedListener = new Dictionary<TileBase, PositionDelegate>();
        Dictionary<TileBase, PositionDelegate> tileColliderRemovedListener = new Dictionary<TileBase, PositionDelegate>();

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
            ownerLevel.colliderBounds.onLoadTile += LoadColliderTile;
            ownerLevel.colliderBounds.onDiscardTile += DiscardColliderTile;
            ownerLevel.rendererBounds.onLoadTile += LoadRendererTile;
            ownerLevel.rendererBounds.onDiscardTile += DiscardRendererTile;
        }

        List<Vector3Int> newPositions = new List<Vector3Int>();
        List<TileBase> newTiles = new List<TileBase>();

        void Update() {
            if (newPositions.Count > 0) {
                tilemap.SetTiles(newPositions.ToArray(), newTiles.ToArray());
                newPositions.Clear();
                newTiles.Clear();
            }
        }

        void DiscardRendererTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                onDiscardRendererTile?.Invoke(position, tile);
                newPositions.Add(position);
                newTiles.Add(null);
            }
        }
        void LoadRendererTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                newPositions.Add(position);
                newTiles.Add(tile);
            }
        }
        void DiscardColliderTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile) && tileColliderRemovedListener.TryGetValue(tile, out var listener)) {
                listener(position);
            }
        }
        void LoadColliderTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile) && tileColliderAddedListener.TryGetValue(tile, out var listener)) {
                listener(position);
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

        public void RegenerateCollider() {
            onRegenerateCollider?.Invoke(this);
        }

        public void AddTileColliderAddedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsFalse(tileColliderAddedListener.ContainsKey(tile));
            tileColliderAddedListener.Add(tile, callback);
        }
        public void AddTileColliderRemovedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsFalse(tileColliderRemovedListener.ContainsKey(tile));
            tileColliderRemovedListener.Add(tile, callback);
        }

        public void RemoveTileColliderAddedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsTrue(tileColliderAddedListener.ContainsKey(tile));
            Assert.AreEqual(tileColliderAddedListener[tile], callback);
            tileColliderAddedListener.Remove(tile);
        }
        public void RemoveTileColliderRemovedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsTrue(tileColliderRemovedListener.ContainsKey(tile));
            Assert.AreEqual(tileColliderRemovedListener[tile], callback);
            tileColliderRemovedListener.Remove(tile);
        }
    }
}