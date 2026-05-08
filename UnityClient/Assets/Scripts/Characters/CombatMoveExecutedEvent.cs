using StreetFighter.Data;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Event data raised when a combat move has been executed.
    /// </summary>
    public sealed class CombatMoveExecutedEvent
    {
        public CombatMoveExecutedEvent(CombatMoveData move)
        {
            Move = move;
        }

        public CombatMoveData Move { get; }
    }
}
