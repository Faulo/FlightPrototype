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
        [SerializeField, Expandable]
        Tilemap background = default;
        [SerializeField, Expandable]
        Tilemap ground = default;
        [SerializeField, Expandable]
        Tilemap objects = default;
        [SerializeField, Expandable]
        Tilemap decorations = default;

        [Header("Levels")]
        [SerializeField, Expandable]
        TilemapChunk[] levels = new TilemapChunk[0];

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onStart = default;

        public Transform observedActor;
        Vector3Int actorPosition;
        ISet<Vector3Int> tilePositions = new HashSet<Vector3Int>();

        [Header("Chunk loading")]
        [SerializeField]
        Vector3Int minimumRange = new Vector3Int(80, 45, 1);
        [SerializeField]
        Vector3Int maximumRange = new Vector3Int(160, 90, 1);

        void Awake() {
            instance = this;
        }
        void Start() {
            onStart.Invoke(gameObject);
        }
        void FixedUpdate() {
            if (observedActor) {
                actorPosition = ground.WorldToCell(observedActor.position);
                actorPosition.z = 0;

                DetermineOldTiles()
                    .ToList()
                    .ForAll(DiscardTile);

                DetermineNewTiles()
                    .ToList()
                    .ForAll(LoadTile);
            }
        }
        IEnumerable<Vector3Int> DetermineOldTiles() {
            var outBounds = new BoundsInt(actorPosition - (maximumRange / 2), maximumRange);
            foreach (var position in tilePositions) {
                if (!outBounds.Contains(position)) {
                    yield return position;
                }
            }
        }
        IEnumerable<Vector3Int> DetermineNewTiles() {
            var inBounds = new BoundsInt(actorPosition - (minimumRange / 2), minimumRange);
            foreach (var position in inBounds.allPositionsWithin) {
                if (!tilePositions.Contains(position)) {
                    yield return position;
                }
            }
        }
        void DiscardTile(Vector3Int position) {
            ground.SetTile(position, null);
            tilePositions.Remove(position);
        }
        void LoadTile(Vector3Int position) {
            ground.SetTile(position, GetTile(position));
            tilePositions.Add(position);
        }
        TileBase GetTile(Vector3Int position) {
            while (position.x < 0) {
                position.x += levels[0].size.x;
            }
            position.x %= levels[0].size.x;
            return levels[0].ground.GetTile(position);
        }

        void OnValidate() {
            if (!background) {
                background = GetComponentsInChildren<Tilemap>()[0];
            }
            if (!ground) {
                ground = GetComponentsInChildren<Tilemap>()[1];
            }
            if (!objects) {
                objects = GetComponentsInChildren<Tilemap>()[2];
            }
            if (!decorations) {
                decorations = GetComponentsInChildren<Tilemap>()[3];
            }
        }
    }
}