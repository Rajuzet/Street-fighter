using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Tracks character health, armor, and damage mitigation.
    /// </summary>
    public sealed class CharacterStats : MonoBehaviour
    {
        [SerializeField]
        private float maxHealth = 100f;

        [SerializeField]
        private float armor = 0f;

        [SerializeField]
        private float damageResistance = 0f; // 0-1 scale (0 = no resistance, 1 = immune)

        [SerializeField]
        private float guardHealth = 50f;

        private float currentHealth;
        private float currentGuard;
        private bool isAlive = true;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthRatio => currentHealth / maxHealth;
        public float Armor => armor;
        public float DamageResistance => damageResistance;
        public float CurrentGuard => currentGuard;
        public bool IsAlive => isAlive;

        public event System.Action<float> HealthChanged;
        public event System.Action<float> GuardChanged;
        public event System.Action CharacterDied;

        private void Start()
        {
            currentHealth = maxHealth;
            currentGuard = guardHealth;
        }

        /// <summary>
        /// Applies raw damage to health after armor/resistance calculation.
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (!isAlive || damage <= 0f) return;

            currentHealth -= damage;
            HealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isAlive = false;
                CharacterDied?.Invoke();
            }
        }

        /// <summary>
        /// Applies damage to guard meter (used when blocking).
        /// </summary>
        public void TakeGuardDamage(float damage)
        {
            if (damage <= 0f) return;

            currentGuard -= damage;
            GuardChanged?.Invoke(currentGuard);

            if (currentGuard <= 0f)
            {
                currentGuard = 0f;
                // Guard break - could trigger stun or other effect
            }
        }

        /// <summary>
        /// Restores health up to maximum.
        /// </summary>
        public void Heal(float amount)
        {
            if (!isAlive || amount <= 0f) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            HealthChanged?.Invoke(currentHealth);
        }

        /// <summary>
        /// Restores guard meter up to maximum.
        /// </summary>
        public void RestoreGuard(float amount)
        {
            if (amount <= 0f) return;
            currentGuard = Mathf.Min(currentGuard + amount, guardHealth);
            GuardChanged?.Invoke(currentGuard);
        }

        /// <summary>
        /// Resets character to full health and guard.
        /// </summary>
        public void ResetStats()
        {
            currentHealth = maxHealth;
            currentGuard = guardHealth;
            isAlive = true;
            HealthChanged?.Invoke(currentHealth);
            GuardChanged?.Invoke(currentGuard);
        }
    }
}
