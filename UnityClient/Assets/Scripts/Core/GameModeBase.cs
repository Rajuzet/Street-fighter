using System;
using StreetFighter.Characters;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 3: Abstract base class for all game modes.
    /// Defines common match flow, player spawning, and mode-specific hooks.
    /// </summary>
    public abstract class GameModeBase : MonoBehaviour
    {
        [Header("Players")]
        [SerializeField]
        protected Transform p1SpawnPoint;

        [SerializeField]
        protected Transform p2SpawnPoint;

        [SerializeField]
        protected GameObject playerPrefab;

        [Header("Mode Info")]
        [SerializeField]
        protected string modeName = "Base Mode";

        [SerializeField]
        protected float matchTimerSeconds = 99f;

        protected GameObject p1Instance;
        protected GameObject p2Instance;
        protected bool matchActive;
        protected float currentTimer;

        /// <summary>
        /// Called when the mode is initialized (before round start).
        /// </summary>
        public abstract void InitializeMode();

        /// <summary>
        /// Called to start the match/round.
        /// </summary>
        public abstract void StartMatch();

        /// <summary>
        /// Called every frame during active match.
        /// </summary>
        public abstract void UpdateMatch();

        /// <summary>
        /// Called when the match/round ends.
        /// </summary>
        public abstract void EndMatch(int winnerPlayerIndex);

        /// <summary>
        /// Spawns both players at their designated spawn points.
        /// </summary>
        protected virtual void SpawnPlayers(CharacterRosterData p1Data, int p1Color,
                                            CharacterRosterData p2Data, int p2Color)
        {
            if (p1SpawnPoint != null && playerPrefab != null)
            {
                p1Instance = Instantiate(playerPrefab, p1SpawnPoint.position, p1SpawnPoint.rotation);
                var setup = p1Instance.GetComponent<CharacterSetup>();
                setup?.InitializeFromRoster(p1Data, p1Color);
            }

            if (p2SpawnPoint != null && playerPrefab != null)
            {
                p2Instance = Instantiate(playerPrefab, p2SpawnPoint.position, p2SpawnPoint.rotation);
                var setup = p2Instance.GetComponent<CharacterSetup>();
                setup?.InitializeFromRoster(p2Data, p2Color);
            }
        }

        /// <summary>
        /// Destroys current player instances.
        /// </summary>
        protected virtual void ClearPlayers()
        {
            if (p1Instance != null)
            {
                Destroy(p1Instance);
                p1Instance = null;
            }
            if (p2Instance != null)
            {
                Destroy(p2Instance);
                p2Instance = null;
            }
        }

        /// <summary>
        /// Gets whether the match is currently active.
        /// </summary>
        public bool IsMatchActive => matchActive;

        /// <summary>
        /// Gets the current match timer.
        /// </summary>
        public float CurrentTimer => currentTimer;

        /// <summary>
        /// Gets the mode display name.
        /// </summary>
        public string ModeName => modeName;

        protected virtual void Update()
        {
            if (matchActive)
            {
                currentTimer -= Time.deltaTime;
                UpdateMatch();

                if (currentTimer <= 0f)
                {
                    currentTimer = 0f;
                    OnTimerExpired();
                }
            }
        }

        /// <summary>
        /// Called when the match timer reaches zero.
        /// </summary>
        protected virtual void OnTimerExpired()
        {
            // Default: evaluate winner by health
            EndMatch(EvaluateWinnerByHealth());
        }

        /// <summary>
        /// Evaluates winner by comparing remaining health.
        /// </summary>
        protected virtual int EvaluateWinnerByHealth()
        {
            float p1Health = GetPlayerHealth(0);
            float p2Health = GetPlayerHealth(1);

            if (p1Health > p2Health) return 0;
            if (p2Health > p1Health) return 1;
            return -1; // Draw
        }

        /// <summary>
        /// Gets normalized health for a player (0-1).
        /// </summary>
        protected float GetPlayerHealth(int playerIndex)
        {
            var instance = playerIndex == 0 ? p1Instance : p2Instance;
            if (instance == null) return 0f;

            var stats = instance.GetComponent<CharacterStats>();
            return stats != null ? stats.HealthRatio : 0f;
        }
    }
}
