using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom.Level {
    public class LevelController : MonoBehaviour {
        [SerializeField]
        GameObjectEvent onStart = default;

        void Start() {
            onStart.Invoke(gameObject);
        }
    }
}