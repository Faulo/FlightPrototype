using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "QuitGame", menuName = "Effects/Quit Game")]
    public class QuitGame : Effect {
        public override void Invoke(GameObject context) {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}