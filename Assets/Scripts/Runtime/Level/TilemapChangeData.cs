using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapChangeData {
        public readonly List<Vector3Int> loadPositions = new List<Vector3Int>();
        public readonly List<TileBase> loadTiles = new List<TileBase>();
        public readonly List<Vector3Int> discardPositions = new List<Vector3Int>();
        public readonly List<TileBase> discardTiles = new List<TileBase>();

        public bool hasChanged => loadPositions.Count > 0;

        public void Clear() {
            loadPositions.Clear();
            loadTiles.Clear();
            discardPositions.Clear();
            discardTiles.Clear();
        }

        public void AddLoad(Vector3Int position, TileBase tile) {
            loadPositions.Add(position);
            loadTiles.Add(tile);
        }

        public void AddDiscard(Vector3Int position, TileBase tile) {
            discardPositions.Add(position);
            discardTiles.Add(tile);
        }
    }
}