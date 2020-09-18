using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom {
    public abstract class ComponentFeature<TComponent> : MonoBehaviour where TComponent : Component {
        [SerializeField, Expandable]
        protected TComponent observedComponent = default;
        protected virtual void Awake() {
            OnValidate();
        }
        protected virtual void OnValidate() {
            if (observedComponent == null) {
                observedComponent = GetComponentInParent<TComponent>();
            }
        }
    }
}