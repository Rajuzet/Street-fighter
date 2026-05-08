using System.Collections.Generic;
using UnityEngine;
using StreetFighter.Combat;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Phase 2B: Manages aggro group behavior - turn-taking, spacing, mini-boss framework.
    /// </summary>
    public class EnemyAggroGroup : MonoBehaviour
    {
        private List<CombatAI> members = new();
        private Dictionary<CombatAI, float> turnCooldowns = new();
        [SerializeField] private float baseTurnDelay = 1.5f;
        [SerializeField] private int maxSimultaneousAttackers = 2;
        private int activeAttackers = 0;

        public void RegisterMember(CombatAI ai)
        {
            if (!members.Contains(ai))
            {
                members.Add(ai);
            }
        }

        public void UnregisterMember(CombatAI ai)
        {
            members.Remove(ai);
            turnCooldowns.Remove(ai);
            if (members.Contains(ai)) activeAttackers--;
        }

        public bool CanAttack(CombatAI ai)
        {
            if (!members.Contains(ai)) return false;

            if (turnCooldowns.TryGetValue(ai, out float cooldown) && Time.time < cooldown)
                return false;

            if (activeAttackers >= maxSimultaneousAttackers)
                return false;

            // Grant turn
            turnCooldowns[ai] = Time.time + baseTurnDelay;
            activeAttackers++;
            return true;
        }

        public void OnAttackComplete(CombatAI ai)
        {
            if (members.Contains(ai)) activeAttackers--;
        }

        // Difficulty scaling: Increase aggression by group size
        public float GetAggroMultiplier() => 1f + (members.Count - 1) * 0.2f;

        // Mini-boss: Leader with buffs
        public CombatAI GetLeader() => members.Count > 0 ? members[0] : null;
    }
}

