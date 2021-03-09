using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarRenderer : MonoBehaviour {
        [SerializeField, Expandable]
        AvatarController avatar = default;
        [SerializeField, Expandable]
        SpriteRenderer attachedRenderer = default;

        Material material;

        float dissolveAmount => avatar.dissolveAmount;

        void Awake() {
            OnValidate();
        }
        void OnValidate() {
            if (!avatar) {
                avatar = GetComponentInParent<AvatarController>();
            }
            if (!attachedRenderer) {
                TryGetComponent(out attachedRenderer);
            }
        }
        void OnEnable() {
            material = attachedRenderer.material;
        }
        void OnDisable() {
            Destroy(material);
        }
        void Update() {
            material.SetFloat(nameof(dissolveAmount), dissolveAmount);
        }
    }
}