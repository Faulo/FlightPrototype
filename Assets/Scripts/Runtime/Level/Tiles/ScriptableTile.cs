using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.Tiles {
    [CreateAssetMenu()]
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
            tileData.flags = TileFlags.LockColor;
            tileData.transform = Matrix4x4.identity;
        }
    }
}
