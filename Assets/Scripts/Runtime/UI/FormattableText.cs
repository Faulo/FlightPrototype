using System.Collections;
using System.Collections.Generic;
using TheCursedBroom.Extensions;
using TMPro;
using UnityEngine;

namespace TheCursedBroom.UI {
    public abstract class FormattableText : ComponentFeature<TextMeshProUGUI> {
        [SerializeField, Range(0, 1)]
        float refreshInterval = 1;
        [SerializeField]
        string template = default;

        public abstract IDictionary<string, string> parameters { get; }

        protected override void Awake() {
            base.Awake();
            template = observedComponent.text;
        }

        void OnEnable() {
            StartCoroutine(UpdateTextRoutine());
        }

        IEnumerator UpdateTextRoutine() {
            while (true) {
                observedComponent.text = template.Format(parameters);
                yield return new WaitForSeconds(refreshInterval);
            }
        }
    }
}