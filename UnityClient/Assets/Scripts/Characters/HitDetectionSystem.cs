using System.Collections;
using StreetFighter.Combat;
using StreetFighter.Core;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Detects hitbox collisions during active attack frames and applies damage to valid targets.
    /// </summary>
    public sealed class HitDetectionSystem : MonoBehaviour
    {
        [SerializeField]
        private Transform hitboxOrigin;

        [SerializeField]
        private LayerMask targetLayers;

        private AttackDefinition currentAttack;
        private bool isDetecting;
        private readonly Collider[] hitBuffer = new Collider[16];

        private void Awake()
        {
            if (hitboxOrigin == null)
                hitboxOrigin = transform;
            if (targetLayers == 0)
                targetLayers = LayerMask.GetMask("Default");
        }

        /// <summary>
        /// Begins hit detection for the specified attack definition.
        /// </summary>
        public void StartAttack(AttackDefinition attack)
        {
            currentAttack = attack;
            if (currentAttack == null) return;

            float startupSeconds = currentAttack.StartupFrames / 60f;
            float activeSeconds = currentAttack.ActiveFrames / 60f;

            StartCoroutine(AttackWindowCoroutine(startupSeconds, activeSeconds));
        }

        private IEnumerator AttackWindowCoroutine(float startup, float active)
        {
            // Startup frames - no hit detection
            if (startup > 0f)
                yield return new WaitForSeconds(startup);

            // Active frames - detect hits
            isDetecting = true;
            float elapsed = 0f;
            float interval = 0.016f; // Check every frame (~60fps)

            while (elapsed < active && isDetecting)
            {
                DetectHits();
                elapsed += interval;
                yield return new WaitForSeconds(interval);
            }

            isDetecting = false;
            currentAttack = null;
        }

        private void DetectHits()
        {
            if (currentAttack == null) return;

            Vector3 center = hitboxOrigin.position + hitboxOrigin.TransformDirection(currentAttack.HitboxOffset);
            int count = Physics.OverlapBoxNonAlloc(
                center,
                currentAttack.HitboxSize * 0.5f,
                hitBuffer,
                hitboxOrigin.rotation,
                targetLayers
            );

            for (int i = 0; i < count; i++)
            {
                var target = hitBuffer[i];
                if (target == null || target.gameObject == gameObject)
                    continue;

                var targetStats = target.GetComponent<CharacterStats>();
                var targetReaction = target.GetComponent<HitReactionSystem>();
                var targetCombat = target.GetComponent<PlayerCombatController>();

                // Skip if target is blocking and attack is blockable
                if (targetCombat != null && targetCombat.IsBlocking && currentAttack.IsBlockable)
                {
                    // Apply guard damage instead
                    targetStats?.TakeGuardDamage(currentAttack.GuardDamage);
                    ServiceLocator.Resolve<IAudioService>()?.PlaySound("block_impact", target.transform.position);
                    continue;
                }

                // Calculate damage
                float damage = currentAttack.BaseDamage;
                var damageSystem = GetComponent<DamageSystem>();
                if (damageSystem != null && targetStats != null)
                {
                    damage = damageSystem.CalculateDamage(damage, targetStats);
                }

                // Apply damage
                targetStats?.TakeDamage(damage);

                // Apply hit reaction (knockback)
                if (targetReaction != null)
                {
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    direction.y = 0.3f; // Slight upward component
                    targetReaction.ApplyKnockback(direction, currentAttack.KnockbackForce, currentAttack.KnockbackDuration);
                }

                // Hit stop
                HitStopManager.TriggerHitStop(currentAttack.HitStopDuration);

                // Camera shake
                CameraShake.Shake(currentAttack.CameraShakeMagnitude);

                // Play hit sound
                ServiceLocator.Resolve<IAudioService>()?.PlaySound(
                    string.IsNullOrEmpty(currentAttack.HitSoundKey) ? "hit_generic" : currentAttack.HitSoundKey,
                    target.transform.position
                );

                // Publish hit event
                var eventBus = ServiceLocator.Resolve<IEventBus>();
                eventBus?.Publish(new CombatHitEvent(
                    attacker: gameObject,
                    target: target.gameObject,
                    damage: damage,
                    attack: currentAttack,
                    isBlocked: false
                ));
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (currentAttack == null || !isDetecting) return;
            Gizmos.color = Color.red;
            Vector3 center = hitboxOrigin != null
                ? hitboxOrigin.position + hitboxOrigin.TransformDirection(currentAttack.HitboxOffset)
                : transform.position + currentAttack.HitboxOffset;
            Gizmos.matrix = Matrix4x4.TRS(center, hitboxOrigin != null ? hitboxOrigin.rotation : Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, currentAttack.HitboxSize);
        }
    }

    /// <summary>
    /// Event raised when a combat hit is confirmed.
    /// </summary>
    public sealed class CombatHitEvent
    {
        public GameObject Attacker { get; }
        public GameObject Target { get; }
        public float Damage { get; }
        public AttackDefinition Attack { get; }
        public bool IsBlocked { get; }

        public CombatHitEvent(GameObject attacker, GameObject target, float damage, AttackDefinition attack, bool isBlocked)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
            Attack = attack;
            IsBlocked = isBlocked;
        }
    }
}
