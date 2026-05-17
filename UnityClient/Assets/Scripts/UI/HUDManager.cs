using StreetFighter.Characters;
using StreetFighter.Core;
using UnityEngine;
using UnityEngine.UI;

namespace StreetFighter.UI
{
    /// <summary>
    /// Phase 3: In-match HUD controller. Manages health bars, stamina bars,
    /// combo counters, timer, and portraits for both players.
    /// </summary>
    public sealed class HUDManager : MonoBehaviour
    {
        [Header("Health Bars")]
        [SerializeField]
        private Image p1HealthFill;

        [SerializeField]
        private Image p2HealthFill;

        [SerializeField]
        private Image p1HealthDrain;

        [SerializeField]
        private Image p2HealthDrain;

        [Header("Stamina Bars")]
        [SerializeField]
        private Image p1StaminaFill;

        [SerializeField]
        private Image p2StaminaFill;

        [Header("Combo Counter")]
        [SerializeField]
        private Text p1ComboText;

        [SerializeField]
        private Text p2ComboText;

        [Header("Timer")]
        [SerializeField]
        private Text timerText;

        [Header("Portraits")]
        [SerializeField]
        private Image p1Portrait;

        [SerializeField]
        private Image p2Portrait;

        [Header("Round Indicators")]
        [SerializeField]
        private Transform p1RoundWins;

        [SerializeField]
        private Transform p2RoundWins;

        private float p1HealthTarget = 1f;
        private float p2HealthTarget = 1f;
        private float p1HealthDrainTarget = 1f;
        private float p2HealthDrainTarget = 1f;

        private const float DrainLerpSpeed = 2f;

        private void Update()
        {
            // Smooth drain bar animation
            if (p1HealthDrain != null)
                p1HealthDrain.fillAmount = Mathf.Lerp(p1HealthDrain.fillAmount, p1HealthDrainTarget, Time.deltaTime * DrainLerpSpeed);
            if (p2HealthDrain != null)
                p2HealthDrain.fillAmount = Mathf.Lerp(p2HealthDrain.fillAmount, p2HealthDrainTarget, Time.deltaTime * DrainLerpSpeed);
        }

        /// <summary>
        /// Updates health bar for a player.
        /// </summary>
        public void SetHealth(int playerIndex, float normalizedHealth)
        {
            normalizedHealth = Mathf.Clamp01(normalizedHealth);

            if (playerIndex == 0)
            {
                if (p1HealthFill != null)
                    p1HealthFill.fillAmount = normalizedHealth;
                p1HealthDrainTarget = normalizedHealth;
            }
            else
            {
                if (p2HealthFill != null)
                    p2HealthFill.fillAmount = normalizedHealth;
                p2HealthDrainTarget = normalizedHealth;
            }
        }

        /// <summary>
        /// Updates stamina bar for a player.
        /// </summary>
        public void SetStamina(int playerIndex, float normalizedStamina)
        {
            normalizedStamina = Mathf.Clamp01(normalizedStamina);

            if (playerIndex == 0 && p1StaminaFill != null)
                p1StaminaFill.fillAmount = normalizedStamina;
            else if (playerIndex == 1 && p2StaminaFill != null)
                p2StaminaFill.fillAmount = normalizedStamina;
        }

        /// <summary>
        /// Updates combo counter display.
        /// </summary>
        public void SetComboCount(int playerIndex, int comboCount)
        {
            var text = playerIndex == 0 ? p1ComboText : p2ComboText;
            if (text == null) return;

            if (comboCount > 1)
            {
                text.text = $"{comboCount} HIT" + (comboCount > 1 ? "S" : "");
                text.gameObject.SetActive(true);
            }
            else
            {
                text.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the match timer display.
        /// </summary>
        public void SetTimer(float remainingSeconds)
        {
            if (timerText == null) return;

            int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
            int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// Sets the portrait sprite for a player.
        /// </summary>
        public void SetPortrait(int playerIndex, Sprite portrait)
        {
            var image = playerIndex == 0 ? p1Portrait : p2Portrait;
            if (image != null)
                image.sprite = portrait;
        }

        /// <summary>
        /// Updates round win indicators (activate child icons).
        /// </summary>
        public void SetRoundWins(int playerIndex, int wins)
        {
            var parent = playerIndex == 0 ? p1RoundWins : p2RoundWins;
            if (parent == null) return;

            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(i < wins);
            }
        }

        /// <summary>
        /// Resets HUD to default state for a new match/round.
        /// </summary>
        public void ResetHUD()
        {
            SetHealth(0, 1f);
            SetHealth(1, 1f);
            SetStamina(0, 1f);
            SetStamina(1, 1f);
            SetComboCount(0, 0);
            SetComboCount(1, 0);
            SetTimer(99f);

            if (p1HealthDrain != null)
            {
                p1HealthDrain.fillAmount = 1f;
                p1HealthDrainTarget = 1f;
            }
            if (p2HealthDrain != null)
            {
                p2HealthDrain.fillAmount = 1f;
                p2HealthDrainTarget = 1f;
            }
        }
    }
}
