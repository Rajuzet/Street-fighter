using UnityEngine;
using Cinemachine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 2: CameraShake provides global camera shake effects using Cinemachine.
    /// Integrates with CombatSystemManager and HitDetectionSystem for impact feedback.
    /// </summary>
    public sealed class CameraShake : MonoBehaviour
    {
        [Header("Cinemachine Integration")]
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;

        [Header("Shake Profiles")]
        [SerializeField]
        private float defaultShakeAmplitude = 1.5f;

        [SerializeField]
        private float defaultShakeFrequency = 10f;

        [SerializeField]
        private float defaultShakeDuration = 0.2f;

        [SerializeField]
        private AnimationCurve shakeDecayCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        private CinemachineBasicMultiChannelPerlin noise;
        private static CameraShake instance;
        private bool isShaking;
        private float shakeTimer;
        private float currentDuration;
        private float currentAmplitude;
        private float currentFrequency;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            if (virtualCamera == null)
            {
                virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }

            if (virtualCamera != null)
            {
                noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                if (noise == null)
                {
                    noise = virtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                }
            }
        }

        private void Update()
        {
            if (!isShaking || noise == null)
                return;

            shakeTimer += Time.deltaTime;
            float t = shakeTimer / currentDuration;
            float decay = shakeDecayCurve.Evaluate(t);

            noise.m_AmplitudeGain = currentAmplitude * decay;
            noise.m_FrequencyGain = currentFrequency * decay;

            if (shakeTimer >= currentDuration)
            {
                StopShake();
            }
        }

        /// <summary>
        /// Triggers a default camera shake.
        /// </summary>
        public static void Shake()
        {
            if (instance != null)
            {
                instance.TriggerShake(
                    instance.defaultShakeAmplitude,
                    instance.defaultShakeFrequency,
                    instance.defaultShakeDuration
                );
            }
        }

        /// <summary>
        /// Triggers a camera shake with custom magnitude.
        /// </summary>
        public static void Shake(float magnitude)
        {
            if (instance != null)
            {
                instance.TriggerShake(
                    magnitude,
                    instance.defaultShakeFrequency,
                    instance.defaultShakeDuration
                );
            }
        }

        /// <summary>
        /// Triggers a fully custom camera shake.
        /// </summary>
        public static void Shake(float amplitude, float frequency, float duration)
        {
            instance?.TriggerShake(amplitude, frequency, duration);
        }

        private void TriggerShake(float amplitude, float frequency, float duration)
        {
            if (noise == null)
                return;

            currentAmplitude = amplitude;
            currentFrequency = frequency;
            currentDuration = duration;
            shakeTimer = 0f;
            isShaking = true;

            noise.m_AmplitudeGain = amplitude;
            noise.m_FrequencyGain = frequency;
        }

        private void StopShake()
        {
            isShaking = false;
            if (noise != null)
            {
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                StopShake();
            }
        }
    }
}
