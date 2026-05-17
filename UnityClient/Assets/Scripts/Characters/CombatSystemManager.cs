using System.Collections;
using System.Collections.Generic;
using StreetFighter.Combat;
using StreetFighter.Core;
using StreetFighter.Data;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Authoritative combat manager that executes moves, combo chains, and damage application.
    /// Phase 2: Integrated HitDetectionSystem, StaminaSystem, and DamageSystem for complete combat loop.
    /// </summary>
    public sealed class CombatSystemManager : MonoBehaviour
    {
        [SerializeField]
        private List<CombatMoveData> moves = new();

        [Header("Phase 2 Systems")]
        [SerializeField]
        private HitDetectionSystem hitDetection;

        [SerializeField]
        private StaminaSystem stamina;

        [SerializeField]
        private DamageSystem damageSystem;

        private readonly Dictionary<string, CombatMoveData> moveLookup = new();
        private ComboChain comboChain;
        private int currentComboCount = 0;
        private float comboResetTimer;
        private const float ComboResetDelay = 2f;
        private IEventBus eventBus;

        private void Awake()
        {
            PopulateMoves();
            comboChain = new ComboChain();
            eventBus = ServiceLocator.Resolve<IEventBus>();

            // Auto-get components if not assigned
            if (hitDetection == null)
                hitDetection = GetComponent<HitDetectionSystem>();
            if (stamina == null)
                stamina = GetComponent<StaminaSystem>();
            if (damageSystem == null)
                damageSystem = GetComponent<DamageSystem>();
        }

        private void Update()
        {
            // Reset combo after delay
            if (comboResetTimer > 0f)
            {
                comboResetTimer -= Time.deltaTime;
                if (comboResetTimer <= 0f)
                {
                    currentComboCount = 0;
                    damageSystem?.ResetCombo();
                }
            }
        }

        /// <summary>
        /// Executes the next move in the current combo sequence.
        /// Phase 2: Validates stamina, applies combo scaling, handles critical hits.
        /// </summary>
        public void ExecuteMove(string moveId)
        {
            if (!moveLookup.TryGetValue(moveId, out CombatMoveData moveData))
            {
                Debug.LogWarning($"Combat move '{moveId}' not found.");
                return;
            }

            var attackDef = moveData as AttackDefinition;
            if (attackDef == null)
            {
                Debug.LogWarning($"AttackDefinition for '{moveId}' was null.");
                return;
            }

            // Validate stamina
            if (stamina != null && !stamina.TryConsumeStamina(attackDef.StaminaCost))
            {
                Debug.Log("Not enough stamina!");
                return;
            }

            // Increment combo
            currentComboCount++;
            comboResetTimer = ComboResetDelay;
            damageSystem?.IncrementCombo();

            // Publish pre-attack event
            eventBus?.Publish(new CombatMoveExecutedEvent(moveData));

            // Start hit detection
            if (hitDetection != null)
            {
                hitDetection.StartAttack(attackDef);
            }

            // Combo scaling and critical hits
            float comboMultiplier = 1f + 0.1f * (currentComboCount - 1);
            bool isCritical = Random.value < attackDef.CriticalChance;
            
            if (isCritical)
            {
                comboMultiplier *= attackDef.CriticalMultiplier;
                HitStopManager.TriggerHitStop(attackDef.HitStopDuration * 1.5f);
                CameraShake.Shake(attackDef.CameraShakeMagnitude * 2f);
                eventBus?.Publish(new ComboCriticalEvent(currentComboCount, attackDef));
            }
            else
            {
                HitStopManager.TriggerHitStop(attackDef.HitStopDuration);
                CameraShake.Shake(attackDef.CameraShakeMagnitude);
            }

            // Play attack sound
            var audioService = ServiceLocator.Resolve<IAudioService>();
            audioService?.PlaySound(attackDef.AttackSoundKey, transform.position);

            // Add to combo chain
            comboChain.AddMove(attackDef);
            StartCoroutine(comboChain.Playback(attackDef, OnComboComplete));

            // Update animation combo count
            var animController = GetComponent<CharacterAnimationController>();
            animController?.SetComboCount(currentComboCount);
        }

        /// <summary>
        /// Cancels any active combo sequence.
        /// </summary>
        public void CancelCombo()
        {
            comboChain.Clear();
            currentComboCount = 0;
            damageSystem?.ResetCombo();

            var animController = GetComponent<CharacterAnimationController>();
            animController?.SetComboCount(0);
        }

        /// <summary>
        /// Gets the current combo count.
        /// </summary>
        public int GetComboCount() => currentComboCount;

        private void PopulateMoves()
        {
            moveLookup.Clear();
            foreach (var move in moves)
            {
                if (move == null)
                    continue;

                moveLookup[move.MoveId] = move;
            }
        }

        private void OnComboComplete()
        {
            comboChain.Clear();
        }
    }
}
