using System.Collections.Generic;
using UnityEngine;

namespace StreetFighter.Data
{
    /// <summary>
    /// Phase 3: Defines a single fighter entry for the character roster.
    /// Includes unique moves, stats, portrait, color palettes, and unlock conditions.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterRosterData", menuName = "StreetFighter/Data/CharacterRosterData")]
    public sealed class CharacterRosterData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string characterId = "";

        [SerializeField]
        private string displayName = "Fighter";

        [SerializeField]
        private string description = "";

        [SerializeField]
        private Sprite portrait;

        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private GameObject prefab;

        [Header("Stats (override base profile)")]
        [SerializeField]
        private CharacterProfileData baseProfile;

        [SerializeField]
        private float maxHealthOverride = -1f;

        [SerializeField]
        private float maxStaminaOverride = -1f;

        [SerializeField]
        private float armorOverride = -1f;

        [Header("Unique Moves")]
        [SerializeField]
        private List<CombatMoveData> uniqueMoves = new();

        [SerializeField]
        private CombatMoveData specialMove;

        [SerializeField]
        private CombatMoveData superMove;

        [SerializeField]
        private CombatMoveData ultraMove;

        [Header("Colors")]
        [SerializeField]
        private List<CharacterColorData> colorPalettes = new();

        [Header("Unlock")]
        [SerializeField]
        private bool unlockedByDefault = true;

        [SerializeField]
        private string unlockCondition = "";

        public string CharacterId => characterId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Portrait => portrait;
        public Sprite Icon => icon;
        public GameObject Prefab => prefab;
        public CharacterProfileData BaseProfile => baseProfile;
        public float MaxHealthOverride => maxHealthOverride;
        public float MaxStaminaOverride => maxStaminaOverride;
        public float ArmorOverride => armorOverride;
        public IReadOnlyList<CombatMoveData> UniqueMoves => uniqueMoves;
        public CombatMoveData SpecialMove => specialMove;
        public CombatMoveData SuperMove => superMove;
        public CombatMoveData UltraMove => ultraMove;
        public IReadOnlyList<CharacterColorData> ColorPalettes => colorPalettes;
        public bool UnlockedByDefault => unlockedByDefault;
        public string UnlockCondition => unlockCondition;

        /// <summary>
        /// Gets the effective max health (uses override if set, else base profile value).
        /// </summary>
        public float GetEffectiveMaxHealth(float baseValue)
        {
            return maxHealthOverride > 0f ? maxHealthOverride : baseValue;
        }

        /// <summary>
        /// Gets the effective max stamina.
        /// </summary>
        public float GetEffectiveMaxStamina(float baseValue)
        {
            return maxStaminaOverride > 0f ? maxStaminaOverride : baseValue;
        }

        /// <summary>
        /// Gets the effective armor.
        /// </summary>
        public float GetEffectiveArmor(float baseValue)
        {
            return armorOverride >= 0f ? armorOverride : baseValue;
        }
    }
}
