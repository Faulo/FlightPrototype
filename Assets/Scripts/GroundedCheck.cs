using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheCursedBroom {
    public class GroundedCheck : MonoBehaviour {
        [Header("Grounded Check")]
        [SerializeField, Range(0, 1)]
        float groundedRadius = 1;
        [SerializeField]
        LayerMask groundedLayers = default;

        public IReadOnlyList<Ground> GetGrounds() {
            return Physics2D
                .OverlapCircleAll(transform.position, groundedRadius, groundedLayers)
                .Select(collider => collider.attachedRigidbody ? collider.attachedRigidbody.gameObject : collider.gameObject)
                .SelectMany(obj => obj.GetComponents<Ground>())
                .ToList();
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, groundedRadius);
        }
    }
}