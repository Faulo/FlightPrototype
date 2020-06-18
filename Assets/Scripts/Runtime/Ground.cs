using UnityEngine;

namespace TheCursedBroom {
    public class Ground : MonoBehaviour {
        [SerializeField, Range(0.001f, 10)]
        public float staticFriction = 1;
        [SerializeField, Range(0.001f, 10)]
        public float kinematicFriction = 1;
    }
}