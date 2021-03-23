using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapChangeData {
        public Vector3Int[] loadPositions = Array.Empty<Vector3Int>();
        public TileBase[] loadTiles = Array.Empty<TileBase>();
        public Vector3Int[] discardPositions = Array.Empty<Vector3Int>();
        public TileBase[] discardTiles = Array.Empty<TileBase>();

        readonly List<Vector3Int> m_loadPositions = new List<Vector3Int>();
        readonly List<TileBase> m_loadTiles = new List<TileBase>();
        readonly List<Vector3Int> m_discardPositions = new List<Vector3Int>();
        readonly List<TileBase> m_discardTiles = new List<TileBase>();

        public int loadCount => loadPositions.Length;
        public int discardCount => discardPositions.Length;

        public int changeCountMaximum = 0;
        public bool hasChanged = false;

        public void Finish() {
            int loadCount = m_loadPositions.Count;
            CopyListToArray(m_loadPositions, ref loadPositions, loadCount);
            CopyListToArray(m_loadTiles, ref loadTiles, loadCount);
            int discardCount = m_discardPositions.Count;
            CopyListToArray(m_discardPositions, ref discardPositions, discardCount);
            CopyListToArray(m_discardTiles, ref discardTiles, discardCount);
        }
        void CopyListToArray<T>(in List<T> source, ref T[] target, int count) {
            target = new T[count];
            for (int i = 0; i < count; i++) {
                target[i] = source[i];
            }
        }
        public void Clear() {
            m_loadPositions.Clear();
            m_loadTiles.Clear();
            m_discardPositions.Clear();
            m_discardTiles.Clear();
            hasChanged = false;
        }

        public void AddLoad(Vector3Int position, TileBase tile) {
            //Assert.IsFalse(loadPositions.Contains(position), $"Double-dipping load-load position {position}");
            //Assert.IsFalse(discardPositions.Contains(position), $"Double-dipping discard-load position {position}");
            m_loadPositions.Add(position);
            m_loadTiles.Add(tile);
            hasChanged = true;
        }

        public void AddDiscard(Vector3Int position, TileBase tile) {
            //Assert.IsFalse(discardPositions.Contains(position), $"Double-dipping discard-discard position {position}");
            //Assert.IsFalse(loadPositions.Contains(position), $"Double-dipping load-discard position {position}");
            m_discardPositions.Add(position);
            m_discardTiles.Add(tile);
            hasChanged = true;
        }
    }
}