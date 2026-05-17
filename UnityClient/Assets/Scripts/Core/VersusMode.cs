using StreetFighter.Characters;
using StreetFighter.UI;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 3: 1v1 Versus mode with best-of rounds.
    /// Supports configurable round count and win tracking.
    /// </summary>
    public sealed class VersusMode : GameModeBase
    {
        [Header("Versus Settings")]
        [SerializeField]
        private int roundsToWin = 2;

        [SerializeField]
        private float roundStartDelay = 2f;

        [SerializeField]
        private float roundEndDelay = 3f;

        [Header("UI")]
        [SerializeField]
        private HUDManager hud;

        [SerializeField]
        private RoundManager roundManager;

        private int p1Wins;
        private int p2Wins;
        private int currentRound;
        private bool roundActive;
        private float roundStateTimer;
        private VersusState state;

        private CharacterRosterData p1Roster;
        private int p1Color;
        private CharacterRosterData p2Roster;
        private int p2Color;

        private enum VersusState
        {
            Idle,
            RoundStart,
            Fighting,
            RoundEnd,
            MatchEnd
        }

        public override void InitializeMode()
        {
            modeName = "Versus";
            p1Wins = 0;
            p2Wins = 0;
            currentRound = 0;
            state = VersusState.Idle;

            hud?.ResetHUD();
            roundManager?.ResetRounds();
        }

        /// <summary>
        /// Sets roster selection data from character select screen.
        /// </summary>
        public void SetSelections(CharacterRosterData p1, int c1, CharacterRosterData p2, int c2)
        {
            p1Roster = p1;
            p1Color = c1;
            p2Roster = p2;
            p2Color = c2;
        }

        public override void StartMatch()
        {
            currentRound = 1;
            StartRound();
        }

        private void StartRound()
        {
            state = VersusState.RoundStart;
            roundStateTimer = roundStartDelay;
            roundActive = false;
            matchActive = false;

            ClearPlayers();
            SpawnPlayers(p1Roster, p1Color, p2Roster, p2Color);

            hud?.ResetHUD();
            hud?.SetPortrait(0, p1Roster?.Portrait);
            hud?.SetPortrait(1, p2Roster?.Portrait);
            hud?.SetRoundWins(0, p1Wins);
            hud?.SetRoundWins(1, p2Wins);

            roundManager?.StartRound(currentRound);
        }

        public override void UpdateMatch()
        {
            if (hud != null)
            {
                hud.SetTimer(currentTimer);

                // Update health/stamina bars from live stats
                UpdateHUDFromPlayers();
            }

            // Check for K.O.
            if (roundActive)
            {
                bool p1KO = GetPlayerHealth(0) <= 0f;
                bool p2KO = GetPlayerHealth(1) <= 0f;

                if (p1KO || p2KO)
                {
                    int winner = EvaluateRoundWinner();
                    EndRound(winner);
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            switch (state)
            {
                case VersusState.RoundStart:
                    roundStateTimer -= Time.deltaTime;
                    if (roundStateTimer <= 0f)
                    {
                        state = VersusState.Fighting;
                        roundActive = true;
                        matchActive = true;
                        currentTimer = matchTimerSeconds;
                        roundManager?.ShowFightText();
                    }
                    break;

                case VersusState.RoundEnd:
                    roundStateTimer -= Time.deltaTime;
                    if (roundStateTimer <= 0f)
                    {
                        if (p1Wins >= roundsToWin || p2Wins >= roundsToWin)
                        {
                            state = VersusState.MatchEnd;
                            int matchWinner = p1Wins >= roundsToWin ? 0 : 1;
                            EndMatch(matchWinner);
                        }
                        else
                        {
                            currentRound++;
                            StartRound();
                        }
                    }
                    break;
            }
        }

        public override void EndMatch(int winnerPlayerIndex)
        {
            matchActive = false;
            roundActive = false;
            state = VersusState.MatchEnd;

            roundManager?.ShowMatchEnd(winnerPlayerIndex);
            PlayVictoryAnimation(winnerPlayerIndex);
            PlayDefeatAnimation(winnerPlayerIndex == 0 ? 1 : 0);
        }

        private void EndRound(int winnerPlayerIndex)
        {
            roundActive = false;
            matchActive = false;
            state = VersusState.RoundEnd;
            roundStateTimer = roundEndDelay;

            if (winnerPlayerIndex == 0)
                p1Wins++;
            else if (winnerPlayerIndex == 1)
                p2Wins++;

            roundManager?.ShowRoundEnd(winnerPlayerIndex, currentRound);
            hud?.SetRoundWins(0, p1Wins);
            hud?.SetRoundWins(1, p2Wins);
        }

        private int EvaluateRoundWinner()
        {
            float p1Health = GetPlayerHealth(0);
            float p2Health = GetPlayerHealth(1);

            if (p1Health <= 0f && p2Health <= 0f)
                return -1; // Double K.O.
            if (p2Health <= 0f)
                return 0;
            if (p1Health <= 0f)
                return 1;
            return EvaluateWinnerByHealth();
        }

        private void UpdateHUDFromPlayers()
        {
            if (p1Instance != null)
            {
                var p1Stats = p1Instance.GetComponent<CharacterStats>();
                var p1Stamina = p1Instance.GetComponent<StaminaSystem>();
                if (p1Stats != null) hud?.SetHealth(0, p1Stats.HealthRatio);
                if (p1Stamina != null) hud?.SetStamina(0, p1Stamina.StaminaRatio);

                var p1Combat = p1Instance.GetComponent<CombatSystemManager>();
                if (p1Combat != null) hud?.SetComboCount(0, p1Combat.GetComboCount());
            }

            if (p2Instance != null)
            {
                var p2Stats = p2Instance.GetComponent<CharacterStats>();
                var p2Stamina = p2Instance.GetComponent<StaminaSystem>();
                if (p2Stats != null) hud?.SetHealth(1, p2Stats.HealthRatio);
                if (p2Stamina != null) hud?.SetStamina(1, p2Stamina.StaminaRatio);

                var p2Combat = p2Instance.GetComponent<CombatSystemManager>();
                if (p2Combat != null) hud?.SetComboCount(1, p2Combat.GetComboCount());
            }
        }

        private void PlayVictoryAnimation(int playerIndex)
        {
            var instance = playerIndex == 0 ? p1Instance : p2Instance;
            var anim = instance?.GetComponent<CharacterAnimationController>();
            anim?.SetVictory(true);
        }

        private void PlayDefeatAnimation(int playerIndex)
        {
            var instance = playerIndex == 0 ? p1Instance : p2Instance;
            var anim = instance?.GetComponent<CharacterAnimationController>();
            anim?.SetDefeat(true);
        }

        /// <summary>
        /// Gets current round number.
        /// </summary>
        public int CurrentRound => currentRound;

        /// <summary>
        /// Gets whether the full match is complete.
        /// </summary>
        public bool IsMatchComplete => state == VersusState.MatchEnd;
    }
}
