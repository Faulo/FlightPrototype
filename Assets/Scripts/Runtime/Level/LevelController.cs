using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class LevelController : MonoBehaviour {
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

        void Start() {
            var level = levels[0];

            var offset = new Vector3Int(level.size.x, 0, 0);

            foreach (var position in level.tilePositions) {
                if (level.ground.HasTile(position)) {
                    ground.SetTile(position - offset, level.ground.GetTile(position));
                    ground.SetTile(position, level.ground.GetTile(position));
                    ground.SetTile(position + offset, level.ground.GetTile(position));
                }
            }


            onStart.Invoke(gameObject);
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