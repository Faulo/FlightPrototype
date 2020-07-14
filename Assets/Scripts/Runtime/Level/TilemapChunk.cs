using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapChunk : MonoBehaviour {
        [Header("MonoBehaviour configuration")]
        [SerializeField, Expandable]
        public Tilemap background = default;
        [SerializeField, Expandable]
        public Tilemap ground = default;
        [SerializeField, Expandable]
        public Tilemap objects = default;
        [SerializeField, Expandable]
        public Tilemap decorations = default;

        [Header("Level configuration")]
        [SerializeField, Range(1, 1000)]
        int width = 100;
        [SerializeField, Range(1, 1000)]
        int height = 50;

        public Vector2Int size => new Vector2Int(width, height);

        public IEnumerable<Vector3Int> tilePositions {
            get {
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        yield return new Vector3Int(x, y, 0);
                    }
                }
            }
        }

        IEnumerable<Tilemap> tilemaps {
            get {
                if (background) {
                    yield return background;
                }
                if (ground) {
                    yield return ground;
                }
                if (objects) {
                    yield return objects;
                }
                if (decorations) {
                    yield return decorations;
                }
            }
        }

        Vector3 worldBottomLeft => background.GetCellCenterWorld(Vector3Int.zero);

        [Header("Editor Tools")]
        [SerializeField]
        bool syncRightWithLeftBorder = false;

        void OnValidate() {
            if (!background) {
                background = GetComponentsInChildren<Tilemap>()[0];
            }
            if (!ground) {
                ground = GetComponentsInChildren<Tilemap>()[1];
            }
            if (!objects) {
                objects = GetComponentsInChildren<Tilemap>()[2];
            }
            if (!decorations) {
                decorations = GetComponentsInChildren<Tilemap>()[3];
            }
            if (syncRightWithLeftBorder) {
                syncRightWithLeftBorder = false;
                SyncBorders();
            }
        }

        void SyncBorders() {
            for (int i = 0; i < size.y; i++) {
                var left = new Vector3Int(0, i, 0);
                var right = new Vector3Int(size.x, i, 0);
                foreach (var tilemap in tilemaps) {
                    tilemap.SetTile(right, tilemap.GetTile(left));
                    tilemap.SetTile(left + Vector3Int.left, tilemap.GetTile(right + Vector3Int.left));
                }
            }
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.white;
            var size = (Vector3)(Vector2)this.size;
            Gizmos.DrawWireCube(worldBottomLeft + ((size - Vector3.one) / 2), size);
        }
    }
}