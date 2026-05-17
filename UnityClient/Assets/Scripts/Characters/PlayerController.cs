using StreetFighter.Audio;
using StreetFighter.Core;
using StreetFighter.Data;
using StreetFighter.Input;
using UnityEngine;
using CameraNamespace = StreetFighter.Camera;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Drives player movement, sprint, jump, camera-relative translation, and parkour.
    /// Phase 2: Integrated ParkourSystem for vault, ledge grab, and slide.
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

        [Header("Parkour")]
        [SerializeField]
        private ParkourSystem parkourSystem = null;

        private CharacterController controller;
        private IInputService inputService;
        private Vector3 velocity;
        private bool jumpRequested;
        private const float GroundCastDistance = 0.2f;

        public bool CanMove => (parkourSystem == null || !parkourSystem.IsParkouring) && !IsHitReacting;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            inputService = ServiceLocator.Resolve<IInputService>();

            // Auto-find parkour system if not assigned
            if (parkourSystem == null)
                parkourSystem = GetComponent<ParkourSystem>();
        }

        private void Update()
        {
            if (inputService == null || profile == null)
            {
                return;
            }

            UpdateLook();

            if (CanMove)
            {
                UpdateMovement();
            }

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

            // Phase 2: Parkour animation states
            if (parkourSystem != null)
            {
                animationController.SetVaulting(false); // Set by ParkourSystem directly
                animationController.SetLedgeGrabbing(false);
                animationController.SetSliding(false);
            }
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(transform.position + Vector3.down * (controller.height / 2f - controller.radius), controller.radius + GroundCastDistance, LayerMask.GetMask("Default"));
        }

        private bool IsHitReacting
        {
            get
            {
                var hitReaction = GetComponent<HitReactionSystem>();
                return hitReaction != null && hitReaction.IsReacting;
            }
        }

        public void ApplyExternalForce(Vector3 force)
        {
            velocity += force;
        }
    }
}
