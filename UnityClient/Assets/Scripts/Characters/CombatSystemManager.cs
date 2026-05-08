using System.Collections;
using System.Collections.Generic;
using StreetFighter.Data;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Authoritative combat manager that executes moves and combo chains.
    /// </summary>
    public sealed class CombatSystemManager : MonoBehaviour
    {
        [SerializeField]
        private List<CombatMoveData> moves = new();

        private readonly Dictionary<string, CombatMoveData> moveLookup = new();
        private ComboChain comboChain;
        private int currentComboCount = 0;
        private IEventBus eventBus;

        private void Awake()
        {
            PopulateMoves();
            comboChain = new ComboChain();
            eventBus = ServiceLocator.Resolve<IEventBus>();
        }

        /// <summary>
        /// Executes the next move in the current combo sequence.
        /// </summary>
        /// <param name="moveId">Move identifier.</param>
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
                Debug.LogWarning($"AttackDefinition for '{moveId}' was null (no Resources fallback in stabilization mode).");
                return;
            }


            var hitDetection = GetComponent<HitDetectionSystem>() ?? gameObject.AddComponent<HitDetectionSystem>();

            // Phase 2B.1: apply stamina cost at attack start (attacker side).
            var stamina = GetComponent<StaminaSystem>();
            if (stamina != null && !stamina.TryConsumeStamina(attackDef.StaminaCost))
            {
                return;
            }


            comboChain.AddMove(moveData);
            StartCoroutine(comboChain.Playback(moveData, OnComboComplete));

            hitDetection.StartAttack(attackDef);

            // Phase 2B: Combo scaling and critical hits
            currentComboCount++;

            float comboMultiplier = 1f + 0.1f * (currentComboCount - 1);
            bool isCritical = Random.value < attackDef.CriticalChance;
            if (isCritical)
            {
                comboMultiplier *= attackDef.CriticalMultiplier;
                HitStopManager.TriggerHitStop(attackDef.HitStopDuration * 1.5f);
                CameraShake.Shake(attackDef.CameraShakeMagnitude * 2f);
                // TODO: Screen flash, slowmo finisher
                eventBus?.Publish(new ComboCriticalEvent(currentComboCount, attackDef));
            }

            // VFX and layered audio
            ServiceLocator.Resolve<IAudioService>()?.PlaySFX(attackDef.ImpactSFXLayer?.Length > 0 ? attackDef.ImpactSFXLayer[0] : attackDef.HitSoundKey);
            // TODO: VFXPoolManager.SpawnDirectional(attackDef.HitVFXKey, transform.position + attackDef.HitboxOffset, hitNormal);

            // Effects hooks
            ServiceLocator.Resolve<IAudioService>()?.PlaySFX(attackDef.AttackSoundKey);
            // VFX spawn at hand position
        }

        /// <summary>
        /// Cancels any active combo sequence.
        /// </summary>
        public void CancelCombo()
        {
            comboChain.Clear();
            currentComboCount = 0;
        }

        private void PopulateMoves()
        {
            moveLookup.Clear();
            foreach (var move in moves)
            {
                if (move == null)
                {
                    continue;
                }

                moveLookup[move.MoveId] = move;
            }
        }

        private void OnComboComplete()
        {
            comboChain.Clear();
        }
    }
}
