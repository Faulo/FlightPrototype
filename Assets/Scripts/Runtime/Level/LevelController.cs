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
                actorPosition = tilemaps.WorldToCell(observedActor.position);
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
            int i = (int)(tilemaps.CellToWorld(position).y / tilemaps.height);
            if (i < 0 || i >= levels.Length) {
                return null;
            }
            position.y -= i * tilemaps.height;

            while (position.x < 0) {
                position.x += tilemaps.width;
            }
            position.x %= tilemaps.width;
            return levels[i].GetTile(type, position);
        }

        void OnValidate() {
            tilemaps.OnValidate(transform);
        }
        void OnDrawGizmos() {
            if (observedActor) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(observedActor.position, minimumRange);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(observedActor.position, maximumRange);
            }
        }
    }
}