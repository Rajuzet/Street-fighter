using UnityEngine;
using StreetFighter.Combat;

namespace StreetFighter.Characters
{
    public class DamageReaction : MonoBehaviour
    {
        private IEventBus eventBus;

        private void Awake()
        {
            eventBus = ServiceLocator.Resolve<IEventBus>();
        }

        private void OnEnable()
        {
            eventBus?.Subscribe<Combat.HitEvent>(OnHit);
        }

        private void OnDisable()
        {
            eventBus?.Unsubscribe<Combat.HitEvent>(OnHit);
        }

        private void OnDestroy()
        {
            // Safety: ensure we never leave dangling subscriptions.
            eventBus?.Unsubscribe<Combat.HitEvent>(OnHit);
        }

        private void OnHit(Combat.HitEvent hit)
        {
            if (hit.Target != gameObject) return;

            var stamina = GetComponent<StaminaSystem>();
            stamina?.TryConsumeStamina(hit.IsBlocked ? 5f : 15f);

            HitStopManager.TriggerHitStop(hit.Attack.HitStopDuration);
            Combat.CameraShake.Shake(hit.Attack.CameraShakeMagnitude);

            KnockbackHandler.ApplyKnockback(gameObject, hit.Attack, Vector3.back);

            // Audio/VFX
            ServiceLocator.Resolve<IAudioService>()?.PlaySFX(hit.Attack.HitSoundKey);

            // Ragdoll toggle for heavy hits
            if (!hit.IsBlocked)
            {
                GetComponent<CharacterAnimationController>()?.TriggerHitReact();
            }
        }
    }
}

