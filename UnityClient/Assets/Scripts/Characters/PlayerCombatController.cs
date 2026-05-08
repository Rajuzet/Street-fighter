using StreetFighter.Core;
using StreetFighter.Input;
using UnityEngine;

namespace StreetFighter.Characters
{
    /// <summary>
    /// Handles player combat input, blocking, and combo initiation.
    /// </summary>
    public sealed class PlayerCombatController : MonoBehaviour
    {
        [SerializeField]
        private CombatSystemManager combatSystem = null;

        [SerializeField]
        private CharacterAnimationController animationController = null;

        private IInputService inputService;
        private bool isBlocking;

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
            if (inputService.BlockHeld)
            {
                isBlocking = true;
                animationController.SetBlocking(true);
                return;
            }

            if (isBlocking)
            {
                isBlocking = false;
                animationController.SetBlocking(false);
            }

            if (inputService.AttackPressed)
            {
                animationController.TriggerAttack();
                combatSystem.ExecuteMove("punch_light");
            }
        }
    }
}
