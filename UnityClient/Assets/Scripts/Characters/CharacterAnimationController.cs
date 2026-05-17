using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Wraps animation state updates and parameter access for the player character.
    /// Phase 2: Added kick, power, special, and parkour animation triggers.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class CharacterAnimationController : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Sets the movement blend values used by animation graphs.
        /// </summary>
        public void SetMovement(float speed, bool sprinting)
        {
            animator.SetFloat(AnimationIds.Speed, speed);
            animator.SetBool(AnimationIds.IsSprinting, sprinting);
        }

        /// <summary>
        /// Controls jump state for the animator.
        /// </summary>
        public void SetJumpState(bool isAirborne)
        {
            animator.SetBool(AnimationIds.IsAirborne, isAirborne);
        }

        /// <summary>
        /// Triggers the attack action in the animator.
        /// </summary>
        public void TriggerAttack()
        {
            animator.SetTrigger(AnimationIds.AttackTrigger);
        }

        /// <summary>
        /// Triggers a kick animation.
        /// </summary>
        public void TriggerKick()
        {
            animator.SetTrigger(AnimationIds.KickTrigger);
        }

        /// <summary>
        /// Triggers a power move animation.
        /// </summary>
        public void TriggerPower()
        {
            animator.SetTrigger(AnimationIds.PowerTrigger);
        }

        /// <summary>
        /// Triggers a special ability animation.
        /// </summary>
        public void TriggerSpecial()
        {
            animator.SetTrigger(AnimationIds.SpecialTrigger);
        }

        /// <summary>
        /// Triggers a throw/grab animation.
        /// </summary>
        public void TriggerThrow()
        {
            animator.SetTrigger(AnimationIds.ThrowTrigger);
        }

        /// <summary>
        /// Triggers the dodge/roll animation.
        /// </summary>
        public void TriggerDodge()
        {
            animator.SetTrigger(AnimationIds.DodgeTrigger);
        }

        /// <summary>
        /// Triggers the hit reaction animation.
        /// </summary>
        public void TriggerHitReact()
        {
            animator.SetTrigger(AnimationIds.HitReactTrigger);
        }

        /// <summary>
        /// Triggers the parry animation.
        /// </summary>
        public void TriggerParry()
        {
            animator.SetTrigger(AnimationIds.ParryTrigger);
        }

        /// <summary>
        /// Sets blocking state for the animator.
        /// </summary>
        public void SetBlocking(bool isBlocking)
        {
            animator.SetBool(AnimationIds.IsBlocking, isBlocking);
        }

        /// <summary>
        /// Sets stagger state for the animator.
        /// </summary>
        public void SetStaggered(bool isStaggered)
        {
            animator.SetBool(AnimationIds.IsStaggered, isStaggered);
        }

        /// <summary>
        /// Sets stun state for the animator.
        /// </summary>
        public void SetStunned(bool isStunned)
        {
            animator.SetBool(AnimationIds.IsStunned, isStunned);
        }

        /// <summary>
        /// Sets vaulting state for the animator.
        /// </summary>
        public void SetVaulting(bool isVaulting)
        {
            animator.SetBool(AnimationIds.IsVaulting, isVaulting);
        }

        /// <summary>
        /// Sets ledge grabbing state for the animator.
        /// </summary>
        public void SetLedgeGrabbing(bool isLedgeGrabbing)
        {
            animator.SetBool(AnimationIds.IsLedgeGrabbing, isLedgeGrabbing);
        }

        /// <summary>
        /// Sets sliding state for the animator.
        /// </summary>
        public void SetSliding(bool isSliding)
        {
            animator.SetBool(AnimationIds.IsSliding, isSliding);
        }

        /// <summary>
        /// Sets the current combo count for animation blending.
        /// </summary>
        public void SetComboCount(int count)
        {
            animator.SetInteger(AnimationIds.ComboCount, count);
        }
    }
}
