using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "Instantiate_New", menuName = "Effects/Instantiate")]
    public class InstantiateEffect : Effect {
        [SerializeField, Expandable]
        GameObject[] prefabs = default;

        public override void Invoke(GameObject context) {
            var prefab = prefabs.RandomElement();
            Instantiate(prefab, context.transform.position, context.transform.rotation);
        }
    }
}
