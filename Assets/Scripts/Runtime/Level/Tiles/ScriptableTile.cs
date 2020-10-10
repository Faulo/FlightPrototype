using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.Tiles {
    [CreateAssetMenu(fileName = "LX_ScriptableTile_New", menuName = "Tiles/Scriptable Tile", order = 100)]
    public class ScriptableTile : TileBase {
        [SerializeField, Expandable]
        Sprite sprite = default;
        [SerializeField, Expandable]
        GameObject prefab = default;
        [SerializeField]
        Tile.ColliderType colliderType = Tile.ColliderType.Grid;
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = sprite;
            tileData.gameObject = prefab;
            tileData.colliderType = colliderType;
        }
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            if (go) {
                var transform = tilemap.GetTransformMatrix(position);
                go.transform.localRotation = Quaternion.LookRotation(
                    new Vector3(transform.m02, transform.m12, transform.m22),
                    new Vector3(transform.m01, transform.m11, transform.m21)
                );
                go.transform.localScale = transform.lossyScale;
                return true;
            } else {
                return false;
            }
        }
    }
}
