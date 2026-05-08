using StreetFighter.Audio;
using StreetFighter.Core;
using StreetFighter.Data;
using StreetFighter.Input;
using UnityEngine;
using CameraNamespace = StreetFighter.Camera;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Drives player movement, sprint, jump and camera-relative translation.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private CharacterProfileData profile = null;

        [SerializeField]
        private CameraNamespace.ThirdPersonCameraRig cameraRig = null;

        [SerializeField]
        private CharacterAnimationController animationController = null;

        [SerializeField]
        private AudioBank footstepBank = null;

        private CharacterController controller;
        private IInputService inputService;
        private Vector3 velocity;
        private bool jumpRequested;
        private const float GroundCastDistance = 0.2f;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            inputService = ServiceLocator.Resolve<IInputService>();
        }

        private void Update()
        {
            if (inputService == null || profile == null)
            {
                return;
            }

            UpdateLook();
            UpdateMovement();
            UpdateAnimation();
        }

        private void UpdateLook()
        {
            cameraRig?.UpdateLook(inputService.Look * profile.MovementSpeed * 0.02f);
        }

        private void UpdateMovement()
        {
            var moveInput = inputService.Move;
            var moveVector = Vector3.zero;
            var forward = cameraRig != null ? cameraRig.transform.forward : transform.forward;
            var right = cameraRig != null ? cameraRig.transform.right : transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            moveVector = (forward * moveInput.y + right * moveInput.x).normalized;
            var currentSpeed = inputService.SprintHeld && moveVector.sqrMagnitude > 0.01f ? profile.SprintSpeed : profile.MovementSpeed;
            var desiredVelocity = moveVector * currentSpeed;
            velocity.x = desiredVelocity.x;
            velocity.z = desiredVelocity.z;

            var grounded = IsGrounded();
            if (grounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            if (inputService.JumpPressed && grounded)
            {
                jumpRequested = true;
            }

            if (jumpRequested && grounded)
            {
                velocity.y = Mathf.Sqrt(profile.JumpHeight * -2f * profile.Gravity);
                jumpRequested = false;
            }

            velocity.y += profile.Gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void UpdateAnimation()
        {
            if (animationController == null)
            {
                return;
            }

            var moveSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            animationController.SetMovement(moveSpeed, inputService.SprintHeld);
            animationController.SetJumpState(!IsGrounded());
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(transform.position + Vector3.down * (controller.height / 2f - controller.radius), controller.radius + GroundCastDistance, LayerMask.GetMask("Default"));
        }
    }
}
