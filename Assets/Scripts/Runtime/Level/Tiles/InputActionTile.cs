using Slothsoft.UnityExtensions;
using TheCursedBroom.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.Tiles {
    [CreateAssetMenu(fileName = "L4_InputAction_New", menuName = "Tiles/InputAction Tile", order = 100)]
    public class InputActionTile : TileBase {
        [SerializeField]
        InputActionReference inputAction = default;
        [SerializeField]
        TileFlags tileOptions = TileFlags.LockAll | TileFlags.InstantiateGameObjectRuntimeOnly;
        [SerializeField]
        Tile.ColliderType colliderType = Tile.ColliderType.None;
        [SerializeField, Expandable]
        GameObject prefab = default;

        Sprite sprite {
            get {
                if (prefab && prefab.TryGetComponent<ControlProvider>(out var provider)) {
                    if (provider.TryLookupSprite(provider.LookupControl(inputAction), out var sprite)) {
                        return sprite;
                    }
                }
                return default;
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = sprite;
            tileData.gameObject = prefab;
            tileData.flags = tileOptions;
            tileData.colliderType = colliderType;
        }
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            if (go) {
                go.GetComponent<ControlProvider>().action = inputAction;
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