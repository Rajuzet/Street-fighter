using StreetFighter.Combat;
using StreetFighter.Data;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 2: Event raised when a critical hit occurs during a combo sequence.
    /// Contains combo count, move data, and critical multiplier info.
    /// </summary>
    public sealed class ComboCriticalEvent : IEvent
    {
        public int ComboCount { get; }
        public CombatMoveData Move { get; }
        public float CriticalMultiplier { get; }
        public GameObject Attacker { get; }
        public GameObject Target { get; }

        public ComboCriticalEvent(int comboCount, CombatMoveData move, float criticalMultiplier = 2f)
        {
            ComboCount = comboCount;
            Move = move;
            CriticalMultiplier = criticalMultiplier;
        }

        public ComboCriticalEvent(int comboCount, CombatMoveData move, GameObject attacker, GameObject target, float criticalMultiplier = 2f)
        {
            ComboCount = comboCount;
            Move = move;
            Attacker = attacker;
            Target = target;
            CriticalMultiplier = criticalMultiplier;
        }
    }
}
