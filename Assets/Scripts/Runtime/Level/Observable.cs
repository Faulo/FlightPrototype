using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Level {
    public class Observable : MonoBehaviour, ILevelObject {
        [SerializeField]
        bool isActor = false;
        [SerializeField, Tooltip("Whether or not the level around this object will be kept in memory")]
        bool m_requireLevel = false;

        public Vector3 position => transform.position;
        public bool requireLevel => m_requireLevel;
        public void TranslateX(float x) {
            transform.Translate(new Vector3(x, 0, 0));
        }

        void Start() {
            if (LevelController.instance) {
                if (isActor) {
                    Assert.IsNull(LevelController.instance.observedActor);
                    LevelController.instance.observedActor = transform;
                    LevelController.instance.RefreshAllTiles();
                } else {
                    LevelController.instance.observedObjects.Add(this);
                }
            }
        }
        void OnDestroy() {
            if (LevelController.instance) {
                if (isActor) {
                    Assert.AreEqual(transform, LevelController.instance.observedActor);
                    LevelController.instance.observedActor = null;
                } else {
                    LevelController.instance.observedObjects.Remove(this);
                }
            }
        }
    }
}