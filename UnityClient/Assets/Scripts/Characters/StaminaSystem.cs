using System;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 2: StaminaSystem handles stamina for blocking, sprinting, attacking, and dodging.
    /// Integrates with CombatSystemManager and PlayerController.
    /// </summary>
    public sealed class StaminaSystem : MonoBehaviour
    {
        [Header("Stamina Settings")]
        [SerializeField]
        private float maxStamina = 100f;

        [SerializeField]
        private float regenRate = 25f;

        [SerializeField]
        private float regenDelay = 1f;

        [Header("Stamina Costs")]
        [SerializeField]
        private float blockCostPerSecond = 10f;

        [SerializeField]
        private float dodgeCost = 25f;

        [SerializeField]
        private float sprintCostPerSecond = 5f;

        [Header("UI / Debug")]
        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;
        public bool IsExhausted => currentStamina <= 0f;

        private float currentStamina;
        private float timeSinceLastConsume;
        private bool isBlocking;

        public event Action<float> OnStaminaChanged;
        public event Action OnExhausted;
        public event Action OnRecovered;

        private void Start()
        {
            currentStamina = maxStamina;
            timeSinceLastConsume = regenDelay;
        }

        private void Update()
        {
            timeSinceLastConsume += Time.deltaTime;

            // Block drain
            if (isBlocking)
            {
                ConsumeStamina(blockCostPerSecond * Time.deltaTime);
            }

            // Regeneration
            if (timeSinceLastConsume >= regenDelay && currentStamina < maxStamina)
            {
                currentStamina += regenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
                OnStaminaChanged?.Invoke(currentStamina);
            }
        }

        /// <summary>
        /// Attempts to consume stamina. Returns true if successful.
        /// </summary>
        public bool TryConsumeStamina(float amount)
        {
            if (currentStamina < amount)
                return false;

            currentStamina -= amount;
            timeSinceLastConsume = 0f;
            OnStaminaChanged?.Invoke(currentStamina);

            if (currentStamina <= 0f)
                OnExhausted?.Invoke();

            return true;
        }

        /// <summary>
        /// Consumes stamina unconditionally (for continuous drains like blocking).
        /// </summary>
        public void ConsumeStamina(float amount)
        {
            currentStamina -= amount;
            timeSinceLastConsume = 0f;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                OnExhausted?.Invoke();
            }

            OnStaminaChanged?.Invoke(currentStamina);
        }

        /// <summary>
        /// Restores stamina.
        /// </summary>
        public void RestoreStamina(float amount)
        {
            currentStamina += amount;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina);
            OnRecovered?.Invoke();
        }

        /// <summary>
        /// Sets whether the character is blocking (affects drain).
        /// </summary>
        public void SetBlocking(bool blocking)
        {
            isBlocking = blocking;
        }

        /// <summary>
        /// Checks if dodge can be performed.
        /// </summary>
        public bool CanDodge => currentStamina >= dodgeCost;

        /// <summary>
        /// Attempts to consume dodge stamina.
        /// </summary>
        public bool TryDodge()
        {
            return TryConsumeStamina(dodgeCost);
        }
    }
}
