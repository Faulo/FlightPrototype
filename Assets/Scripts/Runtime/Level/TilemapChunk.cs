using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheCursedBroom.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapChunk : MonoBehaviour {
        [Header("MonoBehaviour configuration")]
        [SerializeField]
        public TilemapContainer tilemaps = default;

        void Awake() {
            OnValidate();
            tilemaps.Install(transform);
        }
        void OnValidate() {
#if UNITY_EDITOR
            EditorTools();
#endif
        }
        public TileBase GetTile(TilemapLayerAsset layer, Vector3Int position) {
            return tilemaps.GetTilemapByLayer(layer).GetTile(position);
        }

#if UNITY_EDITOR
        [Header("Editor Tools")]
        [SerializeField]
        bool installTilemaps = false;
        [SerializeField]
        bool refreshTilemaps = false;
        [SerializeField]
        bool syncRightWithLeftBorder = false;
        [SerializeField]
        bool moveTilesToCorrectLayer = false;

        void EditorTools() {
            if (installTilemaps) {
                installTilemaps = false;
                StartCoroutine(InstallTilemaps());
            }
            if (refreshTilemaps) {
                refreshTilemaps = false;
                StartCoroutine(RefreshTilemaps());
            }
            if (syncRightWithLeftBorder) {
                syncRightWithLeftBorder = false;
                StartCoroutine(SyncBorders());
            }
            if (moveTilesToCorrectLayer) {
                moveTilesToCorrectLayer = false;
                StartCoroutine(MoveTiles());
            }
        }
        IEnumerator InstallTilemaps() {
            yield return null;
            tilemaps.Install(transform);
            Debug.Log("InstallTilemap complete!");
        }
        IEnumerator RefreshTilemaps() {
            yield return null;
            // refresh tilemaps
            foreach (var (_, tilemap) in tilemaps.all) {
                tilemap.RefreshAllTiles();
                tilemap.ClearAllEditorPreviewTiles();
            }
            Debug.Log("RefreshTilemaps complete!");
        }
        IEnumerator SyncBorders() {
            yield return null;
            foreach (var (_, tilemap) in tilemaps.all) {
                tilemap
                    .GetUsedTiles()
                    .Select(tile => tile.Item1)
                    .Where(IsOutOfBounds)
                    .ToList()
                    .ForEach(tilemap.ClearTile);
            }
            for (int i = 0; i < tilemaps.height; i++) {
                var left = new Vector3Int(0, i, 0);
                var right = new Vector3Int(tilemaps.width, i, 0);
                foreach (var (_, tilemap) in tilemaps.all) {
                    tilemap.SetTile(right, tilemap.GetTile(left));
                    tilemap.SetTile(left + Vector3Int.left, tilemap.GetTile(right + Vector3Int.left));
                }
            }
            Debug.Log("SyncBorders complete!");
        }

        IEnumerator MoveTiles() {
            yield return InstallTilemaps();
            var tileMoves = new List<(Tilemap, Tilemap, Vector3Int, TileBase)>();
            // collect tiles to move
            foreach (var (_, oldTilemap) in tilemaps.all) {
                for (int x = 0; x < tilemaps.width; x++) {
                    for (int y = 0; y < tilemaps.height; y++) {
                        var position = new Vector3Int(x, y, 0);
                        var tile = oldTilemap.GetTile(position);
                        if (tile) {
                            var newTilemap = tilemaps.GetTilemapByTile(tile);
                            if (newTilemap != oldTilemap) {
                                tileMoves.Add((oldTilemap, newTilemap, position, tile));
                            }
                        }
                    }
                }
            }

            // delete tiles from old tilemap
            foreach (var (oldTilemap, _, position, _) in tileMoves) {
                oldTilemap.SetTile(position, null);
            }

            // set tile on new tilemap
            foreach (var (_, newTilemap, position, tile) in tileMoves) {
                newTilemap.SetTile(position, tile);
            }

            // refresh tilemaps
            foreach (var (_, tilemap) in tilemaps.all) {
                tilemap.RefreshAllTiles();
            }

            Debug.Log($"MoveTiles complete! {tileMoves.Count} tiles moved.");
        }
        bool IsOutOfBounds(Vector3Int position) {
            return position.x < 0 || position.y < 0 || position.x >= tilemaps.width || position.y >= tilemaps.height;
        }
        void OnDrawGizmos() {
            Gizmos.color = Color.white;
            var size = new Vector3(tilemaps.width, tilemaps.height, 0);
            Gizmos.DrawWireCube(tilemaps.worldBottomLeft + (size / 2), size);
        }
#endif
    }
}