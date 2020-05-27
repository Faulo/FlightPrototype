using System.Collections;
using System.Collections.Generic;
using Extensions;
using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class FormattableText : MonoBehaviour {
        [SerializeField, Range(0, 1)]
        float refreshInterval = 1;

        public abstract IDictionary<string, string> parameters { get; }

        IEnumerator Start() {
            var textMesh = GetComponent<TextMeshProUGUI>();
            string template = textMesh.text;
            while (true) {
                textMesh.text = template.Format(parameters);
                yield return new WaitForSeconds(refreshInterval);
            }
        }

        public void ToggleDisplay() {
            GetComponent<TextMeshProUGUI>().enabled = !GetComponent<TextMeshProUGUI>().enabled;
        }
    }
}