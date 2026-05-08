using System;

namespace StreetFighter.Core
{
    /// <summary>
    /// Maintains the current application state and broadcasts state transitions.
    /// </summary>
    public sealed class GameStateManager
    {
        private static readonly Lazy<GameStateManager> LazyInstance = new(() => new GameStateManager());

        private GameStateManager()
        {
            CurrentState = GameState.Uninitialized;
        }

        /// <summary>
        /// Singleton instance for the global game state manager.
        /// </summary>
        public static GameStateManager Instance => LazyInstance.Value;

        /// <summary>
        /// Current active state for the game lifecycle.
        /// </summary>
        public GameState CurrentState { get; private set; }

        /// <summary>
        /// Raised when the state machine changes state.
        /// </summary>
        public event Action<GameState> StateChanged;

        /// <summary>
        /// Sets the current game state and notifies listeners.
        /// </summary>
        /// <param name="newState">The state to transition into.</param>
        public void SetState(GameState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            CurrentState = newState;
            StateChanged?.Invoke(newState);
        }
    }

    /// <summary>
    /// Defines the top-level game lifecycle states.
    /// </summary>
    public enum GameState
    {
        Uninitialized,
        Bootstrap,
        MainMenu,
        Loading,
        InGame,
        Paused,
        Shutdown
    }
}
