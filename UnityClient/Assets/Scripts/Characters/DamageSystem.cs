using StreetFighter.Combat;
using StreetFighter.Core;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 2: DamageSystem handles final damage calculation, combo scaling, critical hits, and armor penetration.
    /// </summary>
    public sealed class DamageSystem : MonoBehaviour
    {
        [Header("Damage Scaling")]
        [SerializeField]
        private float comboMultiplierPerHit = 0.1f;

        [SerializeField]
        private float maxComboMultiplier = 3f;

        [SerializeField]
        private float guardBreakThreshold = 50f;

        [Header("Hit Effects")]
        [SerializeField]
        private float baseKnockback = 3f;

        [SerializeField]
        private float knockbackMultiplierPerCombo = 0.3f;

        private int comboCount = 0;
        private IEventBus eventBus;

        private void Awake()
        {
            eventBus = ServiceLocator.Resolve<IEventBus>();
        }

        /// <summary>
        /// Calculates and applies damage from an attack to a target.
        /// </summary>
        public DamageResult ApplyDamage(CombatMoveData attackerMove, CharacterStats attackerStats, CharacterStats defenderStats, bool isBlocked)
        {
            var attackDef = attackerMove as AttackDefinition;
            if (attackDef == null)
                return new DamageResult();

            float rawDamage = attackDef.BaseDamage;

            // Combo scaling
            float comboMultiplier = 1f + (comboCount * comboMultiplierPerHit);
            comboMultiplier = Mathf.Min(comboMultiplier, maxComboMultiplier);
            float scaledDamage = rawDamage * comboMultiplier;

            // Armor reduction
            float armorReduction = Mathf.Max(0, defenderStats.Armor - attackDef.ArmorPenetration);
            float finalDamage = Mathf.Max(1f, scaledDamage - armorReduction);

            // Apply guard damage if blocked
            if (isBlocked)
            {
                float guardDamage = finalDamage * attackDef.GuardDamageMultiplier;
                defenderStats.TakeGuardDamage(guardDamage);

                // Guard break check
                if (defenderStats.GuardHealth <= 0f)
                {
                    return new DamageResult
                    {
                        DamageDealt = 0f,
                        IsGuardBroken = true,
                        Knockback = baseKnockback * 2f,
                        HitReaction = HitReaction.Stagger
                    };
                }

                return new DamageResult
                {
                    DamageDealt = 0f,
                    IsBlocked = true,
                    Knockback = baseKnockback * 0.5f,
                    HitReaction = HitReaction.Block
                };
            }

            // Apply health damage
            defenderStats.TakeDamage(finalDamage);

            // Knockback calculation
            float knockback = baseKnockback + (comboCount * knockbackMultiplierPerCombo);
            knockback *= attackDef.KnockbackMultiplier;

            // Determine hit reaction
            var hitReaction = DetermineHitReaction(attackDef, finalDamage, defenderStats);

            // Publish damage event
            eventBus?.Publish(new CombatHitEvent
            {
                Attacker = attackerStats.gameObject,
                Defender = defenderStats.gameObject,
                Move = attackerMove,
                DamageDealt = finalDamage,
                IsBlocked = false,
                IsGuardBroken = false,
                Knockback = knockback,
                HitReaction = hitReaction,
                ComboCount = comboCount
            });

            return new DamageResult
            {
                DamageDealt = finalDamage,
                Knockback = knockback,
                HitReaction = hitReaction
            };
        }

        /// <summary>
        /// Increments the combo counter.
        /// </summary>
        public void IncrementCombo()
        {
            comboCount++;
        }

        /// <summary>
        /// Resets the combo counter.
        /// </summary>
        public void ResetCombo()
        {
            comboCount = 0;
        }

        private static HitReaction DetermineHitReaction(AttackDefinition attack, float damage, CharacterStats defender)
        {
            if (damage >= defender.MaxHealth * 0.25f)
                return HitReaction.Stun;
            if (attack.KnockbackMultiplier > 1.5f)
                return HitReaction.Knockback;
            if (attack.BaseDamage > 15f)
                return HitReaction.Stagger;

            return HitReaction.Light;
        }
    }

    public struct DamageResult
    {
        public float DamageDealt;
        public bool IsBlocked;
        public bool IsGuardBroken;
        public float Knockback;
        public HitReaction HitReaction;
    }
}
