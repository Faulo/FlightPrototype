using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarEventBridge : MonoBehaviour {
        AvatarController avatar;

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onSpawn = default;
        [SerializeField]
        GameObjectEvent onSave = default;
        [SerializeField]
        GameObjectEvent onLoad = default;
        [SerializeField]
        GameObjectEvent onReset = default;

        void Awake() {
            avatar = GetComponentInParent<AvatarController>();
        }

        void OnEnable() {
            avatar.onSpawn += onSpawn.Invoke;
            avatar.onSave += onSave.Invoke;
            avatar.onLoad += onLoad.Invoke;
            avatar.onReset += onReset.Invoke;
        }

        void OnDisable() {
            avatar.onSpawn -= onSpawn.Invoke;
            avatar.onSave -= onSave.Invoke;
            avatar.onLoad -= onLoad.Invoke;
            avatar.onReset -= onReset.Invoke;
        }
    }
}