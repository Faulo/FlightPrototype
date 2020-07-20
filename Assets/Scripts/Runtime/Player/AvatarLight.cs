using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TheCursedBroom.Player {
    public class AvatarLight : MonoBehaviour {
        [SerializeField, Expandable]
        AvatarController observedAvatar = default;
        [SerializeField, Expandable]
        Light2D observedLight = default;
        [SerializeField]
        AnimationCurve radiusOverCollectibleCount = default;

        int collectibleCount = 0;
        void Start() {
            UpdateLight();
        }

        void Update() {
            if (collectibleCount != observedAvatar.collectibleCount) {
                collectibleCount = observedAvatar.collectibleCount;
                UpdateLight();
            }
        }

        void UpdateLight() {
            observedLight.pointLightOuterRadius = radiusOverCollectibleCount.Evaluate(collectibleCount);
        }
    }
}