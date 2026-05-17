using System.Collections;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 2: HitReactionSystem handles knockback, stagger, stun, and other hit reactions.
    /// Integrates with CharacterAnimationController and PlayerController.
    /// </summary>
    public sealed class HitReactionSystem : MonoBehaviour
    {
        [Header("Hit Reaction Settings")]
        [SerializeField]
        private float staggerDuration = 0.5f;

        [SerializeField]
        private float stunDuration = 1.2f;

        [SerializeField]
        private float knockbackDuration = 0.3f;

        [SerializeField]
        private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        private CharacterAnimationController animationController;
        private PlayerController playerController;
        private CharacterController characterController;
        private bool isReacting;
        private HitReaction currentReaction;

        public bool IsReacting => isReacting;
        public HitReaction CurrentReaction => currentReaction;

        private void Awake()
        {
            animationController = GetComponent<CharacterAnimationController>();
            playerController = GetComponent<PlayerController>();
            characterController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Triggers a hit reaction on this character.
        /// </summary>
        public void TriggerReaction(HitReaction reaction, Vector3 knockbackDirection, float knockbackForce)
        {
            if (isReacting && reaction <= currentReaction)
                return; // Don't override stronger reactions with weaker ones

            StopAllCoroutines();
            isReacting = true;
            currentReaction = reaction;

            switch (reaction)
            {
                case HitReaction.Light:
                    StartCoroutine(HandleLightReaction());
                    break;
                case HitReaction.Stagger:
                    StartCoroutine(HandleStaggerReaction());
                    break;
                case HitReaction.Knockback:
                    StartCoroutine(HandleKnockbackReaction(knockbackDirection, knockbackForce));
                    break;
                case HitReaction.Stun:
                    StartCoroutine(HandleStunReaction());
                    break;
                case HitReaction.Block:
                    StartCoroutine(HandleBlockReaction());
                    break;
                case HitReaction.Parry:
                    StartCoroutine(HandleParryReaction());
                    break;
                default:
                    isReacting = false;
                    break;
            }
        }

        private IEnumerator HandleLightReaction()
        {
            animationController?.TriggerHitReact();
            yield return new WaitForSeconds(0.2f);
            isReacting = false;
            currentReaction = HitReaction.None;
        }

        private IEnumerator HandleStaggerReaction()
        {
            animationController?.SetStaggered(true);
            yield return new WaitForSeconds(staggerDuration);
            animationController?.SetStaggered(false);
            isReacting = false;
            currentReaction = HitReaction.None;
        }

        private IEnumerator HandleKnockbackReaction(Vector3 direction, float force)
        {
            animationController?.SetStaggered(true);
            float timer = 0f;
            
            while (timer < knockbackDuration)
            {
                timer += Time.deltaTime;
                float t = timer / knockbackDuration;
                float curveValue = knockbackCurve.Evaluate(t);
                
                Vector3 knockback = direction * force * curveValue;
                characterController?.Move(knockback * Time.deltaTime);
                playerController?.ApplyExternalForce(knockback);
                
                yield return null;
            }

            animationController?.SetStaggered(false);
            isReacting = false;
            currentReaction = HitReaction.None;
        }

        private IEnumerator HandleStunReaction()
        {
            animationController?.SetStunned(true);
            yield return new WaitForSeconds(stunDuration);
            animationController?.SetStunned(false);
            isReacting = false;
            currentReaction = HitReaction.None;
        }

        private IEnumerator HandleBlockReaction()
        {
            animationController?.TriggerParry();
            yield return new WaitForSeconds(0.15f);
            isReacting = false;
            currentReaction = HitReaction.None;
        }

        private IEnumerator HandleParryReaction()
        {
            animationController?.TriggerParry();
            yield return new WaitForSeconds(0.3f);
            isReacting = false;
            currentReaction = HitReaction.None;
        }

        /// <summary>
        /// Immediately cancels any active reaction.
        /// </summary>
        public void CancelReaction()
        {
            StopAllCoroutines();
            animationController?.SetStaggered(false);
            animationController?.SetStunned(false);
            isReacting = false;
            currentReaction = HitReaction.None;
        }
    }

    public enum HitReaction
    {
        None,
        Light,
        Block,
        Parry,
        Stagger,
        Knockback,
        Stun
    }
}
