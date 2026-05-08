using UnityEngine;
using StreetFighter.Core;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Singleton for hitstop / time freeze effects. Audio/physics continue.
    /// </summary>
    public class HitStopManager : MonoBehaviour
    {
        public static HitStopManager Instance { get; private set; }
        private float duration;
        private float timer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void TriggerHitStop(float hitStopDuration)
        {
            Instance?.StartHitStop(hitStopDuration);
        }

        private void StartHitStop(float duration)
        {
            this.duration = duration;
            timer = duration;
            Time.timeScale = 0f;
        }

        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.unscaledDeltaTime;
                if (timer <= 0)
                {
                    Time.timeScale = 1f;
                }
            }
        }

        // Network hook: Master client triggers, clients simulate locally
    }
}
