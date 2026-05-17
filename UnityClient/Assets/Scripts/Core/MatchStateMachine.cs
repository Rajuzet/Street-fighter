using System;
using UnityEngine;
using UnityEngine.Events;

namespace StreetFighter.Core
{
    /// <summary>
    /// Phase 3: Central match state machine driving the flow from intro to results.
    /// Controls round start, fighting, pause, K.O., and match end states.
    /// </summary>
    public sealed class MatchStateMachine : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent OnIntroStarted;
        public UnityEvent OnRoundStarted;
        public UnityEvent OnFightStarted;
        public UnityEvent OnRoundEnded;
        public UnityEvent OnMatchEnded;
        public UnityEvent OnPaused;
        public UnityEvent OnResumed;

        [Header("Timing")]
        [SerializeField]
        private float introDuration = 3f;

        [SerializeField]
        private float roundStartDelay = 2f;

        [SerializeField]
        private float roundEndDelay = 3f;

        private MatchState currentState = MatchState.None;
        private float stateTimer;
        private bool isPaused;

        public MatchState CurrentState => currentState;
        public bool IsPaused => isPaused;

        /// <summary>
        /// Begins the full match flow from intro.
        /// </summary>
        public void StartMatch()
        {
            TransitionTo(MatchState.Intro);
        }

        /// <summary>
        /// Transitions to a specific state with optional timer.
        /// </summary>
        public void TransitionTo(MatchState newState)
        {
            ExitState(currentState);
            currentState = newState;
            EnterState(newState);
        }

        private void EnterState(MatchState state)
        {
            switch (state)
            {
                case MatchState.Intro:
                    stateTimer = introDuration;
                    OnIntroStarted?.Invoke();
                    break;

                case MatchState.RoundStart:
                    stateTimer = roundStartDelay;
                    OnRoundStarted?.Invoke();
                    break;

                case MatchState.Fighting:
                    OnFightStarted?.Invoke();
                    break;

                case MatchState.RoundEnd:
                    stateTimer = roundEndDelay;
                    OnRoundEnded?.Invoke();
                    break;

                case MatchState.MatchEnd:
                    OnMatchEnded?.Invoke();
                    break;
            }
        }

        private void ExitState(MatchState state)
        {
            // Cleanup per state if needed
        }

        private void Update()
        {
            if (isPaused) return;

            if (stateTimer > 0f)
            {
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    OnStateTimerExpired();
                }
            }
        }

        private void OnStateTimerExpired()
        {
            switch (currentState)
            {
                case MatchState.Intro:
                    TransitionTo(MatchState.RoundStart);
                    break;

                case MatchState.RoundStart:
                    TransitionTo(MatchState.Fighting);
                    break;

                case MatchState.RoundEnd:
                    // Transition handled by game mode (next round or match end)
                    break;
            }
        }

        /// <summary>
        /// Called by game mode when a round K.O. occurs.
        /// </summary>
        public void NotifyRoundEnd()
        {
            if (currentState == MatchState.Fighting)
                TransitionTo(MatchState.RoundEnd);
        }

        /// <summary>
        /// Called by game mode when the full match is decided.
        /// </summary>
        public void NotifyMatchEnd()
        {
            TransitionTo(MatchState.MatchEnd);
        }

        /// <summary>
        /// Toggles pause state.
        /// </summary>
        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;

            if (isPaused)
                OnPaused?.Invoke();
            else
                OnResumed?.Invoke();
        }

        /// <summary>
        /// Resets state machine for a new match.
        /// </summary>
        public void ResetMachine()
        {
            isPaused = false;
            Time.timeScale = 1f;
            TransitionTo(MatchState.None);
        }
    }

    public enum MatchState
    {
        None,
        Intro,
        RoundStart,
        Fighting,
        RoundEnd,
        MatchEnd
    }
}
