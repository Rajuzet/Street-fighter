using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Wraps animation state updates and parameter access for the player character.
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

        public void TriggerDodge()
        {
            animator.SetTrigger("DodgeTrigger");
        }

        public void TriggerHitReact()
        {
            animator.SetTrigger("HitReactTrigger");
        }

        public void TriggerParry()
        {
            animator.SetTrigger("ParryTrigger");
        }

        /// <summary>
        /// Sets blocking state for the animator.
        /// </summary>
        public void SetBlocking(bool isBlocking)
        {
            animator.SetBool(AnimationIds.IsBlocking, isBlocking);
        }
    }
}
