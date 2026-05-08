namespace StreetFighter.Characters
{
    /// <summary>
    /// Animator parameter identifiers used by the player animation controller.
    /// </summary>
    public static class AnimationIds
    {
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
        public static readonly int IsAirborne = Animator.StringToHash("IsAirborne");
        public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
        public static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
    }
}
