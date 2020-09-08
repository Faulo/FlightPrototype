using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    [Serializable]
    public class TilemapContainer {
        [Header("Level size")]
        [SerializeField, Range(1, 1000)]
        public int width = 300;
        [SerializeField, Range(1, 1000)]
        public int height = 150;

        [Header("Tilemaps")]
        [SerializeField]
        public TilemapLayerAsset[] tilemapLayers = new TilemapLayerAsset[0];
        [SerializeField]
        public Tilemap[] tilemaps = new Tilemap[0];

        IDictionary<TilemapLayerAsset, Tilemap> layerToTilemap;
        IDictionary<TileBase, int> tileToId;

        public IEnumerable<(TilemapLayerAsset, Tilemap)> all {
            get {
                for (int i = 0; i < tilemapLayers.Length; i++) {
                    yield return (tilemapLayers[i], tilemaps[i]);
                }
            }
        }

        public CompositeCollider2D[] colliders;

        public TilemapLayerAsset GetTilemapLayerByTile(TileBase tile) {
            return tilemapLayers[tileToId[tile]];
        }
        public Tilemap GetTilemapByTile(TileBase tile) {
            return tilemaps[tileToId[tile]];
        }
        public Tilemap GetTilemapByLayer(TilemapLayerAsset layer) {
            return layerToTilemap[layer];
        }
        public void Install(Transform context) {
            tilemapLayers = TilemapLayerAsset.all;
            tilemaps = new Tilemap[tilemapLayers.Length];
            layerToTilemap = new Dictionary<TilemapLayerAsset, Tilemap>();
            tileToId = new Dictionary<TileBase, int>();

            var currentTilemaps = context.GetComponentsInChildren<Tilemap>(true);
            for (int i = 0; i < tilemapLayers.Length; i++) {
                var child = i < currentTilemaps.Length
                    ? currentTilemaps[i].gameObject
                    : new GameObject();
                child.transform.parent = context;
                tilemaps[i] = tilemapLayers[i].InstallTilemap(child);

                layerToTilemap[tilemapLayers[i]] = tilemaps[i];

                foreach (var tile in tilemapLayers[i].allowedTiles) {
                    Assert.IsFalse(tileToId.ContainsKey(tile), $"Tile {tile} can't be on two layers! One of them is: {tilemapLayers[i]}");
                    tileToId[tile] = i;
                }
            }

            colliders = context.GetComponentsInChildren<CompositeCollider2D>();
        }

        public Vector3 tileAnchor => tilemaps[0].tileAnchor;
        public Vector3Int WorldToCell(Vector3 position) => tilemaps[0].WorldToCell(position);
        public Vector3 CellToWorld(Vector3Int position) => tilemaps[0].CellToWorld(position);
        public Vector3 worldBottomLeft => CellToWorld(Vector3Int.zero);
        public IEnumerable<Vector3Int> tilePositions {
            get {
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        yield return new Vector3Int(x, y, 0);
                    }
                }
            }
        }
    }
}