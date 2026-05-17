namespace StreetFighter.Characters
{
    /// <summary>
    /// Animator parameter identifiers used by the player animation controller.
    /// Phase 2: Added kick, power, special, and parkour animation triggers.
    /// </summary>
    public static class AnimationIds
    {
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
        public static readonly int IsAirborne = Animator.StringToHash("IsAirborne");
        public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
        public static readonly int IsBlocking = Animator.StringToHash("IsBlocking");

        // Phase 2: Combat move types
        public static readonly int KickTrigger = Animator.StringToHash("KickTrigger");
        public static readonly int PowerTrigger = Animator.StringToHash("PowerTrigger");
        public static readonly int SpecialTrigger = Animator.StringToHash("SpecialTrigger");
        public static readonly int ThrowTrigger = Animator.StringToHash("ThrowTrigger");

        // Phase 2: Hit reactions
        public static readonly int HitReactTrigger = Animator.StringToHash("HitReactTrigger");
        public static readonly int ParryTrigger = Animator.StringToHash("ParryTrigger");
        public static readonly int DodgeTrigger = Animator.StringToHash("DodgeTrigger");
        public static readonly int IsStaggered = Animator.StringToHash("IsStaggered");
        public static readonly int IsStunned = Animator.StringToHash("IsStunned");

        // Phase 2: Parkour
        public static readonly int IsVaulting = Animator.StringToHash("IsVaulting");
        public static readonly int IsLedgeGrabbing = Animator.StringToHash("IsLedgeGrabbing");
        public static readonly int IsSliding = Animator.StringToHash("IsSliding");

        // Phase 2: Combo
        public static readonly int ComboCount = Animator.StringToHash("ComboCount");
    }
}
