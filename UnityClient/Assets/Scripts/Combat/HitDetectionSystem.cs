using System;
using UnityEngine;
using StreetFighter.Core;
using StreetFighter.Data;
using StreetFighter.Combat;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Frame-based hit detection using dynamic colliders for startup/active/recovery phases.
    /// Integrates with AttackDefinition frame data.
    /// Multiplayer-safe with RPC hooks.
    /// </summary>
    public class HitDetectionSystem : MonoBehaviour
    {
        [SerializeField] private AttackDefinition currentAttack;
        private Collider hitCollider;
        private BoxCollider attackCollider;
        private bool isActivePhase;
        private float frameTimer;

        // Reused hitbox to avoid Add/Destroy churn.
        private bool colliderInitialized;

        private IEventBus eventBus;

        public void StartAttack(AttackDefinition attack)
        {
            if (attack == null) return;
            currentAttack = attack;

            // Prevent overlapping attack lifecycles corrupting state/collider enable.
            if (attackLifecycleCoroutine != null)
            {
                StopCoroutine(attackLifecycleCoroutine);
                attackLifecycleCoroutine = null;
            }

            EnsureHitbox();
            attackCollider.enabled = false;
            isActivePhase = false;
            frameTimer = 0f;
            attackLifecycleCoroutine = StartCoroutine(AttackLifecycle());
        }



        private Coroutine attackLifecycleCoroutine;

        private IEnumerator AttackLifecycle()
        {
            // Startup
            yield return new WaitForSeconds(currentAttack.StartupFrames / 60f);

            // Active - enable collider
            isActivePhase = true;
            eventBus?.Publish(new AttackActivatedEvent(currentAttack, gameObject));
            if (attackCollider != null) attackCollider.enabled = true;
            yield return new WaitForSeconds(currentAttack.ActiveFrames / 60f);

            // Recovery
            isActivePhase = false;
            if (attackCollider != null) attackCollider.enabled = false;
            yield return new WaitForSeconds(currentAttack.RecoveryFrames / 60f);
            // Keep collider for reuse (no Destroy).

            attackLifecycleCoroutine = null;
        }


        private void EnsureHitbox()
        {
            if (!colliderInitialized)
            {
                attackCollider = gameObject.AddComponent<BoxCollider>();
                attackCollider.isTrigger = true;
                colliderInitialized = true;
            }

            attackCollider.center = currentAttack.HitboxOffset;
            attackCollider.size = currentAttack.HitboxSize;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!isActivePhase || other.CompareTag("Player")) return;

            bool isBlocked = other.GetComponent<PlayerCombatController>()?.IsBlocking ?? false;
            float damage = isBlocked ? currentAttack.GuardDamage : currentAttack.BaseDamage;

            Vector3 hitPosition = other.ClosestPoint(transform.position);
            Vector3 toTarget = (other.bounds.center - transform.position);
            if (toTarget.sqrMagnitude < 0.0001f)
                toTarget = other.transform.position - transform.position;
            Vector3 hitNormal = toTarget.sqrMagnitude < 0.0001f ? Vector3.forward : (-toTarget.normalized);

            eventBus?.Publish(new HitEvent(currentAttack, gameObject, other.gameObject,
                hitPosition, hitNormal, damage, isBlocked));


            // Multiplayer RPC hook
            // photonView.RPC("OnHitConfirmed", RpcTarget.All, damage, isBlocked);
        }

        private void CleanupHitbox()
        {
            // No-op in Phase 2B.1: collider is reused.
        }


        private void Awake()
        {
            eventBus = ServiceLocator.Resolve<IEventBus>();
        }
    }
}
