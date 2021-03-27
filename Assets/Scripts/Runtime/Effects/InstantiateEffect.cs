using System.Linq;
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
                if (instance.TryGetComponent<ParticleSystem>(out var particles)) {
                    var main = particles.main;
                    main.stopAction = ParticleSystemStopAction.Destroy;
                    particles.Stop();
                    Enumerable.Range(0, instance.transform.childCount)
                        .Select(instance.transform.GetChild)
                        .ToList()
                        .ForAll(t => Destroy(t.gameObject));
                } else {
                    Destroy(instance);
                }
            }
            instance = Instantiate(prefab, context.transform.position, context.transform.rotation);
        }
    }
}
