using System.Collections;
using System.Collections.Generic;
using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "CollectibleCount_New", menuName = "Effects/CollectibleCount")]
    public class CollectibleCountEffect : Effect {
        public override void Invoke(GameObject context) {
            if (!AvatarController.instance) {
                return;
            }
            AvatarController.instance.collectibleCount++;
        }
    }
}