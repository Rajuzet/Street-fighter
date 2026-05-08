using UnityEngine;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Stamina system configuration for player actions.
    /// </summary>
    [CreateAssetMenu(fileName = "StaminaConfig", menuName = "StreetFighter/Combat/StaminaConfig")]
    public sealed class StaminaConfig : ScriptableObject
    {
        [SerializeField]
        private float maxStamina = 100f;

        [SerializeField]
        private float regenPerSecond = 15f;

        [SerializeField]
        private float regenDelayAfterAction = 0.5f;

        [SerializeField]
        private float sprintCostPerSecond = 20f;

        [SerializeField]
        private float dodgeCost = 30f;

        [SerializeField]
        private float blockCostPerSecond = 5f;

        public float MaxStamina => maxStamina;
        public float RegenPerSecond => regenPerSecond;
        public float RegenDelayAfterAction => regenDelayAfterAction;
        public float SprintCostPerSecond => sprintCostPerSecond;
        public float DodgeCost => dodgeCost;
        public float BlockCostPerSecond => blockCostPerSecond;
    }
}
