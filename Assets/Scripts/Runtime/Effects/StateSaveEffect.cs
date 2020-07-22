using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "StateSave_New", menuName = "Effects/State Save")]
    public class StateSaveEffect : Effect {
        public override void Invoke(GameObject context) {
            var avatar = AvatarController.instance;
            if (avatar) {
                avatar.StateSave();
            }
        }
    }
}
