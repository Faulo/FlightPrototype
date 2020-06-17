using System;
using UnityEngine;
using UnityEngine.Events;

namespace TheCursedBroom.Player {
    [Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> {
    }
}