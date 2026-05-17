using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StreetFighter.Combat;
using StreetFighter.Data;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 2 combo chain executor with move type validation.
    /// Validates chaining rules and tracks combo progression.
    /// </summary>
    public sealed class ComboChain
    {
        private readonly List<AttackDefinition> chain = new();
        private int lastComboId;
        private MoveType lastMoveType = MoveType.None;

        public int Count => chain.Count;
        public IReadOnlyList<AttackDefinition> ChainMoves => chain;

        /// <summary>
        /// Adds a move to the combo chain if chaining rules allow.
        /// </summary>
        public bool TryAddMove(AttackDefinition moveData)
        {
            if (moveData == null) return false;

            // Get move type from the attack definition (can be extended with a MoveType field)
            var moveType = InferMoveType(moveData);

            // Validate chaining
            if (!MoveTypeRules.CanChainInto(lastMoveType, moveType))
            {
                Debug.LogWarning($"Cannot chain {lastMoveType} into {moveType}. Clearing combo.");
                Clear();
                lastMoveType = MoveType.None;
            }

            chain.Add(moveData);
            lastMoveType = moveType;
            return true;
        }

        public void AddMove(AttackDefinition moveData)
        {
            TryAddMove(moveData);
        }

        // Overload to preserve older signatures
        public void AddMove(object moveData)
        {
            if (moveData is AttackDefinition ad)
                AddMove(ad);
        }

        public IEnumerator Playback(object moveData, Action onComplete)
        {
            if (moveData is AttackDefinition attack)
            {
                AddMove(attack);
            }

            int comboId = ++lastComboId;

            for (int i = 0; i < chain.Count; i++)
            {
                if (comboId != lastComboId)
                    yield break;

                AttackDefinition move = chain[i];
                if (move == null) continue;

                float totalSeconds = move.TotalFrames / 60f;
                float safe = Mathf.Max(0.01f, totalSeconds);

                yield return new WaitForSeconds(safe);
            }

            onComplete?.Invoke();
        }

        public IEnumerator Playback(AttackDefinition moveData, Action onComplete)
            => Playback((object)moveData, onComplete);

        public void Clear()
        {
            chain.Clear();
            lastComboId++;
            lastMoveType = MoveType.None;
        }

        /// <summary>
        /// Infers move type from attack properties.
        /// </summary>
        private static MoveType InferMoveType(AttackDefinition attack)
        {
            // Infer from animation trigger name or properties
            string trigger = attack.AnimationTrigger?.ToLowerInvariant() ?? "";
            
            if (trigger.Contains("kick"))
                return MoveType.Kick;
            if (trigger.Contains("power") || trigger.Contains("heavy") || attack.BaseDamage > 20f)
                return MoveType.Power;
            if (trigger.Contains("special") || trigger.Contains("spin") || attack.BaseDamage > 30f)
                return MoveType.Special;
            if (trigger.Contains("throw"))
                return MoveType.Throw;
            if (trigger.Contains("counter"))
                return MoveType.Counter;
            
            return MoveType.Punch; // Default
        }
    }
}
