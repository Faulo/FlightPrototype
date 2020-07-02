using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "Instantiate_New", menuName = "Effects/Instantiate")]
    public class InstantiateEffect : Effect {
        [SerializeField, Expandable]
        GameObject[] prefabs = default;
        [SerializeField]
        bool destroyPreviousInstance = false;
        GameObject instance;

        public override void Invoke(GameObject context) {
            var prefab = prefabs.RandomElement();
            if (destroyPreviousInstance && instance) {
                Destroy(instance);
            }
            instance = Instantiate(prefab, context.transform.position, context.transform.rotation);
        }
    }
}
