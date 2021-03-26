using MyBox;
using UnityEngine;

namespace TheCursedBroom.Level {
    [CreateAssetMenu()]
    public class LevelSettingsAsset : ScriptableObject {
        [SerializeField]
        public bool allowMultithreading = false;
        [SerializeField, ConditionalField(nameof(allowMultithreading)), Range(0, 100)]
        public int pauseBetweenThreadUpdates = 0;
        [SerializeField]
        public TilemapBounds rendererBounds = new TilemapBounds();
        [SerializeField]
        public TilemapBounds colliderBounds = new TilemapBounds();
        [SerializeField]
        public TilemapBounds shadowBounds = new TilemapBounds();
    }
}