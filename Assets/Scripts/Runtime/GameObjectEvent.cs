using System;
using UnityEngine;
using UnityEngine.Events;

namespace TheCursedBroom.Player {
    [Serializable]
    class GameObjectEvent : UnityEvent<GameObject> {
    }
}