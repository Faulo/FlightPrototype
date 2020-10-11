using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.Tiles {
    [CreateAssetMenu(fileName = "LX_PlatformTile_New", menuName = "Tiles/Platform Tile", order = 100)]
    public class PlatformTile : TileBase {
        enum SpriteId {
            TopLeft,
            Top,
            TopRight,
            ColumnTop,
            Left,
            Center,
            Right,
            ColumnCenter,
            BottomLeft,
            Bottom,
            BottomRight,
            ColumnBottom,
            RowLeft,
            RowCenter,
            RowRight,
            Single
        }
        [Flags]
        enum SpritePosition {
            Top = 1 << 0,
            Right = 1 << 1,
            Bottom = 1 << 2,
            Left = 1 << 3
        }
        static readonly Dictionary<SpritePosition, SpriteId> idOverPosition = new Dictionary<SpritePosition, SpriteId> {
            [0] = SpriteId.Single,
            [SpritePosition.Bottom | SpritePosition.Right] = SpriteId.TopLeft,
            [SpritePosition.Bottom | SpritePosition.Left] = SpriteId.TopRight,
            [SpritePosition.Top | SpritePosition.Left] = SpriteId.BottomRight,
            [SpritePosition.Top | SpritePosition.Right] = SpriteId.BottomLeft,
            [SpritePosition.Left] = SpriteId.RowRight,
            [SpritePosition.Left | SpritePosition.Right] = SpriteId.RowCenter,
            [SpritePosition.Right] = SpriteId.RowLeft,
            [SpritePosition.Top] = SpriteId.ColumnBottom,
            [SpritePosition.Top | SpritePosition.Bottom] = SpriteId.ColumnCenter,
            [SpritePosition.Bottom] = SpriteId.ColumnTop,
            [SpritePosition.Top | SpritePosition.Bottom | SpritePosition.Left] = SpriteId.Right,
            [SpritePosition.Top | SpritePosition.Bottom | SpritePosition.Right] = SpriteId.Left,
            [SpritePosition.Top | SpritePosition.Left | SpritePosition.Right] = SpriteId.Bottom,
            [SpritePosition.Bottom | SpritePosition.Left | SpritePosition.Right] = SpriteId.Top,
            [SpritePosition.Top | SpritePosition.Bottom | SpritePosition.Left | SpritePosition.Right] = SpriteId.Center,
        };
        static readonly Dictionary<SpritePosition, Vector3Int> offsetOverPosition = new Dictionary<SpritePosition, Vector3Int> {
            [SpritePosition.Top] = Vector3Int.up,
            [SpritePosition.Left] = Vector3Int.left,
            [SpritePosition.Bottom] = Vector3Int.down,
            [SpritePosition.Right] = Vector3Int.right,
        };

        [SerializeField, Expandable]
        Texture2D spriteSheet = default;
        [SerializeField, HideInInspector]
        Sprite[] sprites = default;
        [SerializeField]
        TileFlags tileOptions = TileFlags.LockAll;
        [SerializeField]
        Tile.ColliderType colliderType = Tile.ColliderType.None;

        TilemapCache tilemapCache = new TilemapCache();

        void OnValidate() {
#if UNITY_EDITOR
            if (spriteSheet) {
                sprites = UnityEditor.AssetDatabase
                    .LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(spriteSheet))
                    .OfType<Sprite>()
                    .ToArray();
            }
#endif
        }
        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            var controller = tilemapCache[tilemap];
            for (int yd = -1; yd <= 1; yd++) {
                for (int xd = -1; xd <= 1; xd++) {
                    var pos = new Vector3Int(position.x + xd, position.y + yd, position.z);
                    if (controller.IsTile(pos, this)) {
                        tilemap.RefreshTile(pos);
                    }
                }
            }
        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            var mask = CalculateSpritePosition(position, tilemapCache[tilemap]);
            tileData.sprite = LookupSprite(idOverPosition[mask]);
            tileData.flags = tileOptions;
            tileData.colliderType = colliderType;
        }
        SpritePosition CalculateSpritePosition(Vector3Int position, TilemapController tilemap) {
            return offsetOverPosition
                    .Where(keyval => tilemap.IsTile(position + keyval.Value, this))
                    .Aggregate((SpritePosition)0, (sum, keyval) => sum | keyval.Key);
        }
        Sprite LookupSprite(SpriteId spriteId) {
            return sprites[(int)spriteId];
        }
    }
}