using System.Linq;
using UnityEngine;

public class GroundedCheck : MonoBehaviour {
    [Header("Grounded Check")]
    [SerializeField, Range(0, 1)]
    float groundedRadius = 1;
    [SerializeField]
    LayerMask groundedLayers = default;

    public bool IsGrounded(GameObject context) {
        return Physics2D
            .OverlapCircleAll(transform.position, groundedRadius, groundedLayers)
            .Any(collider => collider.gameObject != context);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, groundedRadius);
    }
}
