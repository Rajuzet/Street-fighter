namespace StreetFighter.Combat
{
    /// <summary>
    /// Phase 2: Enum defining move categories for combo chaining rules.
    /// </summary>
    public enum MoveType
    {
        None,
        Punch,
        Kick,
        Power,
        Special,
        Throw,
        Counter
    }

    /// <summary>
    /// Phase 2: Static rules defining valid move type transitions for combo chains.
    /// </summary>
    public static class MoveTypeRules
    {
        /// <summary>
        /// Determines if a move type can chain into another move type.
        /// </summary>
        public static bool CanChainInto(MoveType from, MoveType to)
        {
            if (from == MoveType.None)
                return to != MoveType.None;

            if (from == MoveType.Power)
                return to == MoveType.Punch || to == MoveType.Kick || to == MoveType.Special || to == MoveType.Throw;

            if (from == MoveType.Special)
                return to == MoveType.Power || to == MoveType.Counter;

            if (from == MoveType.Throw)
                return to == MoveType.Punch;

            if (from == MoveType.Counter)
                return to == MoveType.Punch || to == MoveType.Kick || to == MoveType.Power || to == MoveType.Special;

            return to != MoveType.None && !(from == to && to == MoveType.Special);
        }

        /// <summary>
        /// Gets the default move type priority (higher = more restrictive chaining).
        /// </summary>
        public static int GetPriority(MoveType type)
        {
            return type switch
            {
                MoveType.None => 0,
                MoveType.Punch => 1,
                MoveType.Kick => 1,
                MoveType.Throw => 2,
                MoveType.Power => 3,
                MoveType.Special => 4,
                MoveType.Counter => 5,
                _ => 0
            };
        }
    }
}
