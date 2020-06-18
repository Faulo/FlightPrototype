using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TheCursedBroom.Player {
    public class AvatarCamera : MonoBehaviour {
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

        CinemachineFramingTransposer transposer;

        void Start() {
            transposer = targetCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        void Update() {
            transposer.m_ScreenX = 0.5f - (facingLookahead * observedAvatar.facing);
            float targetSize = observedAvatar.isFlying
                ? airborneSize
                : groundedSize;
            float duration = observedAvatar.isFlying
                ? airborneChangeDuration
                : groundedChangeDuration;
            targetCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(targetCamera.m_Lens.OrthographicSize, targetSize, ref sizeChangeVelocity, duration);
        }
    }
}