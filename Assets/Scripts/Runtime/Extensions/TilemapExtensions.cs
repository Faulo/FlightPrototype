using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Extensions {
    public static class TilemapExtensions {
        public static void ClearTile(this Tilemap tilemap, Vector3Int position) {
            tilemap.SetTile(position, null);
        }
        public static IEnumerable<(Vector3Int, TileBase)> GetUsedTiles(this ITilemap tilemap) {
            foreach (var position in tilemap.cellBounds.allPositionsWithin) {
                var tile = tilemap.GetTile(position);
                if (tile) {
                    yield return (position, tile);
                }
            }
        }
        public static IEnumerable<(Vector3Int, TileBase)> GetUsedTiles(this Tilemap tilemap) {
            foreach (var position in tilemap.cellBounds.allPositionsWithin) {
                var tile = tilemap.GetTile(position);
                if (tile) {
                    yield return (position, tile);
                }
            }
        }
        public static IEnumerable<(Vector3Int, TTile)> GetUsedTiles<TTile>(this ITilemap tilemap) where TTile : TileBase {
            foreach (var position in tilemap.cellBounds.allPositionsWithin) {
                var tile = tilemap.GetTile<TTile>(position);
                if (tile) {
                    yield return (position, tile);
                }
            }
        }
        public static IEnumerable<(Vector3Int, TTile)> GetUsedTiles<TTile>(this Tilemap tilemap) where TTile : TileBase {
            foreach (var position in tilemap.cellBounds.allPositionsWithin) {
                var tile = tilemap.GetTile<TTile>(position);
                if (tile) {
                    yield return (position, tile);
                }
            }
        }
        public static Vector3 GetUsedCenter(this ITilemap tilemap) {
            var center = Vector3.zero;
            int i = 0;
            foreach (var (position, tile) in tilemap.GetUsedTiles()) {
                center += position;
                i++;
            }
            return i == 0
                ? center
                : center / i;
        }
        public static Vector3 GetUsedCenter(this Tilemap tilemap) {
            var center = Vector3.zero;
            int i = 0;
            foreach (var (position, tile) in tilemap.GetUsedTiles()) {
                center += position;
                i++;
            }
            return i == 0
                ? center
                : center / i;
        }
    }
}