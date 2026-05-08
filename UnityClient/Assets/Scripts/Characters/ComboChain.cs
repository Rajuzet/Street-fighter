using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StreetFighter.Data;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Phase 2B combo chain executor.
    /// Maintains the existing CombatSystemManager API expectations:
    /// - AddMove(CombatMoveData)
    /// - Playback(CombatMoveData, Action onComplete)
    /// - Clear()
    /// </summary>
    public sealed class ComboChain
    {
        private readonly List<AttackDefinition> chain = new();
        private int lastComboId;

        public int Count => chain.Count;

        public void AddMove(AttackDefinition moveData)
        {
            if (moveData == null) return;
            chain.Add(moveData);
        }

        // Overload to preserve any older signatures that used a more generic move type.
        public void AddMove(object moveData)
        {
            if (moveData is AttackDefinition ad)
                AddMove(ad);
        }

        public IEnumerator Playback(object moveData, Action onComplete)
        {
            // If caller supplies a move, ensure it is part of the chain.
            if (moveData is AttackDefinition attack)
            {
                AddMove(attack);
            }

            // Playback all queued moves sequentially.
            // This preserves existing architecture (CombatSystemManager drives playback coroutines).
            int comboId = ++lastComboId;

            for (int i = 0; i < chain.Count; i++)
            {
                // If Clear() has been called, lastComboId changes and we stop.
                if (comboId != lastComboId)
                    yield break;

                AttackDefinition move = chain[i];
                if (move == null) continue;

                // We avoid hard-coded animation wait where possible:
                // We still need a fallback for now because the current framework.
                // If animations exist, they should trigger hit windows via HitDetectionSystem.
                float totalSeconds = move.TotalFrames / 60f;
                float safe = Mathf.Max(0.01f, totalSeconds);

                yield return new WaitForSeconds(safe);
            }

            onComplete?.Invoke();
        }

        public void Clear()
        {
            chain.Clear();
            // Invalidate any in-flight coroutine playback.
            lastComboId++;
        }

        // Compatibility method: old code path expects the concrete move definition.
        public IEnumerator Playback(AttackDefinition moveData, Action onComplete)
            => Playback((object)moveData, onComplete);
    }
}

