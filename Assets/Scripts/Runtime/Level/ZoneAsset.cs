using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheCursedBroom.Level {
    public class ZoneAsset : ScriptableObject {
        [SerializeField, Expandable]
        PostProcessingData postProcessing = default;
    }
}