using System;
using UnityEngine.InputSystem;

namespace StreetFighter.Input
{
    /// <summary>
    /// Encapsulates the player input actions used by the runtime input service.
    /// </summary>
    public class PlayerInputActions : IDisposable
    {
        private readonly InputActionMap actionMap;

        public InputAction Move { get; }
        public InputAction Look { get; }
        public InputAction Jump { get; }
        public InputAction Sprint { get; }
        public InputAction Attack { get; }
        public InputAction Block { get; }

        public PlayerInputActions()
        {
            actionMap = new InputActionMap("Player");

            Move = actionMap.AddAction("Move", binding: "<Gamepad>/leftStick");
            Look = actionMap.AddAction("Look", binding: "<Mouse>/delta");
            Jump = actionMap.AddAction("Jump", binding: "<Keyboard>/space");
            Sprint = actionMap.AddAction("Sprint", binding: "<Keyboard>/leftShift");
            Attack = actionMap.AddAction("Attack", binding: "<Mouse>/leftButton");
            Block = actionMap.AddAction("Block", binding: "<Mouse>/rightButton");

            Move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            Look.AddBinding("<Gamepad>/rightStick");
            Jump.AddBinding("<Gamepad>/buttonSouth");
            Sprint.AddBinding("<Gamepad>/leftStickPress");
            Attack.AddBinding("<Gamepad>/buttonWest");
            Block.AddBinding("<Gamepad>/buttonEast");
        }

        public void Enable()
        {
            actionMap.Enable();
        }

        public void Disable()
        {
            actionMap.Disable();
        }

        public void Dispose()
        {
            actionMap.Dispose();
        }
    }
}
