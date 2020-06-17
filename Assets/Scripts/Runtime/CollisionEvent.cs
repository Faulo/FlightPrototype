using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom {
    public class CollisionEvent : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        float impulseThreshold = 0;
        [SerializeField]
        GameObjectEvent onColliderEnter = default;

        void OnCollisionEnter2D(Collision2D collision) {
            collision.contacts.ForAll(Dispatch);
        }

        void Dispatch(ContactPoint2D contact) {
            if (contact.normalImpulse < impulseThreshold) {
                return;
            }
            var tempObject = new GameObject();
            tempObject.transform.position = contact.point;
            tempObject.transform.rotation = AngleUtil.DirectionalRotation(contact.normal);
            Debug.Log(tempObject.transform.rotation.eulerAngles);
            onColliderEnter.Invoke(tempObject);
            Destroy(tempObject, 0);
        }
    }
}