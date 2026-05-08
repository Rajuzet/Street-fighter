using Cinemachine;
using UnityEngine;

namespace StreetFighter.Camera
{
    /// <summary>
    /// Controls the third-person camera rig and smooth camera rotation.
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public sealed class ThirdPersonCameraRig : MonoBehaviour
    {
        [SerializeField]
        private Transform followTarget = null;

        [SerializeField]
        private float rotationSmoothTime = 0.08f;

        [SerializeField]
        private float minPitch = -35f;

        [SerializeField]
        private float maxPitch = 60f;

        [SerializeField]
        private float distance = 4f;

        [SerializeField]
        private Vector3 offset = new(0f, 1.8f, 0f);

        private CinemachineVirtualCamera virtualCamera;
        private CinemachineComposer composer;
        private Vector2 currentRotation;
        private Vector2 rotationVelocity;

        private void Awake()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (followTarget != null)
            {
                virtualCamera.Follow = followTarget;
                virtualCamera.LookAt = followTarget;
            }
        }

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            var targetPosition = followTarget.position + offset;
            transform.position = targetPosition - transform.forward * distance;
        }

        /// <summary>
        /// Updates the camera rotation from player input.
        /// </summary>
        /// <param name="lookInput">The current look input vector.</param>
        public void UpdateLook(Vector2 lookInput)
        {
            currentRotation.x = Mathf.SmoothDamp(currentRotation.x, currentRotation.x + lookInput.x, ref rotationVelocity.x, rotationSmoothTime);
            currentRotation.y = Mathf.SmoothDamp(currentRotation.y, Mathf.Clamp(currentRotation.y - lookInput.y, minPitch, maxPitch), ref rotationVelocity.y, rotationSmoothTime);

            transform.localRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);

            if (composer != null)
            {
                composer.m_TrackedObjectOffset.y = offset.y;
            }
        }
    }
}
