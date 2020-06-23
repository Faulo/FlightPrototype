using Cinemachine;
using Slothsoft.UnityExtensions;
using TheCursedBroom.Player;
using UnityEngine;
using UnityEngine.Audio;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "StateLoad_New", menuName = "Effects/State Load")]
    public class StateLoadEffect : Effect {
        public override void Invoke(GameObject context) {
            var avatar = FindObjectOfType<AvatarController>();
            avatar.transform.position = StateSaveEffect.position;
        }
    }
}
