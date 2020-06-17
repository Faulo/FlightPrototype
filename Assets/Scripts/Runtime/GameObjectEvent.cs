using System;
using UnityEngine;
using UnityEngine.Events;

namespace TheCursedBroom {
    [Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> {
    }
}