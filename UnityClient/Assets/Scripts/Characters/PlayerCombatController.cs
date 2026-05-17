using StreetFighter.Combat;
using StreetFighter.Core;
using StreetFighter.Input;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Handles player combat input, blocking, combo initiation, and move type selection.
    /// Phase 2: Added kick, power, and special attack inputs with combo chaining rules.
    /// </summary>
    public sealed class PlayerCombatController : MonoBehaviour
    {
        [SerializeField]
        private CombatSystemManager combatSystem = null;

        [SerializeField]
        private CharacterAnimationController animationController = null;

        [SerializeField]
        private StaminaSystem staminaSystem = null;

        [Header("Input Mapping")]
        [SerializeField]
        private string punchMoveId = "punch_light";

        [SerializeField]
        private string kickMoveId = "kick_light";

        [SerializeField]
        private string powerMoveId = "power_heavy";

        [SerializeField]
        private string specialMoveId = "special_spinning";

        private IInputService inputService;
        private bool isBlocking;
        private bool wasAttackPressed;
        private bool wasKickPressed;
        private bool wasPowerPressed;
        private bool wasSpecialPressed;

        public bool IsBlocking => isBlocking;

        private void Awake()
        {
            inputService = ServiceLocator.Resolve<IInputService>();
        }

        private void Update()
        {
            if (inputService == null || combatSystem == null || animationController == null)
            {
                return;
            }

            HandleCombatInput();
        }

        private void HandleCombatInput()
        {
            // Blocking takes highest priority
            if (inputService.BlockHeld)
            {
                if (!isBlocking)
                {
                    isBlocking = true;
                    animationController.SetBlocking(true);
                    staminaSystem?.SetBlocking(true);
                }
                return;
            }

            if (isBlocking)
            {
                isBlocking = false;
                animationController.SetBlocking(false);
                staminaSystem?.SetBlocking(false);
            }

            // If staggered or stunned, can't attack
            var hitReaction = GetComponent<HitReactionSystem>();
            if (hitReaction != null && hitReaction.IsReacting)
                return;

            // Dodge input (Shift + Block without movement)
            if (inputService.SprintHeld && inputService.BlockHeld && inputService.Move.magnitude < 0.1f)
            {
                animationController.TriggerDodge();
                return;
            }

            // Special attack input (Alt/Right-click + forward)
            bool specialInput = inputService.AttackPressed && inputService.Move.magnitude > 0.8f && inputService.Move.y > 0.5f;
            if (specialInput && !wasSpecialPressed)
            {
                animationController.TriggerSpecial();
                combatSystem.ExecuteMove(specialMoveId);
                wasSpecialPressed = true;
                return;
            }
            wasSpecialPressed = specialInput;

            // Power attack input (Hold attack button)
            bool powerInput = inputService.AttackPressed && Time.time - lastAttackDownTime > 0.3f;
            if (powerInput && !wasPowerPressed)
            {
                animationController.TriggerPower();
                combatSystem.ExecuteMove(powerMoveId);
                wasPowerPressed = true;
                return;
            }
            wasPowerPressed = powerInput;

            // Kick input (E key or gamepad button - alternate attack)
            bool kickInput = UnityEngine.Input.GetKeyDown(KeyCode.E) || (inputService.AttackPressed && inputService.Move.magnitude > 0.1f);
            if (kickInput && !wasKickPressed)
            {
                animationController.TriggerKick();
                combatSystem.ExecuteMove(kickMoveId);
                wasKickPressed = true;
                return;
            }
            wasKickPressed = kickInput;

            // Standard punch/light attack
            if (inputService.AttackPressed && !wasAttackPressed)
            {
                lastAttackDownTime = Time.time;
                animationController.TriggerAttack();
                combatSystem.ExecuteMove(punchMoveId);
                wasAttackPressed = true;
                return;
            }
            wasAttackPressed = inputService.AttackPressed;
        }

        private float lastAttackDownTime;
    }
}
