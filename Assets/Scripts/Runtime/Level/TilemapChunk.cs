using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapChunk : MonoBehaviour {
        [Header("MonoBehaviour configuration")]
        [SerializeField]
        public TilemapContainer tilemaps = default;

        [Header("Editor Tools")]
        [SerializeField]
        bool syncRightWithLeftBorder = false;

        void OnValidate() {
            tilemaps.OnValidate(transform);

            if (syncRightWithLeftBorder) {
                syncRightWithLeftBorder = false;
                SyncBorders();
            }
        }

        void SyncBorders() {
            for (int i = 0; i < tilemaps.height; i++) {
                var left = new Vector3Int(0, i, 0);
                var right = new Vector3Int(tilemaps.width, i, 0);
                foreach (var (_, tilemap) in tilemaps.all) {
                    tilemap.SetTile(right, tilemap.GetTile(left));
                    tilemap.SetTile(left + Vector3Int.left, tilemap.GetTile(right + Vector3Int.left));
                }
            }
        }

        public TileBase GetTile(TilemapType type, Vector3Int position) {
            return tilemaps.GetTilemapByType(type).GetTile(position);
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.white;
            var size = new Vector3(tilemaps.width, tilemaps.height, 0);
            Gizmos.DrawWireCube(tilemaps.worldBottomLeft + (size / 2), size);
        }
    }
}