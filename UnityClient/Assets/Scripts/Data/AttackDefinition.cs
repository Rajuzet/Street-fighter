using UnityEngine;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Frame-perfect combat move definition with timing, damage, and effects.
    /// </summary>
    public sealed class AttackDefinition : ScriptableObject
    {
        [Header("General")]
        [SerializeField]
        private string attackId = "";

        [SerializeField]
        private string displayName = "";

        [Header("Timing")]
        [SerializeField]
        private float startupFrames = 6f;

        [SerializeField]
        private float activeFrames = 4f;

        [SerializeField]
        private float recoveryFrames = 12f;

        [SerializeField]
        private float cancelWindow = 2f;

        [Header("Damage")]
        [SerializeField]
        private float baseDamage = 10f;

        [SerializeField]
        private float guardDamage = 2f;

        [SerializeField]
        private bool isBlockable = true;

        [Header("Physics")]
        [SerializeField]
        private Vector3 hitboxOffset = Vector3.zero;

        [SerializeField]
        private Vector3 hitboxSize = new(0.5f, 0.5f, 0.5f);

        [SerializeField]
        private float knockbackForce = 5f;

        [SerializeField]
        private float knockbackDuration = 0.2f;

        [Header("Stamina")]
        [SerializeField]
        private float staminaCost = 10f;

        [Header("Animation")]
        [SerializeField]
        private string animationTrigger = "";

        [SerializeField]
        private float animationSpeed = 1f;

        [Header("Effects")]
        [SerializeField]
        private float cameraShakeMagnitude = 0.2f;

        [SerializeField]
        private float hitStopDuration = 0.05f;

        [SerializeField]
        private bool hasHitEffect = true;

        [Header("Advanced")]
        [SerializeField] private float parryWindow = 0.1f;
        [SerializeField] private bool hasIFrames = false;

[Header("Audio")]
        [SerializeField]
        private string attackSoundKey = "";

        [SerializeField]
        private string hitSoundKey = "";

        [Header("Critical & Polish")]
        [SerializeField] private float criticalChance = 0.1f;
        [SerializeField] private float criticalMultiplier = 1.5f;
        [SerializeField] private float screenFlashIntensity = 0.5f;
        [SerializeField] private float slowMoDuration = 0.3f;
        [SerializeField] private string hitVFXKey = "";
        [SerializeField] private string[] impactSFXLayer = new string[0];

        public string AttackId => attackId;
        public string DisplayName => displayName;
        public float StartupFrames => startupFrames;
        public float ActiveFrames => activeFrames;
        public float RecoveryFrames => recoveryFrames;
        public float TotalFrames => startupFrames + activeFrames + recoveryFrames;
        public float CancelWindow => cancelWindow;
        public float BaseDamage => baseDamage;
        public float GuardDamage => guardDamage;
        public bool IsBlockable => isBlockable;
        public Vector3 HitboxOffset => hitboxOffset;
        public Vector3 HitboxSize => hitboxSize;
        public float KnockbackForce => knockbackForce;
        public float KnockbackDuration => knockbackDuration;
        public float StaminaCost => staminaCost;
        public string AnimationTrigger => animationTrigger;
        public float AnimationSpeed => animationSpeed;
        public float CameraShakeMagnitude => cameraShakeMagnitude;
        public float HitStopDuration => hitStopDuration;
        public bool HasHitEffect => hasHitEffect;
        public string AttackSoundKey => attackSoundKey;
        public string HitSoundKey => hitSoundKey;
        public float CriticalChance => criticalChance;
        public float CriticalMultiplier => criticalMultiplier;
        public float ScreenFlashIntensity => screenFlashIntensity;
        public float SlowMoDuration => slowMoDuration;
        public string HitVFXKey => hitVFXKey;
        public string[] ImpactSFXLayer => impactSFXLayer;
    }
}
