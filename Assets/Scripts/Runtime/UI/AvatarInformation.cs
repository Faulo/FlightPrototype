using System.Collections.Generic;
using System.Reflection;
using Slothsoft.UnityExtensions;
using TheCursedBroom.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheCursedBroom.UI {
    public class AvatarInformation : FormattableText {
        [SerializeField, Expandable]
        AvatarController avatar = default;
        public override IDictionary<string, string> parameters {
            get {
                var dict = new Dictionary<string, string>();
                if (avatar) {
                    dict["Avatar.forward"] = avatar.forward.ToString();
                    dict["Avatar.rotationAngle"] = avatar.rotationAngle.ToString();
                    dict["Avatar.facing"] = avatar.facing.ToString();
                    dict["Avatar.velocity"] = avatar.velocity.ToString();
                    dict["Avatar.drag"] = avatar.drag.ToString();
                    dict["Avatar.gravityScale"] = avatar.gravityScale.ToString();
                }
                return dict;
            }
        }
    }
}