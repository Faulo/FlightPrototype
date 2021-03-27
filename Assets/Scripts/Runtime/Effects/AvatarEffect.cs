using System;
using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "Avatar_New", menuName = "Effects/Avatar")]
    public class AvatarEffect : Effect {
        enum Method {
            CastSave,
            CastLoad,
            StateSave,
            StateLoad,
            Die,
        }
        [SerializeField]
        Method methodToCall = default;
        public override void Invoke(GameObject context) {
            if (!AvatarController.instance) {
                return;
            }
            switch (methodToCall) {
                case Method.CastSave:
                    AvatarController.instance.CastSave();
                    break;
                case Method.CastLoad:
                    AvatarController.instance.CastLoad();
                    break;
                case Method.StateSave:
                    AvatarController.instance.StateSave();
                    break;
                case Method.StateLoad:
                    AvatarController.instance.StateLoad();
                    break;
                case Method.Die:
                    AvatarController.instance.Die();
                    break;
                default:
                    throw new NotImplementedException(methodToCall.ToString());
            }
        }
    }
}