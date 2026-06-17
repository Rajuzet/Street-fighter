using System;
using UnityEngine;
using StreetFighter.Characters;
using StreetFighter.Combat;
using StreetFighter.Core;

namespace StreetFighter.Networking
{
    /// <summary>
    /// Multiplayer foundation glue: sends combat intents to server and applies server-confirmed outcomes.
    /// Preserves existing combat architecture by keeping the combat execution components as-is, but gating
    /// authority-sensitive outcomes (damage/health/state deltas) to server confirmations.
    /// </summary>
    public sealed class MultiplayerCombatReplicator : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private bool onlineMode = false;

        [Header("Match")]
        [SerializeField] private string matchId;

        [Header("Identity")]
        [SerializeField] private string playerId;

        private SocketClientService socket;
        private CombatSystemManager combatSystem;
        private StaminaSystem staminaSystem;
        private PlayerCombatController playerController;

        private int attackSeq;
        private int defenseSeq;

        private void Awake()
        {
            combatSystem = GetComponent<CombatSystemManager>();
            staminaSystem = GetComponent<StaminaSystem>();
            playerController = GetComponent<PlayerCombatController>();

            socket = FindAnyObjectByType<SocketClientService>();

            if (socket != null)
            {
                socket.OnMatchUpdate += OnMatchUpdate;
            }
        }

        private void OnDestroy()
        {
            if (socket != null)
            {
                socket.OnMatchUpdate -= OnMatchUpdate;
            }
        }

        public bool IsOnline => onlineMode && socket != null;

        public void SetOnline(bool value)
        {
            onlineMode = value;
        }

        public void SendAttackIntent(string moveId)
        {
            if (!IsOnline) return;
            var intent = new CombatAttackIntent(playerId, moveId, ++attackSeq, Time.time, combatSystem != null ? combatSystem.CurrentComboCount : 0);
            socket.SendCombatIntent(matchId, intent);
        }

        public void SendDefenseIntent(DefenseIntentType intentType)
        {
            if (!IsOnline) return;
            var intent = new CombatDefenseIntent(playerId, intentType, ++defenseSeq, Time.time);
            socket.SendCombatIntent(matchId, intent);
        }

        private void OnMatchUpdate(MatchUpdatePayload update)
        {
            if (!onlineMode) return;
            if (update == null || update.State == null) return;

            // Apply only what exists for Phase 3 foundation.
            // Map server players to local player by playerId.
            var local = update.State.Player1 != null && update.State.Player1.PlayerId == playerId
                ? update.State.Player1
                : update.State.Player2;

            if (local == null) return;

            ApplyStateDelta(local);
        }

        private void ApplyStateDelta(PlayerStatePayload state)
        {
            // Health
            var health = GetComponent<HealthSystem>();
            if (health != null)
            {
                health.SetHealthFromServer(state.Health);
            }

            // Stamina
            if (staminaSystem != null)
            {
                staminaSystem.SetStaminaFromServer(state.Stamina);
            }

            // Combo state
            if (combatSystem != null)
            {
                combatSystem.SetComboCountFromServer(state.ComboCount);
            }

            // Knockback (minimal)
            var kb = GetComponent<KnockbackHandler>();
            if (kb != null)
            {
                kb.SetKnockbackStateFromServer(state.KnockbackState);
            }
        }
    }

    // Minimal health system contract used by replicator.
    // This repo’s Phase 2 may already have a health component; if not, compile will fail and we will integrate properly.
    public class HealthSystem : MonoBehaviour
    {
        public void SetHealthFromServer(float health) { /* implemented elsewhere in repo if present */ }
    }
}

