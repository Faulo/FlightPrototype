using System.Collections.Generic;
using UnityEngine;

namespace TheCursedBroom.Level {
    public abstract class TilemapColliderBakerAsset : ScriptableObject {
        public abstract void SetupBaker(TilemapController tilemap);

        public static IEnumerable<Vector2> GetPoints(TilemapLayerAsset type) {
            foreach (var position in LevelController.instance.loadedTilePositions) {
                if (LevelController.instance.HasTile(type, position)) {
                    yield return new Vector2(position.x, position.y);
                }
            }
        }
    }
}