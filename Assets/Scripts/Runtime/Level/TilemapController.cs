using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapController : MonoBehaviour {
        public event Action<TilemapController> onRegenerateCollider;

        Dictionary<TileBase, PositionDelegate> tileAddedListener = new Dictionary<TileBase, PositionDelegate>();
        Dictionary<TileBase, PositionDelegate> tileRemovedListener = new Dictionary<TileBase, PositionDelegate>();

        [SerializeField, Expandable]
        public TilemapLayerAsset type = default;
        [SerializeField, Expandable]
        LevelController ownerLevel = default;

        Tilemap tilemap {
            get {
                if (!m_tilemap) {
                    m_tilemap = GetComponent<Tilemap>();
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

        IList<Vector3Int> newPositions = new List<Vector3Int>();
        IList<TileBase> newTiles = new List<TileBase>();

        void Update() {
            if (newPositions.Count > 0) {
                tilemap.SetTiles(newPositions.ToArray(), newTiles.ToArray());
                newPositions.Clear();
                newTiles.Clear();
            }
        }

        public void DiscardTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                newPositions.Add(position);
                newTiles.Add(null);
                DiscardTileListener(position, tile);
            }
        }
        public void LoadTile(Vector3Int position) {
            if (TryGetTileFromStorage(position, out var tile)) {
                newPositions.Add(position);
                newTiles.Add(tile);
                LoadTileListener(position, tile);
            }
        }

        public bool IsTile(Vector3Int position, TileBase tile) {
            return TryGetTileFromStorage(position, out var otherTile)
                ? tileComparer.IsSynonym(otherTile, tile)
                : false;
        }

        bool TryGetTileFromStorage(Vector3Int position, out TileBase tile) {
            if (storage == null) {
                tile = tilemap.GetTile(position);
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

        void LoadTileListener(Vector3Int position, TileBase tile) {
            if (tileAddedListener.ContainsKey(tile)) {
                tileAddedListener[tile](position);
            }
        }

        void DiscardTileListener(Vector3Int position, TileBase tile) {
            if (tileRemovedListener.ContainsKey(tile)) {
                tileRemovedListener[tile](position);
            }
        }

        public void RegenerateCollider() {
            onRegenerateCollider?.Invoke(this);
        }

        public void AddTileAddedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsFalse(tileAddedListener.ContainsKey(tile));
            tileAddedListener.Add(tile, callback);
        }
        public void AddTileRemovedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsFalse(tileRemovedListener.ContainsKey(tile));
            tileRemovedListener.Add(tile, callback);
        }

        public void RemoveTileAddedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsTrue(tileAddedListener.ContainsKey(tile));
            Assert.AreEqual(tileAddedListener[tile], callback);
            tileAddedListener.Remove(tile);
        }
        public void RemoveTileRemovedListener(TileBase tile, PositionDelegate callback) {
            Assert.IsTrue(tileRemovedListener.ContainsKey(tile));
            Assert.AreEqual(tileRemovedListener[tile], callback);
            tileRemovedListener.Remove(tile);
        }
    }
}