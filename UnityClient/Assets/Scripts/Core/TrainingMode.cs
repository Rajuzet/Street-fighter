using StreetFighter.Characters;
using UnityEngine;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 3: Training mode with dummy opponent, infinite resources,
    /// frame data display, and recording/playback for practice.
    /// </summary>
    public sealed class TrainingMode : GameModeBase
    {
        [Header("Training Settings")]
        [SerializeField]
        private bool infiniteHealth = true;

        [SerializeField]
        private bool infiniteStamina = true;

        [SerializeField]
        private bool displayFrameData = true;

        [SerializeField]
        private bool showHitboxes = false;

        [Header("Dummy")]
        [SerializeField]
        private DummyBehavior dummyBehavior = DummyBehavior.Standing;

        [SerializeField]
        private bool dummyBlock = false;

        [SerializeField]
        private bool dummyCounterAttack = false;

        [Header("Frame Data UI")]
        [SerializeField]
        private UnityEngine.UI.Text frameDataText;

        private int startupFrames;
        private int activeFrames;
        private int recoveryFrames;
        private int totalFrameAdvantage;
        private bool recording;
        private bool playback;

        public override void InitializeMode()
        {
            modeName = "Training";
            currentTimer = 0f; // No timer in training
        }

        /// <summary>
        /// Sets the player and dummy selections.
        /// </summary>
        public void SetSelections(CharacterRosterData player, int pColor,
                                  CharacterRosterData dummy, int dColor)
        {
            p1Roster = player;
            p1Color = pColor;
            p2Roster = dummy;
            p2Color = dColor;
        }

        public override void StartMatch()
        {
            ClearPlayers();
            SpawnPlayers(p1Roster, p1Color, p2Roster, p2Color);
            matchActive = true;

            if (infiniteHealth)
            {
                var p1Stats = p1Instance?.GetComponent<CharacterStats>();
                if (p1Stats != null)
                    p1Stats.SetInvulnerable(true);

                var p2Stats = p2Instance?.GetComponent<CharacterStats>();
                if (p2Stats != null)
                    p2Stats.SetInvulnerable(true);
            }

            if (infiniteStamina)
            {
                var p1Stamina = p1Instance?.GetComponent<StaminaSystem>();
                p1Stamina?.SetInfinite(true);

                var p2Stamina = p2Instance?.GetComponent<StaminaSystem>();
                p2Stamina?.SetInfinite(true);
            }

            ConfigureDummy();
        }

        public override void UpdateMatch()
        {
            if (displayFrameData && frameDataText != null)
            {
                frameDataText.text = $"Startup: {startupFrames}\n" +
                                     $"Active: {activeFrames}\n" +
                                     $"Recovery: {recoveryFrames}\n" +
                                     $"Advantage: {totalFrameAdvantage}";
            }
        }

        public override void EndMatch(int winnerPlayerIndex)
        {
            matchActive = false;
        }

        private void ConfigureDummy()
        {
            var dummyController = p2Instance?.GetComponent<PlayerController>();
            var dummyCombat = p2Instance?.GetComponent<PlayerCombatController>();

            if (dummyController != null)
            {
                dummyController.SetAIControlled(true);
            }

            // Dummy behavior state would be read by an AI or simple state machine
            // For now, we leave the dummy idle or controlled by external AI
        }

        /// <summary>
        /// Sets the dummy's behavior state.
        /// </summary>
        public void SetDummyBehavior(DummyBehavior behavior)
        {
            dummyBehavior = behavior;
            ConfigureDummy();
        }

        /// <summary>
        /// Toggles hitbox visualization.
        /// </summary>
        public void ToggleHitboxes()
        {
            showHitboxes = !showHitboxes;
            // Hitbox visualization would integrate with HitDetectionSystem gizmos
        }

        /// <summary>
        /// Updates frame data display from a combat move.
        /// </summary>
        public void UpdateFrameData(int startup, int active, int recovery, int advantage)
        {
            startupFrames = startup;
            activeFrames = active;
            recoveryFrames = recovery;
            totalFrameAdvantage = advantage;
        }

        /// <summary>
        /// Resets both characters to spawn positions.
        /// </summary>
        public void ResetPositions()
        {
            if (p1Instance != null && p1SpawnPoint != null)
                p1Instance.transform.SetPositionAndRotation(p1SpawnPoint.position, p1SpawnPoint.rotation);

            if (p2Instance != null && p2SpawnPoint != null)
                p2Instance.transform.SetPositionAndRotation(p2SpawnPoint.position, p2SpawnPoint.rotation);

            // Reset stats
            var p1Stats = p1Instance?.GetComponent<CharacterStats>();
            p1Stats?.ResetHealth();

            var p2Stats = p2Instance?.GetComponent<CharacterStats>();
            p2Stats?.ResetHealth();
        }

        /// <summary>
        /// Starts recording player input for playback.
        /// </summary>
        public void StartRecording()
        {
            recording = true;
            playback = false;
        }

        /// <summary>
        /// Stops recording and prepares for playback.
        /// </summary>
        public void StopRecording()
        {
            recording = false;
        }

        /// <summary>
        /// Starts playback of recorded dummy behavior.
        /// </summary>
        public void StartPlayback()
        {
            playback = true;
            recording = false;
        }
    }

    public enum DummyBehavior
    {
        Standing,
        Crouching,
        Jumping,
        Walking,
        Blocking,
        CounterAttack,
        Playback
    }
}
