using System.Collections.Generic;
using StreetFighter.Characters;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 3: Arcade ladder mode. Progresses through a pre-defined opponent list.
    /// Handles difficulty scaling, bonus stages, and progression persistence.
    /// </summary>
    public sealed class ArcadeMode : GameModeBase
    {
        [Header("Arcade Settings")]
        [SerializeField]
        private List<ArcadeOpponent> opponentLadder = new();

        [SerializeField]
        private int currentStage = 0;

        [SerializeField]
        private bool allowContinue = true;

        [SerializeField]
        private int continuesUsed = 0;

        [Header("Player")]
        [SerializeField]
        private CharacterRosterData playerRoster;

        [SerializeField]
        private int playerColor;

        private bool stageActive;
        private bool waitingForContinue;

        public override void InitializeMode()
        {
            modeName = "Arcade";
            currentStage = 0;
            continuesUsed = 0;
            stageActive = false;
            waitingForContinue = false;
        }

        /// <summary>
        /// Sets the player selection for arcade mode.
        /// </summary>
        public void SetPlayerSelection(CharacterRosterData roster, int color)
        {
            playerRoster = roster;
            playerColor = color;
        }

        public override void StartMatch()
        {
            StartStage();
        }

        private void StartStage()
        {
            if (currentStage >= opponentLadder.Count)
            {
                OnArcadeComplete();
                return;
            }

            var opponent = opponentLadder[currentStage];
            if (opponent == null || opponent.OpponentData == null)
            {
                currentStage++;
                StartStage();
                return;
            }

            ClearPlayers();
            SpawnPlayers(playerRoster, playerColor, opponent.OpponentData, opponent.OpponentColor);

            currentTimer = opponent.StageTimer > 0f ? opponent.StageTimer : matchTimerSeconds;
            stageActive = true;
            matchActive = true;
        }

        public override void UpdateMatch()
        {
            if (!stageActive || waitingForContinue) return;

            // Check for K.O.
            bool playerKO = GetPlayerHealth(0) <= 0f;
            bool opponentKO = GetPlayerHealth(1) <= 0f;

            if (playerKO || opponentKO)
            {
                stageActive = false;
                matchActive = false;

                if (opponentKO)
                {
                    currentStage++;
                    StartStage();
                }
                else
                {
                    OnPlayerDefeated();
                }
            }
        }

        public override void EndMatch(int winnerPlayerIndex)
        {
            stageActive = false;
            matchActive = false;
        }

        private void OnPlayerDefeated()
        {
            if (allowContinue)
            {
                waitingForContinue = true;
                continuesUsed++;
                // UI prompt for continue would go here
                // After continue, restart current stage
                waitingForContinue = false;
                StartStage();
            }
            else
            {
                OnArcadeFailed();
            }
        }

        private void OnArcadeComplete()
        {
            // Trigger ending, credits, unlocks
        }

        private void OnArcadeFailed()
        {
            // Return to main menu or game over screen
        }

        /// <summary>
        /// Adds a stage to the arcade ladder at runtime.
        /// </summary>
        public void AddStage(ArcadeOpponent opponent)
        {
            opponentLadder.Add(opponent);
        }

        /// <summary>
        /// Gets current progress (0-based stage index).
        /// </summary>
        public int CurrentStage => currentStage;

        /// <summary>
        /// Gets total stages in ladder.
        /// </summary>
        public int TotalStages => opponentLadder.Count;

        /// <summary>
        /// Gets whether arcade run is complete.
        /// </summary>
        public bool IsComplete => currentStage >= opponentLadder.Count;
    }

    /// <summary>
    /// Defines a single arcade stage opponent.
    /// </summary>
    [System.Serializable]
    public class ArcadeOpponent
    {
        public CharacterRosterData OpponentData;
        public int OpponentColor;
        public float StageTimer = 99f;
        public bool IsBossStage = false;
        public int AIBehaviorPreset = 0; // Maps to difficulty/behavior tree variant
    }
}
