using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapChangeData {
        public Vector3Int[] loadPositions = new Vector3Int[0];
        public TileBase[] loadTiles = new TileBase[0];
        public Vector3Int[] discardPositions = new Vector3Int[0];
        public TileBase[] discardTiles = new TileBase[0];

        public int loadCount = 0;
        public int discardCount = 0;
        public int changeCountMaximum = 0;

        public bool hasChanged => loadCount > 0 || discardCount > 0;

        public void Finish() {
            Array.Resize(ref loadPositions, loadCount);
            Array.Resize(ref loadTiles, loadCount);
            Array.Resize(ref discardPositions, discardCount);
            Array.Resize(ref discardTiles, discardCount);
        }
        public void Clear() {
            loadCount = discardCount = 0;
            Array.Resize(ref loadPositions, changeCountMaximum);
            Array.Resize(ref loadTiles, changeCountMaximum);
            Array.Resize(ref discardPositions, changeCountMaximum);
            Array.Resize(ref discardTiles, changeCountMaximum);
        }

        public void AddLoad(Vector3Int position, TileBase tile) {
            //Assert.IsFalse(loadPositions.Contains(position), $"Double-tipping load-load position {position}");
            //Assert.IsFalse(discardPositions.Contains(position), $"Double-tipping discard-load position {position}");
            loadPositions[loadCount] = position;
            loadTiles[loadCount] = tile;
            loadCount++;
        }

        public void AddDiscard(Vector3Int position, TileBase tile) {
            //Assert.IsFalse(discardPositions.Contains(position), $"Double-tipping discard-discard position {position}");
            //Assert.IsFalse(loadPositions.Contains(position), $"Double-tipping load-discard position {position}");
            discardPositions[discardCount] = position;
            discardTiles[discardCount] = tile;
            discardCount++;
        }
    }
}