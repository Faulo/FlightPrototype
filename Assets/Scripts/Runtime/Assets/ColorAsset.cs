using UnityEngine;

namespace TheCursedBroom.Assets {
    [CreateAssetMenu()]
    public class ColorAsset : ScriptableObject {
        [SerializeField, ColorUsage(true, true)]
        public Color color = Color.white;
    }
}