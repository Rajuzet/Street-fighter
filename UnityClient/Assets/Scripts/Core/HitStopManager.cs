using System.Collections;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 2: HitStopManager provides global hit-stop (frame freeze) effects on impactful hits.
    /// Creates fighting-game style impact feel by briefly pausing time.
    /// </summary>
    public sealed class HitStopManager : MonoBehaviour
    {
        [Header("Hit Stop Settings")]
        [SerializeField]
        private float defaultHitStopDuration = 0.08f;

        [SerializeField]
        private float maxHitStopDuration = 0.3f;

        [SerializeField]
        private AnimationCurve hitStopCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Time Scale")]
        [SerializeField]
        private float hitStopTimeScale = 0.05f;

        private static HitStopManager instance;
        private bool isInHitStop;
        private Coroutine currentHitStop;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        /// <summary>
        /// Triggers a hit-stop effect with default duration.
        /// </summary>
        public static void TriggerHitStop()
        {
            instance?.InternalTriggerHitStop(instance.defaultHitStopDuration);
        }

        /// <summary>
        /// Triggers a hit-stop effect with specified duration.
        /// </summary>
        public static void TriggerHitStop(float duration)
        {
            instance?.InternalTriggerHitStop(duration);
        }

        /// <summary>
        /// Triggers a hit-stop with strength-based scaling.
        /// </summary>
        public static void TriggerHitStop(float baseDuration, float strength)
        {
            float scaledDuration = Mathf.Min(baseDuration * strength, instance?.maxHitStopDuration ?? baseDuration);
            instance?.InternalTriggerHitStop(scaledDuration);
        }

        private void InternalTriggerHitStop(float duration)
        {
            if (isInHitStop && currentHitStop != null)
            {
                StopCoroutine(currentHitStop);
            }

            currentHitStop = StartCoroutine(HitStopCoroutine(duration));
        }

        private IEnumerator HitStopCoroutine(float duration)
        {
            isInHitStop = true;
            float originalTimeScale = Time.timeScale;
            float timer = 0f;

            Time.timeScale = hitStopTimeScale;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float t = timer / duration;
                float curveValue = hitStopCurve.Evaluate(t);
                Time.timeScale = Mathf.Lerp(hitStopTimeScale, originalTimeScale, curveValue);
                yield return null;
            }

            Time.timeScale = originalTimeScale;
            isInHitStop = false;
            currentHitStop = null;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                Time.timeScale = 1f;
            }
        }
    }
}
