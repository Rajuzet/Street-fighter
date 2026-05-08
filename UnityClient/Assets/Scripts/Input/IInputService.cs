using UnityEngine;

namespace StreetFighter.Input
{
    /// <summary>
    /// Defines the interface for input providers used by gameplay systems.
    /// </summary>
    public interface IInputService
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool JumpPressed { get; }
        bool SprintHeld { get; }
        bool AttackPressed { get; }
        bool BlockHeld { get; }
        void Initialize();
        void Enable();
        void Disable();
    }
}
