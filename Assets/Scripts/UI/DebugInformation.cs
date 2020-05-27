using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class DebugInformation : FormattableText {
        public override IDictionary<string, string> parameters {
            get {
                return new Dictionary<string, string> {
                    ["Scene.name"] = SceneManager.GetActiveScene().name,
                    ["Scene.path"] = SceneManager.GetActiveScene().path,
                    ["Assembly.version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    ["Assembly.build"] = Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(),
                    ["Assembly.revision"] = Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString(),
                    ["Debug.FPS"] = Time.smoothDeltaTime == 0 ? "???" : Mathf.RoundToInt(1 / Time.smoothDeltaTime).ToString(),
                };
            }
        }
    }
}