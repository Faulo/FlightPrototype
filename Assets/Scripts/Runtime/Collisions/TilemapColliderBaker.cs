using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using TheCursedBroom.Extensions;
using TheCursedBroom.Level;
using UnityEngine;

namespace TheCursedBroom.Collisions {
    public class TilemapColliderBaker : MonoBehaviour {
        [SerializeField, Expandable]
        public TilemapLayerAsset type = default;
        [SerializeField, Expandable]
        public EdgeCollider2D edgeCollider = default;

        ISet<Vector3Int> loadedTilePositions;

        void OnValidate() {
            if (!edgeCollider) {
                edgeCollider = gameObject.GetOrAddComponent<EdgeCollider2D>();
            }
        }

        void Start() {
            loadedTilePositions = LevelController.instance.loadedTilePositions;
        }

        public void BakeCollider() {
            if (loadedTilePositions.Count > 0) {
                var points = new List<Vector2>();
                foreach (var position in loadedTilePositions) {
                    if (LevelController.instance.HasTile(type, position)) {
                        points.Add(new Vector2(position.x, position.y));
                    }
                }
                edgeCollider.points = points.ToArray();
            }
        }
    }
}