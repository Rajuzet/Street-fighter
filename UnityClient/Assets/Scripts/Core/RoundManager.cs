using UnityEngine;
using UnityEngine.UI;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 3: Round UI manager. Displays round start, FIGHT, K.O., round end,
    /// and match end text. Tracks round counts per player.
    /// </summary>
    public sealed class RoundManager : MonoBehaviour
    {
        [Header("Text Displays")]
        [SerializeField]
        private Text roundStartText;

        [SerializeField]
        private Text fightText;

        [SerializeField]
        private Text koText;

        [SerializeField]
        private Text roundEndText;

        [SerializeField]
        private Text matchEndText;

        [Header("Animation")]
        [SerializeField]
        private Animator textAnimator;

        [SerializeField]
        private string roundStartTrigger = "RoundStart";

        [SerializeField]
        private string fightTrigger = "Fight";

        [SerializeField]
        private string koTrigger = "KO";

        [SerializeField]
        private string winTrigger = "Win";

        private int p1RoundWins;
        private int p2RoundWins;
        private int currentRound;

        private void Awake()
        {
            ResetRounds();
        }

        /// <summary>
        /// Resets round tracking and hides all text.
        /// </summary>
        public void ResetRounds()
        {
            p1RoundWins = 0;
            p2RoundWins = 0;
            currentRound = 0;
            HideAllText();
        }

        /// <summary>
        /// Shows round start announcement.
        /// </summary>
        public void StartRound(int roundNumber)
        {
            currentRound = roundNumber;
            HideAllText();

            if (roundStartText != null)
            {
                roundStartText.text = $"ROUND {roundNumber}";
                roundStartText.gameObject.SetActive(true);
            }

            textAnimator?.SetTrigger(roundStartTrigger);
        }

        /// <summary>
        /// Shows FIGHT text and hides round start.
        /// </summary>
        public void ShowFightText()
        {
            if (roundStartText != null)
                roundStartText.gameObject.SetActive(false);

            if (fightText != null)
            {
                fightText.text = "FIGHT!";
                fightText.gameObject.SetActive(true);
            }

            textAnimator?.SetTrigger(fightTrigger);

            // Auto-hide fight text after brief display
            Invoke(nameof(HideFightText), 1.5f);
        }

        private void HideFightText()
        {
            if (fightText != null)
                fightText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows K.O. text when a player is defeated.
        /// </summary>
        public void ShowKO()
        {
            if (koText != null)
            {
                koText.text = "K.O.";
                koText.gameObject.SetActive(true);
            }

            textAnimator?.SetTrigger(koTrigger);
        }

        /// <summary>
        /// Shows round end result.
        /// </summary>
        public void ShowRoundEnd(int winnerPlayerIndex, int roundNumber)
        {
            HideAllText();

            if (roundEndText != null)
            {
                string winnerName = winnerPlayerIndex == 0 ? "PLAYER 1" : "PLAYER 2";
                roundEndText.text = $"{winnerName} WINS ROUND {roundNumber}";
                roundEndText.gameObject.SetActive(true);
            }

            if (winnerPlayerIndex == 0)
                p1RoundWins++;
            else if (winnerPlayerIndex == 1)
                p2RoundWins++;
        }

        /// <summary>
        /// Shows match end result with winner announcement.
        /// </summary>
        public void ShowMatchEnd(int winnerPlayerIndex)
        {
            HideAllText();

            if (matchEndText != null)
            {
                string winnerName = winnerPlayerIndex == 0 ? "PLAYER 1" : "PLAYER 2";
                matchEndText.text = $"{winnerName} WINS!";
                matchEndText.gameObject.SetActive(true);
            }

            textAnimator?.SetTrigger(winTrigger);
        }

        private void HideAllText()
        {
            if (roundStartText != null) roundStartText.gameObject.SetActive(false);
            if (fightText != null) fightText.gameObject.SetActive(false);
            if (koText != null) koText.gameObject.SetActive(false);
            if (roundEndText != null) roundEndText.gameObject.SetActive(false);
            if (matchEndText != null) matchEndText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Gets current round number.
        /// </summary>
        public int CurrentRound => currentRound;

        /// <summary>
        /// Gets player 1 round wins.
        /// </summary>
        public int P1RoundWins => p1RoundWins;

        /// <summary>
        /// Gets player 2 round wins.
        /// </summary>
        public int P2RoundWins => p2RoundWins;
    }
}
