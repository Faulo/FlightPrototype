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
        IDictionary<TilemapLayerAsset, TileBase[][]> tiles = new Dictionary<TilemapLayerAsset, TileBase[][]>();
        ISet<Vector3Int> loadedTilePositions = new HashSet<Vector3Int>();

        bool tilesHaveChanged;

        [Header("Levels")]
        [SerializeField, Expandable]
        TilemapChunk[] levels = new TilemapChunk[0];

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onStart = default;

        public Transform observedActor;
        public ISet<ILevelObject> observedObjects = new HashSet<ILevelObject>();
        IList<Vector3Int> observedPositions = new List<Vector3Int> { Vector3Int.zero };
        IList<Vector3Int> lastPositions = new List<Vector3Int> { Vector3Int.zero };

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
            if (observedActor) {
                UpdateTiles();
            }
        }
        void PrepareTiles() {
            map.Install(transform);
            for (int i = 0; i < levels.Length; i++) {
                levels[i].tilemaps.Install(levels[i].transform);
            }
            foreach (var (layer, tilemap) in map.all) {
                tiles[layer] = new TileBase[map.height * levels.Length][];
                for (int i = 0; i < levels.Length; i++) {
                    for (int y = 0; y < map.height; y++) {
                        int j = y + (i * map.height);
                        tiles[layer][j] = new TileBase[map.width];
                        for (int x = 0; x < map.width; x++) {
                            tiles[layer][j][x] = levels[i].GetTile(layer, new Vector3Int(x, y, 0));
                        }
                    }
                }
            }
        }
        void UpdateTiles() {
            tilesHaveChanged = false;

            foreach (var observedObject in observedObjects) {
                while (observedObject.position.x < observedActor.position.x - (map.width / 2)) {
                    observedObject.TranslateX(map.width);
                }
                while (observedObject.position.x > observedActor.position.x + (map.width / 2)) {
                    observedObject.TranslateX(-map.width);
                }
            }
            if (allowNonActorTileLoading) {
                observedPositions = observedObjects
                    .Where(o => o.requireLevel)
                    .Select(o => o.position)
                    .Append(observedActor.position)
                    .Select(map.WorldToCell)
                    .ToList();
            } else {
                observedPositions[0] = map.WorldToCell(observedActor.position);
            }

            if (lastPositions[0] != observedPositions[0]) {
                lastPositions[0] = observedPositions[0];

                DiscardOldTiles();
                LoadNewTiles();

                if (tilesHaveChanged) {
                    foreach (var collider in map.colliders) {
                        collider.GenerateGeometry();
                    }
                }
            }
        }
        void DiscardOldTiles() {
            var positions = new HashSet<Vector3Int>();
            foreach (var position in loadedTilePositions) {
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
            foreach (var center in observedPositions) {
                if (position.x >= center.x - maximumRangeX && position.x <= center.x + maximumRangeX) {
                    if (position.y >= center.y - maximumRangeY && position.y <= center.y + maximumRangeY) {
                        return false;
                    }
                }
            }
            return true;
        }
        void LoadNewTiles() {
            foreach (var center in observedPositions) {
                for (int x = center.x - minimumRangeX; x <= center.x + minimumRangeX; x++) {
                    for (int y = center.y - minimumRangeY; y <= center.y + minimumRangeY; y++) {
                        var position = new Vector3Int(x, y, 0);
                        if (!loadedTilePositions.Contains(position)) {
                            LoadTile(position);
                        }
                    }
                }
            }
        }
        void DiscardTile(Vector3Int position) {
            foreach (var (_, tilemap) in map.all) {
                tilemap.SetTile(position, null);
            }
            loadedTilePositions.Remove(position);
        }
        void LoadTile(Vector3Int position) {
            foreach (var (type, tilemap) in map.all) {
                tilemap.SetTile(position, GetTile(type, position));
            }
            loadedTilePositions.Add(position);
            tilesHaveChanged = true;
        }
        TileBase GetTile(TilemapLayerAsset type, Vector3Int position) {
            if (position.y < 0 || position.y >= map.height * levels.Length) {
                return null;
            }
            while (position.x < 0) {
                position.x += map.width;
            }
            position.x %= map.width;
            return tiles[type][position.y][position.x];
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