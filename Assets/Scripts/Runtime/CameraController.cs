using System.Collections;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TheCursedBroom {
    public class CameraController : MonoBehaviour {
        public static CameraController instance;
        [SerializeField, Expandable]
        Camera observedCamera = default;
        [SerializeField, Expandable]
        Light2D observedLight = default;
        void Awake() {
            instance = this;
        }

        Coroutine changeColorRoutine;
        public void ChangeBackgroundColorTo(Color targetColor, float duration) {
            if (changeColorRoutine != null) {
                StopCoroutine(changeColorRoutine);
            }
            changeColorRoutine = StartCoroutine(CreateColorChange(targetColor, duration));
        }
        IEnumerator CreateColorChange(Color targetColor, float duration) {
            float timer = duration;
            var startColor = observedCamera.backgroundColor;
            while (timer > 0) {
                timer -= Time.deltaTime;
                observedCamera.backgroundColor = Color.Lerp(startColor, targetColor, 1 - (timer / duration));
                yield return null;
            }
            observedCamera.backgroundColor = targetColor;
        }
        Coroutine changeLightRoutine;
        public void ChangeMainLightTo(float targetIntensity, Color targetColor, float duration) {
            if (changeLightRoutine != null) {
                StopCoroutine(changeLightRoutine);
            }
            changeLightRoutine = StartCoroutine(CreateMainLightChange(targetIntensity, targetColor, duration));
        }
        IEnumerator CreateMainLightChange(float targetIntensity, Color targetColor, float duration) {
            float timer = duration;
            var startColor = observedLight.color;
            float startIntensity = observedLight.intensity;
            while (timer > 0) {
                timer -= Time.deltaTime;
                observedLight.color = Color.Lerp(startColor, targetColor, 1 - (timer / duration));
                observedLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, 1 - (timer / duration));
                yield return null;
            }
            observedLight.intensity = targetIntensity;
        }
    }
}