using StreetFighter.Core;
using StreetFighter.Data;
using UnityEngine;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Manages player stamina for actions: sprinting, dodging, blocking, attacking.
    /// </summary>
    public sealed class StaminaSystem : MonoBehaviour
    {
        [SerializeField]
        private StaminaConfig staminaConfig = null;

        private float currentStamina;
        private float regenDelayTimer;
        private IEventBus eventBus;

        public float CurrentStamina => currentStamina;
        public float MaxStamina => staminaConfig.MaxStamina;
        public float StaminaPercent => currentStamina / staminaConfig.MaxStamina;
        public bool HasStamina(float amount) => currentStamina >= amount;

        private void Awake()
        {
            currentStamina = staminaConfig.MaxStamina;
            eventBus = ServiceLocator.Resolve<IEventBus>();
        }

        private void Update()
        {
            if (regenDelayTimer > 0f)
            {
                regenDelayTimer -= Time.deltaTime;
            }
            else
            {
                RegenerateStamina();
            }
        }

        /// <summary>
        /// Consumes stamina and resets the regen delay.
        /// </summary>
        public bool TryConsumeStamina(float amount)
        {
            if (!HasStamina(amount))
            {
                return false;
            }

            currentStamina -= amount;
            regenDelayTimer = staminaConfig.RegenDelayAfterAction;
            return true;
        }

        /// <summary>
        /// Returns stamina without resetting the regen delay.
        /// </summary>
        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(currentStamina + amount, staminaConfig.MaxStamina);
        }

        private void RegenerateStamina()
        {
            currentStamina = Mathf.Min(currentStamina + staminaConfig.RegenPerSecond * Time.deltaTime, staminaConfig.MaxStamina);
        }
    }
}
