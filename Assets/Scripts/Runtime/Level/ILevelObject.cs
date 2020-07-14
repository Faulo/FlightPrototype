using UnityEngine;

namespace TheCursedBroom.Level {
    public interface ILevelObject {
        Vector3 position { get; }

        bool requireLevel { get; }

        void TranslateX(float x);
    }
}