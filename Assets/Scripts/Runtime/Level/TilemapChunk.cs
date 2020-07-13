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

        Vector3 worldBottomLeft => background.GetCellCenterWorld(Vector3Int.zero);

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
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.white;
            var size = (Vector3)(Vector2)this.size;
            Gizmos.DrawWireCube(worldBottomLeft + size / 2, size + Vector3.one);
        }
    }
}