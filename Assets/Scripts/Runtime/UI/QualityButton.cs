using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheCursedBroom.UI {
    public class QualityButton : MonoBehaviour {
        [SerializeField]
        Button observedButton = default;
        [SerializeField, Range(0, 2)]
        int quality = 0;

        void Awake() {
            OnValidate();
            observedButton.onClick.AddListener(HandleClick);
        }
        void Start() {
            if (QualitySettings.GetQualityLevel() == quality) {
                observedButton.Select();
            }
        }
        void HandleClick() {
            QualitySettings.SetQualityLevel(quality);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
        void OnValidate() {
            if (!observedButton) {
                TryGetComponent(out observedButton);
            }
        }
    }
}