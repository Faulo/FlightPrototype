using Slothsoft.UnityExtensions;
using TheCursedBroom.Player;
using UnityEngine;
using UnityEngine.Audio;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "StateSave_New", menuName = "Effects/State Save")]
    public class StateSaveEffect : Effect {
        public static Vector3 position;
        public override void Invoke(GameObject context) {
            var avatar = FindObjectOfType<AvatarController>();
            position = avatar.transform.position;
        }
    }
}
