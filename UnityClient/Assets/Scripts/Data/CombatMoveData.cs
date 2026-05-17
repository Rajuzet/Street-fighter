using UnityEngine;

namespace StreetFighter.Data
{
    /// <summary>
    /// Phase 3: Defines a combat action that can be chained into combos.
    /// Links to a specific character via CharacterId for roster-based move loading.
    /// </summary>
    [CreateAssetMenu(fileName = "CombatMoveData", menuName = "StreetFighter/Data/CombatMoveData")]
    public sealed class CombatMoveData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string moveId = "";

        [SerializeField]
        private string displayName = "";

        [Header("Character Link")]
        [SerializeField]
        [Tooltip("Character ID this move belongs to. Empty for universal moves.")]
        private string characterId = "";

        [Header("Combat Properties")]
        [SerializeField]
        private float damage = 10f;

        [SerializeField]
        private float cooldown = 0.25f;

        [SerializeField]
        private float recovery = 0.35f;

        [SerializeField]
        private bool isBlockable = true;

        [SerializeField]
        private string animationTrigger = "";

        public string MoveId => moveId;
        public string DisplayName => displayName;

        // Phase 3: Character linkage for roster-specific moves
        public string CharacterId => characterId;
        public bool IsCharacterSpecific => !string.IsNullOrEmpty(characterId);

        public float Damage => damage;
        public float Cooldown => cooldown;
        public float Recovery => recovery;
        public bool IsBlockable => isBlockable;
        public string AnimationTrigger => animationTrigger;
    }
}
