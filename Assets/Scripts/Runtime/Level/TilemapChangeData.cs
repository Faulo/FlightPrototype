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

        public int loadCount = 0;
        public int discardCount = 0;

        public int changeCountMaximum {
            get => m_changeCountMaximum;
            set {
                if (m_changeCountMaximum != value) {
                    m_changeCountMaximum = value;
                    Array.Resize(ref loadPositions, value);
                    Array.Resize(ref loadTiles, value);
                    Array.Resize(ref discardPositions, value);
                    Array.Resize(ref discardTiles, value);
                }
            }
        }
        int m_changeCountMaximum;

        public bool hasChanged = false;

        public void Finish() {
            loadCount = m_loadPositions.Count;
            CopyListToArray(m_loadPositions, loadPositions, loadCount);
            CopyListToArray(m_loadTiles, loadTiles, loadCount);

            discardCount = m_discardPositions.Count;
            CopyListToArray(m_discardPositions, discardPositions, discardCount);
            CopyListToArray(m_discardTiles, discardTiles, discardCount);
        }
        void CopyListToArray<T>(in List<T> source, in T[] target, int count) {
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