using System;
using Cinemachine;
using UnityEngine;

namespace SESCO.InGame.Camera
{
    public class CameraFollowTarget : MonoBehaviour
    {
        private void OnEnable()
        {
            var cinemachine_virtual_camera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            if (cinemachine_virtual_camera == null) return;
            cinemachine_virtual_camera.Follow = transform;
        }

        private void OnDisable()
        {
            var cinemachine_virtual_camera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            if (cinemachine_virtual_camera == null || cinemachine_virtual_camera.Follow != transform) return;
            cinemachine_virtual_camera.Follow = null;
        }
    }
}