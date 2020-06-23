using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "StateLoad_New", menuName = "Effects/State Load")]
    public class StateLoadEffect : Effect {
        public override void Invoke(GameObject context) {
            var avatar = FindObjectOfType<AvatarController>();
            if (avatar) {
                avatar.StateLoad();
            }
        }
    }
}
