using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheCursedBroom {
    public class GroundedCheck : MonoBehaviour {
        [Header("Grounded Check")]
        [SerializeField]
        Vector2 groundedSize = Vector2.one;
        [SerializeField]
        LayerMask groundedLayers = default;

        public IReadOnlyList<Ground> GetGrounds() {
            return Physics2D
                .OverlapBoxAll(transform.position, groundedSize, groundedLayers)
                .Select(collider => collider.attachedRigidbody ? collider.attachedRigidbody.gameObject : collider.gameObject)
                .SelectMany(obj => obj.GetComponents<Ground>())
                .ToList();
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, groundedSize);
        }
    }
}