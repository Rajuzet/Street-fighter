using UnityEngine;

namespace StreetFighter.Data
{
    /// <summary>
    /// Defines a combat action that can be chained into combos.
    /// </summary>
    [CreateAssetMenu(fileName = "CombatMoveData", menuName = "StreetFighter/Data/CombatMoveData")]
    public sealed class CombatMoveData : ScriptableObject
    {
        [SerializeField]
        private string moveId = "";

        [SerializeField]
        private string displayName = "";

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
        public float Damage => damage;
        public float Cooldown => cooldown;
        public float Recovery => recovery;
        public bool IsBlockable => isBlockable;
        public string AnimationTrigger => animationTrigger;
    }
}
