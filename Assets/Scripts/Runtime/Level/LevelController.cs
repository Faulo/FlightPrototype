using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class LevelController : MonoBehaviour {
        enum TileState {
            LoadRequested,
            Active,
            Discard
        }
        public static LevelController instance;

        [Header("MonoBehaviour configuration")]
        [SerializeField]
        TilemapContainer tilemaps = default;
        TileBase[][] tiles;

        [Header("Levels")]
        [SerializeField, Expandable]
        TilemapChunk[] levels = new TilemapChunk[0];

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onStart = default;


        public Transform observedActor;
        public ISet<ILevelObject> observedObjects = new HashSet<ILevelObject>();
        IList<Vector3Int> objectPositions = new List<Vector3Int> { Vector3Int.zero };

        ISet<Vector3Int> tilePositions = new HashSet<Vector3Int>();

        [Header("Chunk loading")]
        [SerializeField, Tooltip("Whether or not to respect the ILevelObject::requireLevel property")]
        bool allowNonActorTileLoading = false;
        [SerializeField, Range(1, 100)]
        int minimumRangeX = 48;
        [SerializeField, Range(1, 100)]
        int minimumRangeY = 27;
        [SerializeField, Range(1, 100)]
        int maximumRangeX = 64;
        [SerializeField, Range(1, 100)]
        int maximumRangeY = 36;

        void Awake() {
            instance = this;
            tiles = new TileBase[tilemaps.width][];
            for (int x = 0; x < tilemaps.width; x++) {
                tiles[x] = new TileBase[tilemaps.height * levels.Length];
                for (int i = 0; i < levels.Length; i++) {
                    for (int y = 0; y < tilemaps.height; y++) {
                        tiles[x][y + (i * tilemaps.height)] = levels[i].GetTile(TilemapType.Ground, new Vector3Int(x, y, 0));
                    }
                }
            }
        }
        void Start() {
            onStart.Invoke(gameObject);
        }
        void FixedUpdate() {
            if (observedActor) {
                foreach (var observedObject in observedObjects) {
                    while (observedObject.position.x < observedActor.position.x - (tilemaps.width / 2)) {
                        observedObject.TranslateX(tilemaps.width);
                    }
                    while (observedObject.position.x > observedActor.position.x + (tilemaps.width / 2)) {
                        observedObject.TranslateX(-tilemaps.width);
                    }
                }
                if (allowNonActorTileLoading) {
                    objectPositions = observedObjects
                        .Where(o => o.requireLevel)
                        .Select(o => o.position)
                        .Append(observedActor.position)
                        .Select(tilemaps.WorldToCell)
                        .ToList();
                } else {
                    objectPositions[0] = tilemaps.WorldToCell(observedActor.position);
                }

                DiscardOldTiles();
                LoadNewTiles();
            }
        }
        void DiscardOldTiles() {
            var positions = new HashSet<Vector3Int>();
            foreach (var position in tilePositions) {
                if (!positions.Contains(position)) {
                    if (IsOutOfBounds(position)) {
                        positions.Add(position);
                    }
                }
            }
            foreach (var position in positions) {
                DiscardTile(position);
            }
        }
        bool IsOutOfBounds(Vector3Int position) {
            foreach (var center in objectPositions) {
                if (position.x >= center.x - maximumRangeX && position.x <= center.x + maximumRangeX) {
                    if (position.y >= center.y - maximumRangeY && position.y <= center.y + maximumRangeY) {
                        return false;
                    }
                }
            }
            return true;
        }
        void LoadNewTiles() {
            foreach (var center in objectPositions) {
                foreach (int x in Enumerable.Range(center.x - minimumRangeX, (minimumRangeX * 2) + 1)) {
                    foreach (int y in Enumerable.Range(center.y - minimumRangeY, (minimumRangeY * 2) + 1)) {
                        var position = new Vector3Int(x, y, 0);
                        if (!tilePositions.Contains(position)) {
                            LoadTile(position);
                        }
                    }
                }
            }
        }
        void DiscardTile(Vector3Int position) {
            foreach (var (_, tilemap) in tilemaps.all) {
                tilemap.SetTile(position, null);
            }
            tilePositions.Remove(position);
        }
        void LoadTile(Vector3Int position) {
            foreach (var (type, tilemap) in tilemaps.all) {
                tilemap.SetTile(position, GetTile(type, position));
            }
            tilePositions.Add(position);
        }
        TileBase GetTile(TilemapType type, Vector3Int position) {
            if (position.y < 0 || position.y >= tilemaps.height * levels.Length) {
                return null;
            }
            while (position.x < 0) {
                position.x += tilemaps.width;
            }
            position.x %= tilemaps.width;
            return type == TilemapType.Ground
                ? tiles[position.x][position.y]
                : null;
            /*
            return levels[i].GetTile(type, position);
            //*/
        }

        void OnValidate() {
            tilemaps.OnValidate(transform);
        }
        void OnDrawGizmos() {
            if (observedActor) {
                var center = new Vector3((int)observedActor.position.x, (int)observedActor.position.y, 0);
                center += tilemaps.tileAnchor;
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center, new Vector3((2 * minimumRangeX) + 1, (2 * minimumRangeY) + 1, 1));
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(center, new Vector3((2 * maximumRangeX) + 1, (2 * maximumRangeY) + 1, 1));
            }
        }
    }
}