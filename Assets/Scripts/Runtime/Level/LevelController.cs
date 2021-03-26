using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;
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
        [SerializeField, Range(1, 80)]
        int pauseBetweenTilemapUpdates = 1;
        int currentTilemapIndex = 0;

        [SerializeField]
        LevelSettingsAsset lowSettings = default;
        [SerializeField]
        LevelSettingsAsset mediumSettings = default;
        [SerializeField]
        LevelSettingsAsset highSettings = default;
        LevelSettingsAsset GetCurrentSettings() => QualitySettings.GetQualityLevel() switch {
            0 => lowSettings,
            1 => mediumSettings,
            2 => highSettings,
            _ => throw new NotImplementedException(),
        };

        bool allowMultithreading;
        int pauseBetweenThreadUpdates;
        [NonSerialized]
        public TilemapBounds rendererBounds;
        [NonSerialized]
        public TilemapBounds colliderBounds;
        [NonSerialized]
        public TilemapBounds shadowBounds;

        void ApplySettings() {
            var settings = GetCurrentSettings();
            allowMultithreading = settings.allowMultithreading;
            pauseBetweenThreadUpdates = settings.pauseBetweenThreadUpdates;
            rendererBounds = settings.rendererBounds;
            colliderBounds = settings.colliderBounds;
            shadowBounds = settings.shadowBounds;
        }

        [Header("Debug output")]
        public Transform observedActor;
        public readonly ISet<ILevelObject> observedObjects = new HashSet<ILevelObject>();
        Vector3Int observedCenter;

        void Awake() {
            OnValidate();
            instance = this;
            ApplySettings();
            PrepareTilemap();
        }
        void OnValidate() {
#if UNITY_EDITOR
            EditorTools();
#endif
        }
        Thread tileUpdater;
        UpdateState tileState;
        enum UpdateState {
            Idle,
            Prepare,
            Update,
        }
        void OnEnable() {
            if (allowMultithreading) {
                tileState = UpdateState.Idle;
                tileUpdater = new Thread(UpdateTilesThread) {
                    Name = "TileUpdater"
                };
                tileUpdater.Start();
            }
        }
        void OnDisable() {
            if (allowMultithreading) {
                tileUpdater?.Abort();
            }
        }
        void Start() {
            onStart.Invoke(gameObject);
        }
        void UpdateTilesThread() {
            while (true) {
                switch (tileState) {
                    case UpdateState.Idle:
                        break;
                    case UpdateState.Prepare:
                        PrepareTiles();
                        tileState = UpdateState.Update;
                        break;
                    case UpdateState.Update:
                        UpdateTiles();
                        break;
                }
                Thread.Sleep(pauseBetweenThreadUpdates);
            }
        }
        void Update() {
            if (currentTilemapIndex < map.tilemapControllers.Length * pauseBetweenTilemapUpdates) {
                if (currentTilemapIndex % pauseBetweenTilemapUpdates == 0) {
                    map.tilemapControllers[currentTilemapIndex / pauseBetweenTilemapUpdates].RegenerateTilemap();
                }
                currentTilemapIndex++;
            } else {
                UpdateObservedCenter();
                if (!allowMultithreading) {
                    UpdateTiles();
                }
            }
        }
        public void RefreshAllTiles() {
            Assert.IsTrue(observedActor);
            UpdateObservedCenter();
            if (allowMultithreading) {
                tileState = UpdateState.Prepare;
            } else {
                PrepareTiles();
                for (int i = 0; i < map.tilemapControllers.Length; i++) {
                    map.tilemapControllers[i].RegenerateTilemap();
                }
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
        void PrepareTilemap() {
            map.Install(transform);
            for (int i = 0; i < levels.Length; i++) {
                levels[i].tilemaps.Install(levels[i].transform);
            }
        }
        void UpdateObservedCenter() {
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
        }
        void PrepareTiles() {
            var center = observedCenter;
            rendererBounds.PrepareTiles(center);
            colliderBounds.PrepareTiles(center);
            shadowBounds.PrepareTiles(center);
            currentTilemapIndex = 0;
        }
        void UpdateTiles() {
            var center = observedCenter;
            int tilesChangedCount = 0;

            tilesChangedCount += colliderBounds.UpdateTiles(center);
            tilesChangedCount += rendererBounds.UpdateTiles(center);
            tilesChangedCount += shadowBounds.UpdateTiles(center);

            if (tilesChangedCount > 0) {
                currentTilemapIndex = 0;
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
                shadowBounds.OnDrawGizmos();
            }
        }
#endif
    }
}