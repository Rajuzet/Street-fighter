using UnityEngine;
using UnityEngine.UI;

namespace StreetFighter.UI
{
    /// <summary>
    /// Phase 3: Displays a character portrait with optional tint/color overlay
    /// and animated transitions (fade, scale pop) for UI screens.
    /// </summary>
    public sealed class CharacterPortrait : MonoBehaviour
    {
        [Header("Portrait")]
        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Image tintOverlay;

        [Header("Animation")]
        [SerializeField]
        private float popScale = 1.15f;

        [SerializeField]
        private float popDuration = 0.15f;

        [SerializeField]
        private float fadeDuration = 0.2f;

        private Vector3 baseScale;
        private Coroutine currentAnimation;

        private void Awake()
        {
            if (portraitImage != null)
                baseScale = portraitImage.rectTransform.localScale;
        }

        /// <summary>
        /// Sets the portrait sprite and optionally applies a tint color.
        /// </summary>
        public void SetPortrait(Sprite portrait, Color? tintColor = null)
        {
            if (portraitImage != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.color = Color.white;
            }

            if (tintOverlay != null)
            {
                tintOverlay.color = tintColor ?? Color.clear;
            }

            PlayPopAnimation();
        }

        /// <summary>
        /// Sets the background color (e.g., for player identity).
        /// </summary>
        public void SetBackgroundColor(Color color)
        {
            if (backgroundImage != null)
                backgroundImage.color = color;
        }

        /// <summary>
        /// Fades the portrait in or out.
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (portraitImage == null) return;

            float targetAlpha = visible ? 1f : 0f;
            portraitImage.canvasRenderer.SetAlpha(visible ? 0f : 1f);
            portraitImage.CrossFadeAlpha(targetAlpha, fadeDuration, false);
        }

        /// <summary>
        /// Plays a brief scale-pop animation on the portrait.
        /// </summary>
        public void PlayPopAnimation()
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);

            currentAnimation = StartCoroutine(PopCoroutine());
        }

        private System.Collections.IEnumerator PopCoroutine()
        {
            if (portraitImage == null) yield break;

            var rectTransform = portraitImage.rectTransform;
            float elapsed = 0f;

            // Scale up
            while (elapsed < popDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / popDuration;
                float scale = Mathf.Lerp(1f, popScale, Mathf.Sin(t * Mathf.PI * 0.5f));
                rectTransform.localScale = baseScale * scale;
                yield return null;
            }

            // Scale back
            elapsed = 0f;
            while (elapsed < popDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / popDuration;
                float scale = Mathf.Lerp(popScale, 1f, Mathf.Sin(t * Mathf.PI * 0.5f));
                rectTransform.localScale = baseScale * scale;
                yield return null;
            }

            rectTransform.localScale = baseScale;
            currentAnimation = null;
        }

        /// <summary>
        /// Resets the portrait to default state.
        /// </summary>
        public void Clear()
        {
            if (portraitImage != null)
            {
                portraitImage.sprite = null;
                portraitImage.color = Color.white;
                portraitImage.rectTransform.localScale = baseScale;
            }

            if (tintOverlay != null)
                tintOverlay.color = Color.clear;
        }
    }
}
