using System;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "ChangeQualitySetting_New", menuName = "Effects/ChangeQualitySetting")]
    public class ChangeQualitySettingEffect : Effect {
        enum Change {
            IncreaseByOne,
            DecreaseByOne,
        }
        [SerializeField]
        Change change = default;

        public override void Invoke(GameObject context) {
            switch (change) {
                case Change.IncreaseByOne:
                    QualitySettings.IncreaseLevel();
                    break;
                case Change.DecreaseByOne:
                    QualitySettings.DecreaseLevel();
                    break;
                default:
                    throw new NotImplementedException(change.ToString());
            }
        }
    }
}