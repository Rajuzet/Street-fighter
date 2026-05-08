using StreetFighter.Input;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Bridges Unity Input System actions to gameplay services.
    /// </summary>
    public class InputManager : IInputService
    {
        private PlayerInputActions inputActions;

        /// <inheritdoc />
        public Vector2 Move { get; private set; }

        /// <inheritdoc />
        public Vector2 Look { get; private set; }

        /// <inheritdoc />
        public bool JumpPressed { get; private set; }

        /// <inheritdoc />
        public bool SprintHeld { get; private set; }

        /// <inheritdoc />
        public bool AttackPressed { get; private set; }

        /// <inheritdoc />
        public bool BlockHeld { get; private set; }

        /// <inheritdoc />
        public void Initialize()
        {
            inputActions = new PlayerInputActions();
            inputActions.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
            inputActions.Move.canceled += _ => Move = Vector2.zero;
            inputActions.Look.performed += ctx => Look = ctx.ReadValue<Vector2>();
            inputActions.Look.canceled += _ => Look = Vector2.zero;
            inputActions.Jump.performed += _ => JumpPressed = true;
            inputActions.Jump.canceled += _ => JumpPressed = false;
            inputActions.Sprint.performed += _ => SprintHeld = true;
            inputActions.Sprint.canceled += _ => SprintHeld = false;
            inputActions.Attack.performed += _ => AttackPressed = true;
            inputActions.Attack.canceled += _ => AttackPressed = false;
            inputActions.Block.performed += _ => BlockHeld = true;
            inputActions.Block.canceled += _ => BlockHeld = false;
            Enable();
        }

        /// <inheritdoc />
        public void Enable()
        {
            inputActions?.Enable();
        }

        /// <inheritdoc />
        public void Disable()
        {
            inputActions?.Disable();
        }
    }
}
