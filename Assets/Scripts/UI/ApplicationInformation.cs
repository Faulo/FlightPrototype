using System.Collections.Generic;
using UnityEngine;

namespace TheCursedBroom.UI {
    public class ApplicationInformation : FormattableText {
        public override IDictionary<string, string> parameters => new Dictionary<string, string> {
            ["Application.companyName"] = Application.companyName,
            ["Application.productName"] = Application.productName,
            ["Application.version"] = Application.version,
            ["Application.installerName"] = Application.installerName,
            ["Application.platform"] = Application.platform.ToString(),
            ["Application.unityVersion"] = Application.unityVersion
        };
    }
}