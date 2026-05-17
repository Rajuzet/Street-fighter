using UnityEngine;

namespace StreetFighter.Data
{
    /// <summary>
    /// Phase 3: Defines character archetype, base movement, and fighter-specific stats
    /// (health, stamina, attack, defense) used by CharacterRosterData for stat overrides.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterProfileData", menuName = "StreetFighter/Data/CharacterProfileData")]
    public sealed class CharacterProfileData : ScriptableObject
    {
        [Header("Base Profile")]
        [SerializeField]
        private string profileName = "Default";

        [Header("Movement")]
        [SerializeField]
        private float movementSpeed = 6f;

        [SerializeField]
        private float sprintSpeed = 9f;

        [SerializeField]
        private float jumpHeight = 1.8f;

        [SerializeField]
        private float gravity = -24f;

        [Header("Fighter Stats")]
        [SerializeField]
        [Tooltip("Base maximum health for this character.")]
        private float baseHealth = 1000f;

        [SerializeField]
        [Tooltip("Base maximum stamina for this character.")]
        private float baseStamina = 100f;

        [SerializeField]
        [Tooltip("Base attack power multiplier.")]
        private float baseAttack = 1f;

        [SerializeField]
        [Tooltip("Base defense damage reduction multiplier.")]
        private float baseDefense = 1f;

        [SerializeField]
        [Tooltip("Stamina regeneration rate per second.")]
        private float staminaRegenRate = 10f;

        public string ProfileName => profileName;
        public float MovementSpeed => movementSpeed;
        public float SprintSpeed => sprintSpeed;
        public float JumpHeight => jumpHeight;
        public float Gravity => gravity;

        // Phase 3: Fighter stat accessors
        public float BaseHealth => baseHealth;
        public float BaseStamina => baseStamina;
        public float BaseAttack => baseAttack;
        public float BaseDefense => baseDefense;
        public float StaminaRegenRate => staminaRegenRate;
    }
}
