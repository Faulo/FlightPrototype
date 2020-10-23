using System;
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

        [Header("Levels")]
        [SerializeField, Expandable]
        TilemapChunk[] levels = new TilemapChunk[0];

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onStart = default;

        [Header("Chunk loading")]
        [SerializeField, Tooltip("Whether or not to respect the ILevelObject::requireLevel property")]
        bool allowNonActorTileLoading = false;
        [SerializeField]
        public TilemapBounds colliderBounds = new TilemapBounds();
        [SerializeField]
        public TilemapBounds rendererBounds = new TilemapBounds();

        int currentColliderIndex = 0;
        [SerializeField, Range(1, 80)]
        int pauseBetweenColliderUpdates = 1;

        [Header("Debug output")]
        public Transform observedActor;
        public readonly ISet<ILevelObject> observedObjects = new HashSet<ILevelObject>();
        Vector3Int observedCenter;

        void Awake() {
            OnValidate();
            instance = this;
            PrepareTiles();
        }
        void OnValidate() {
#if UNITY_EDITOR
            EditorTools();
#endif
        }
        void Start() {
            onStart.Invoke(gameObject);
            RefreshAllTiles();
        }
        void FixedUpdate() {
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
            colliderBounds.PrepareTiles();
            rendererBounds.PrepareTiles();
        }
        void UpdateTiles() {
            if (!observedActor) {
                return;
            }

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

            int tilesChangedCount = 0;

            tilesChangedCount += colliderBounds.UpdateTiles(observedCenter);
            tilesChangedCount += rendererBounds.UpdateTiles(observedCenter);

            if (tilesChangedCount > 0) {
                currentColliderIndex = 0;
            }
        }

#if UNITY_EDITOR
        void EditorTools() {
            if (installTilemap) {
                installTilemap = false;
                StartCoroutine(InstallTilemap());
            }
        }
        [Header("Editor Tools")]
        [SerializeField]
        bool installTilemap = false;
        IEnumerator InstallTilemap() {
            yield return null;
            map.Install(transform);
            Debug.Log("InstallTilemap complete!");
        }
        void OnDrawGizmos() {
            if (observedActor) {
                colliderBounds.OnDrawGizmos();
                rendererBounds.OnDrawGizmos();
            }
        }
#endif
    }
}