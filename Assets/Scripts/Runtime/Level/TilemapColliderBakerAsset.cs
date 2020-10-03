using UnityEngine;

namespace TheCursedBroom.Level {
    public abstract class TilemapColliderBakerAsset : ScriptableObject {
        public abstract void SetupBaker(TilemapController tilemap);
    }
}