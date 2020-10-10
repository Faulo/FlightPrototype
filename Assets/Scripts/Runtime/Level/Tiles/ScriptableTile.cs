using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.Tiles {
    [CreateAssetMenu(fileName = "LX_ScriptableTile_New", menuName = "Tiles/Scriptable Tile", order = 100)]
    public class ScriptableTile : TileBase {
        [SerializeField, Expandable]
        Texture2D spriteSheet = default;
        [SerializeField, Expandable]
        Sprite sprite = default;
        [SerializeField, HideInInspector]
        Sprite[] sprites = new Sprite[1];
        [SerializeField, Range(0, 100)]
        float perlinScale = 0;
        [SerializeField, Range(0, 100)]
        float perlinOffset = 0;
        [SerializeField, Expandable]
        GameObject prefab = default;
        [SerializeField]
        TileFlags tileOptions = TileFlags.LockAll | TileFlags.InstantiateGameObjectRuntimeOnly;
        [SerializeField]
        bool instantiateSpriteEditorOnly = false;
        [SerializeField]
        Tile.ColliderType colliderType = Tile.ColliderType.Grid;

        Sprite GetSprite(Vector3Int position) {
            int index = 0;
            if (sprites.Length > 1) {
                index = Mathf.Clamp(Mathf.FloorToInt(RuleTile.GetPerlinValue(position, perlinScale, perlinOffset) * sprites.Length), 0, sprites.Length - 1);
            }
            return sprites[index];
        }
        void OnValidate() {
#if UNITY_EDITOR
            if (spriteSheet) {
                sprites = UnityEditor.AssetDatabase
                    .LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(spriteSheet))
                    .OfType<Sprite>()
                    .ToArray();
                sprite = sprites[0];
            } else {
                sprites = new[] { sprite };
            }
#endif
        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            if (!instantiateSpriteEditorOnly || !Application.isPlaying) {
                tileData.sprite = GetSprite(position);
            }
            tileData.gameObject = prefab;
            tileData.flags = tileOptions;
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
