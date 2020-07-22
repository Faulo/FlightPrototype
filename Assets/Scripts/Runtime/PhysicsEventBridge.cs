using System.Collections.Generic;
using UnityEngine;

namespace TheCursedBroom {
    public class PhysicsEventBridge : MonoBehaviour {
        PhysicsEventEmitter emitter;

        [Header("Collisions")]
        [SerializeField, Range(0, 10)]
        float onCollisionImpulseThreshold = 0;
        [SerializeField]
        GameObjectEvent onCollisionEnter = default;
        [SerializeField]
        GameObjectEvent onCollisionExit = default;

        [Header("Triggers")]
        [SerializeField]
        GameObjectEvent onTriggerEnter = default;
        [SerializeField]
        GameObjectEvent onTriggerExit = default;

        void Awake() {
            emitter = GetComponentInParent<PhysicsEventEmitter>();
        }
        void OnEnable() {
            emitter.onCollisionEnter += CollisionEnterListener;
            emitter.onCollisionExit += CollisionExitListener;
            emitter.onTriggerEnter += TriggerEnterListener;
            emitter.onTriggerExit += TriggerExitListener;
        }
        void OnDisable() {
            emitter.onCollisionEnter -= CollisionEnterListener;
            emitter.onCollisionExit -= CollisionExitListener;
            emitter.onTriggerEnter -= TriggerEnterListener;
            emitter.onTriggerExit -= TriggerExitListener;
        }
        void CollisionEnterListener(Collision2D collision) {
            if (onCollisionEnter.GetPersistentEventCount() > 0) {
                foreach (var tempObject in CreateContactObjects(collision)) {
                    onCollisionEnter.Invoke(tempObject);
                }
            }
        }
        void CollisionExitListener(Collision2D collision) {
            if (onCollisionExit.GetPersistentEventCount() > 0) {
                foreach (var tempObject in CreateContactObjects(collision)) {
                    onCollisionExit.Invoke(tempObject);
                }
            }
        }
        IEnumerable<GameObject> CreateContactObjects(Collision2D collision) {
            var contacts = collision.contacts;
            for (int i = 0; i < contacts.Length; i++) {
                if (contacts[i].normalImpulse >= onCollisionImpulseThreshold) {
                    var tempObject = new GameObject();
                    tempObject.transform.position = contacts[i].point;
                    tempObject.transform.rotation = AngleUtil.DirectionalRotation(contacts[i].normal);
                    Destroy(tempObject, 0);
                    yield return tempObject;
                }
            }
        }
        void TriggerEnterListener(Collider2D collider) {
            onTriggerEnter.Invoke(collider.gameObject);
        }
        void TriggerExitListener(Collider2D collider) {
            onTriggerExit.Invoke(collider.gameObject);
        }
    }
}