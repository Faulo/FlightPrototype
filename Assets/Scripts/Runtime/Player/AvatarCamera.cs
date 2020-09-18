using Cinemachine;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarCamera : MonoBehaviour {
        [Header("MonoBehaviour configuration")]
        [SerializeField]
        AvatarController observedAvatar = default;
        [SerializeField]
        CinemachineVirtualCamera targetCamera = default;

        [Header("Parameters")]
        [SerializeField, Range(0, 1)]
        float facingLookahead = 0;
        [SerializeField, Range(0, 100)]
        float groundedSize = 10;
        [SerializeField, Range(0, 100)]
        float airborneSize = 10;
        [SerializeField, Range(0, 10)]
        float groundedChangeDuration = 1;
        [SerializeField, Range(0, 10)]
        float airborneChangeDuration = 1;
        float sizeChangeVelocity = 0;
        float cameraSize {
            get => targetCamera.m_Lens.OrthographicSize;
            set => targetCamera.m_Lens.OrthographicSize = value;
        }

        CinemachineFramingTransposer transposer;

        void Start() {
            transposer = targetCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        void OnEnable() {
            observedAvatar.onTeleport += TeleportListener;
        }
        void OnDisable() {
            observedAvatar.onTeleport -= TeleportListener;
        }
        void TeleportListener(GameObject context, Vector3 deltaPosition) {
            targetCamera.OnTargetObjectWarped(context.transform, deltaPosition);
        }

        void Update() {
            transposer.m_ScreenX = 0.5f - (facingLookahead * observedAvatar.facing);
            if (observedAvatar.broom.isFlying) {
                ChangeCameraSize(airborneSize, airborneChangeDuration);
            } else {
                ChangeCameraSize(groundedSize, groundedChangeDuration);
            }
        }
        void ChangeCameraSize(float size, float duration) {
            cameraSize = Mathf.SmoothDamp(cameraSize, size, ref sizeChangeVelocity, duration);
        }
    }
}