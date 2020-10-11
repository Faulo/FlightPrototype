using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.Tiles {
    [CreateAssetMenu(fileName = "LX_AttachToNeighborTile_New", menuName = "Tiles/Attach To Neighbor Tile", order = 100)]
    public class AttachToNeighborTile : TileBase {
        [Header("Attach To Neighbor Tile")]
        [SerializeField, Expandable]
        TileBase neighborTile = default;
        [SerializeField, Expandable]
        Sprite sprite = default;
        [SerializeField, Expandable]
        GameObject prefab = default;
        [SerializeField]
        TileFlags tileOptions = TileFlags.None;
        [SerializeField]
        bool instantiateSpriteEditorOnly = false;
        [SerializeField]
        Tile.ColliderType tileCollider = Tile.ColliderType.None;

        TilemapCache tilemapCache = new TilemapCache();

        Dictionary<int, Vector3Int> neighborPositions = new Dictionary<int, Vector3Int> {
            [0] = new Vector3Int(0, -1, 0),
            [180] = new Vector3Int(0, 1, 0),
            [90] = new Vector3Int(1, 0, 0),
            [270] = new Vector3Int(-1, 0, 0)
        };

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            if (!instantiateSpriteEditorOnly || !Application.isPlaying) {
                tileData.sprite = sprite;
            }
            tileData.gameObject = prefab;
            tileData.flags = tileOptions;
            tileData.colliderType = tileCollider;
            tileData.transform *= Matrix4x4.Rotate(CalculateRotation(position, tilemap));
        }
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            if (go) {
                go.transform.rotation = CalculateRotation(position, tilemap);
                return true;
            } else {
                return false;
            }
        }

        Quaternion CalculateRotation(Vector3Int position, ITilemap tilemap) {
            var rotation = prefab.transform.rotation;
            foreach (var neighbor in neighborPositions) {
                if (tilemapCache[tilemap].IsTile(position + neighbor.Value, neighborTile, tilemap)) {
                    rotation = Quaternion.Euler(0, 0, neighbor.Key);
                    break;
                }
            }
            return rotation;
        }
    }
}
