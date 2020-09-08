using UnityEngine;

namespace TheCursedBroom.Extensions {
    public static class GameObjectExtensions {
        public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject)
            where TComponent : Component {
            return gameObject.TryGetComponent<TComponent>(out var component)
                ? component
                : gameObject.AddComponent<TComponent>();
        }
        public static void DestroyComponent<TComponent>(this GameObject gameObject)
           where TComponent : Component {
            if (gameObject.TryGetComponent<TComponent>(out var component)) {
#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(component);
#else
                UnityEngine.Object.Destroy(component);
#endif
            }
        }
    }
}