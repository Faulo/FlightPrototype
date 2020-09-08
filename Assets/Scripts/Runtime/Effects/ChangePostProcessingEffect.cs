using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "ChangePostProcessing_New", menuName = "Effects/ChangePostProcessing")]
    public class ChangePostProcessingEffect : Effect {
        [SerializeField, Expandable]
        VolumeProfile profile = default;

        public override void Invoke(GameObject context) {
            FindObjectOfType<Volume>().profile = profile;
        }
    }
}