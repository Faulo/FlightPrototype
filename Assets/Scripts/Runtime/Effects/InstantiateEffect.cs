using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu()]
    public class InstantiateEffect : Effect {
        [SerializeField, Expandable]
        GameObject[] prefabs = default;

        public override void Invoke(GameObject context) {
            var prefab = prefabs.RandomElement();
            Instantiate(prefab, context.transform.position, prefab.transform.rotation);
        }
    }
}
