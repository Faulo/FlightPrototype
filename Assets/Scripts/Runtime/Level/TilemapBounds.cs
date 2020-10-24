using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Level {
    [Serializable]
    public class TilemapBounds {
        public event Action<Vector3Int> onLoadTile;
        public event Action<Vector3Int> onDiscardTile;

        [SerializeField, Range(1, 100)]
        int width = 10;
        [SerializeField, Range(1, 100)]
        int height = 10;

        public BoundsInt.PositionEnumerator allPositionsWithin => bounds.allPositionsWithin;
        public bool Contains(Vector3Int position) => bounds.Contains(position);

        Vector3Int center;
        Vector3Int extends;
        BoundsInt bounds;

        readonly HashSet<Vector3Int> loadedTilePositions = new HashSet<Vector3Int>();

        int tilesChangedCount;

        public void PrepareTiles() {
            extends.x = width;
            extends.y = height;
            bounds.size = (2 * extends) + new Vector3Int(0, 0, 1);
        }
        public int UpdateTiles(Vector3Int newCenter) {
            tilesChangedCount = 0;
            if (center != newCenter) {
                center = newCenter;
                bounds.position = center - extends;

                DiscardOldTiles();
                LoadNewTiles();
            }
            return tilesChangedCount;
        }
        void DiscardOldTiles() {
            loadedTilePositions
                .Where(IsOutOfBounds)
                .ToList()
                .ForAll(DiscardTile);
        }
        bool IsOutOfBounds(Vector3Int position) => !bounds.Contains(position);
        void LoadNewTiles() {
            foreach (var position in bounds.allPositionsWithin) {
                if (!loadedTilePositions.Contains(position)) {
                    LoadTile(position);
                }
            }
        }
        void DiscardTile(Vector3Int position) {
            onDiscardTile?.Invoke(position);
            loadedTilePositions.Remove(position);
            tilesChangedCount++;
        }
        void LoadTile(Vector3Int position) {
            onLoadTile?.Invoke(position);
            loadedTilePositions.Add(position);
            tilesChangedCount++;
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public int TryGetShapes(ISet<Vector3Int> positions, ref TileShape[] shapes) {
            int shapeCount = 0;
            bool inBounds(Vector3Int testPosition) {
                return bounds.Contains(testPosition) && positions.Contains(testPosition);
            }
            foreach (var position in bounds.allPositionsWithin) {
                if (positions.Contains(position)) {
                    for (int i = 0; i < shapeCount; i++) {
                        if (shapes[i].ContainsPosition(position)) {
                            goto SKIP;
                        }
                    }
                    shapes[shapeCount++] = CreateTileShape(inBounds, position, Vector3Int.up);
                }
SKIP:
                ;
            }
            return shapeCount;
        }
        public bool TryGetBounds(ISet<Vector3Int> positions, out Vector3 offset, out Vector3 size) {
            var colliderPositions = positions.Where(bounds.Contains).ToList();
            if (colliderPositions.Count == 0) {
                offset = size = Vector3.zero;
                return false;
            }
            var bottomLeft = (Vector3)colliderPositions.First();
            var topRight = bottomLeft;
            foreach (var position in colliderPositions) {
                if (topRight.x < position.x) {
                    topRight.x = position.x;
                }
                if (topRight.y < position.y) {
                    topRight.y = position.y;
                }
                if (bottomLeft.x > position.x) {
                    bottomLeft.x = position.x;
                }
                if (bottomLeft.y > position.y) {
                    bottomLeft.y = position.y;
                }
            }
            offset = (topRight + bottomLeft + Vector3.one) / 2;
            size = topRight - bottomLeft + Vector3.one;
            return true;
        }
        static readonly Dictionary<Vector3Int, Vector2> offsets = new Dictionary<Vector3Int, Vector2> {
            [Vector3Int.right] = new Vector2(0, 1),
            [Vector3Int.down] = new Vector2(1, 1),
            [Vector3Int.left] = new Vector2(1, 0),
            [Vector3Int.up] = new Vector2(0, 0),
        };
        static readonly Dictionary<Vector3Int, Vector3Int> forwardRotation = new Dictionary<Vector3Int, Vector3Int> {
            [Vector3Int.right] = Vector3Int.down,
            [Vector3Int.down] = Vector3Int.left,
            [Vector3Int.left] = Vector3Int.up,
            [Vector3Int.up] = Vector3Int.right,
        };
        static readonly Dictionary<Vector3Int, Vector3Int> backwardRotation = new Dictionary<Vector3Int, Vector3Int> {
            [Vector3Int.right] = Vector3Int.up,
            [Vector3Int.down] = Vector3Int.right,
            [Vector3Int.left] = Vector3Int.down,
            [Vector3Int.up] = Vector3Int.left,
        };
        static TileShape CreateTileShape(Func<Vector3Int, bool> inBounds, Vector3Int startPosition, Vector3Int startDirection) {
            var shape = new TileShape();
            var position = startPosition;
            var direction = startDirection;
            do {
                if (inBounds(position + direction + backwardRotation[direction])) {
                    position += direction + backwardRotation[direction];
                    direction = backwardRotation[direction];
                    shape.AddCorner(position, offsets[direction]);
                } else if (inBounds(position + direction)) {
                    position += direction;
                } else {
                    direction = forwardRotation[direction];
                    shape.AddCorner(position, offsets[direction]);
                }
            } while (!(position == startPosition && direction == startDirection));
            return shape;
        }
    }
}