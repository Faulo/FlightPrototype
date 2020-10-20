﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class LevelController : MonoBehaviour {
        public static LevelController instance;

        [Header("MonoBehaviour configuration")]
        [SerializeField]
        TilemapContainer map = default;
        readonly ISet<Vector3Int> loadedTilePositions = new HashSet<Vector3Int>();

        [Header("Levels")]
        [SerializeField, Expandable]
        TilemapChunk[] levels = new TilemapChunk[0];

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onStart = default;

        public Transform observedActor;
        public ISet<ILevelObject> observedObjects = new HashSet<ILevelObject>();
        Vector3Int observedCenter = Vector3Int.zero;
        Vector3Int lastCenter = Vector3Int.zero;

        [Header("Chunk loading")]
        [SerializeField, Tooltip("Whether or not to respect the ILevelObject::requireLevel property")]
        bool allowNonActorTileLoading = false;
        [SerializeField, Range(1, 100)]
        int colliderRangeX = 16;
        [SerializeField, Range(1, 100)]
        int colliderRangeY = 9;
        [SerializeField, Range(1, 100)]
        int minimumRangeX = 48;
        [SerializeField, Range(1, 100)]
        int minimumRangeY = 27;
        [SerializeField, Range(1, 100)]
        int maximumRangeX = 64;
        [SerializeField, Range(1, 100)]
        int maximumRangeY = 36;

        int tilesChangedCount;
        [SerializeField, Range(-1, 1000)]
        int tilesChangedMaximum = -1;

        int currentColliderIndex = 0;
        [SerializeField, Range(1, 80)]
        int pauseBetweenColliderUpdates = 1;
        [SerializeField, Range(1, 100)]
        int shapeCountMaximum = 1;

        [Header("Editor Tools")]
        [SerializeField]
        bool installTilemap = false;

        void Awake() {
            instance = this;
            PrepareTiles();
        }
        void Start() {
            onStart.Invoke(gameObject);
        }
        void Update() {
            if (currentColliderIndex < map.tilemapControllers.Length * pauseBetweenColliderUpdates) {
                if (currentColliderIndex % pauseBetweenColliderUpdates == 0) {
                    map.tilemapControllers[currentColliderIndex / pauseBetweenColliderUpdates].RegenerateCollider();
                }
                currentColliderIndex++;
            } else {
                UpdateTiles();
            }
        }
        public void RefreshAllTiles() {
            UpdateTiles();
            for (int i = 0; i < map.tilemapControllers.Length; i++) {
                map.tilemapControllers[i].RegenerateCollider();
            }
        }
        public TileBase[][] CreateTilemapStorage(TilemapLayerAsset type) {
            var storage = new TileBase[map.height * levels.Length][];
            foreach (var (layer, tilemap) in map.all) {
                if (layer == type) {
                    for (int i = 0; i < levels.Length; i++) {
                        for (int y = 0; y < map.height; y++) {
                            int j = y + (i * map.height);
                            storage[j] = new TileBase[map.width];
                            for (int x = 0; x < map.width; x++) {
                                storage[j][x] = levels[i].GetTile(layer, new Vector3Int(x, y, 0));
                            }
                        }
                    }
                }
            }
            return storage;
        }
        void PrepareTiles() {
            map.Install(transform);
            for (int i = 0; i < levels.Length; i++) {
                levels[i].tilemaps.Install(levels[i].transform);
            }
        }
        void UpdateTiles() {
            if (!observedActor) {
                return;
            }

            tilesChangedCount = 0;

            foreach (var observedObject in observedObjects) {
                while (observedObject.position.x < observedActor.position.x - (map.width / 2)) {
                    observedObject.TranslateX(map.width);
                }
                while (observedObject.position.x > observedActor.position.x + (map.width / 2)) {
                    observedObject.TranslateX(-map.width);
                }
            }
            if (allowNonActorTileLoading) {
                observedObjects
                    .Where(o => o.requireLevel)
                    .Select(o => o.position)
                    .Append(observedActor.position)
                    .Select(map.WorldToCell)
                    .ToList();
                throw new NotImplementedException(nameof(allowNonActorTileLoading));
            } else {
                observedCenter = map.WorldToCell(observedActor.position);
            }

            if (lastCenter != observedCenter) {
                lastCenter = observedCenter;

                DiscardOldTiles();
                LoadNewTiles();

                if (tilesChangedCount > 0) {
                    currentColliderIndex = 0;
                }
            }
        }
        void DiscardOldTiles() {
            loadedTilePositions
                .Where(IsOutOfBounds)
                .ToList()
                .ForAll(DiscardTile);
        }
        bool IsOutOfBounds(Vector3Int position) {
            if (position.x >= observedCenter.x - maximumRangeX && position.x <= observedCenter.x + maximumRangeX) {
                if (position.y >= observedCenter.y - maximumRangeY && position.y <= observedCenter.y + maximumRangeY) {
                    return false;
                }
            }
            return true;
        }
        void LoadNewTiles() {
            for (int x = observedCenter.x - minimumRangeX; x <= observedCenter.x + minimumRangeX; x++) {
                for (int y = observedCenter.y - minimumRangeY; y <= observedCenter.y + minimumRangeY; y++) {
                    var position = new Vector3Int(x, y, 0);
                    if (!loadedTilePositions.Contains(position)) {
                        LoadTile(position);
                        if (tilesChangedCount == tilesChangedMaximum) {
                            return;
                        }
                    }
                }
            }
        }
        void DiscardTile(Vector3Int position) {
            for (int i = 0; i < map.tilemapControllers.Length; i++) {
                map.tilemapControllers[i].DiscardTile(position);
            }
            loadedTilePositions.Remove(position);
        }
        void LoadTile(Vector3Int position) {
            for (int i = 0; i < map.tilemapControllers.Length; i++) {
                map.tilemapControllers[i].LoadTile(position);
            }
            loadedTilePositions.Add(position);
            tilesChangedCount++;
        }
        void OnValidate() {
            if (installTilemap) {
                installTilemap = false;
                StartCoroutine(InstallTilemap());
            }
        }
        IEnumerator InstallTilemap() {
            yield return null;
            map.Install(transform);
            Debug.Log("InstallTilemap complete!");
        }

        public IList<TileShape> GetTileShapes(ISet<Vector3Int> positions) {
            var shapes = new List<TileShape>();
            var bounds = new BoundsInt(observedCenter, new Vector3Int(colliderRangeX, colliderRangeY, 0));
            bool inBounds(Vector3Int testPosition) {
                return bounds.Contains(testPosition) && positions.Contains(testPosition);
            }
            for (int y = observedCenter.y - colliderRangeY; y <= observedCenter.y + colliderRangeY; y++) {
                for (int x = observedCenter.x - colliderRangeX; x <= observedCenter.x + colliderRangeX; x++) {
                    var position = new Vector3Int(x, y, 0);
                    if (positions.Contains(position)) {
                        for (int i = 0; i < shapes.Count; i++) {
                            if (shapes[i].ContainsPosition(position)) {
                                goto SKIP;
                            }
                        }
                        shapes.Add(CreateTileShape(inBounds, position, Vector3Int.up));
                        if (shapes.Count == shapeCountMaximum) {
                            return shapes;
                        }
                    }
SKIP:
                    ;
                }
            }
            return shapes;
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
            int i = 0;
            do {
                if (inBounds(position + direction + backwardRotation[direction])) {
                    position += direction + backwardRotation[direction];
                    direction = backwardRotation[direction];
                    shape.positions.Add(position);
                    shape.vertices.Add(offsets[direction] + new Vector2(position.x, position.y));
                } else if (inBounds(position + direction)) {
                    position += direction;
                } else {
                    direction = forwardRotation[direction];
                    shape.positions.Add(position);
                    shape.vertices.Add(offsets[direction] + new Vector2(position.x, position.y));
                }
                if (++i == 1000) {
                    Debug.Log($"Stack overflow in {inBounds}! Position: {position} Direction: {direction}");
                    Debug.Log(string.Join(", ", shape.positions));
                    throw new Exception(inBounds.ToString());
                }
            } while (!(position == startPosition && direction == startDirection));
            return shape;
        }

        void OnDrawGizmos() {
            if (observedActor) {
                var center = new Vector3((int)observedActor.position.x, (int)observedActor.position.y, 0);
                center += map.tileAnchor;
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center, new Vector3((2 * minimumRangeX) + 1, (2 * minimumRangeY) + 1, 1));
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(center, new Vector3((2 * maximumRangeX) + 1, (2 * maximumRangeY) + 1, 1));
            }
        }
    }
}