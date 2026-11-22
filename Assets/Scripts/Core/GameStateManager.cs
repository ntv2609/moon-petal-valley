using UnityEngine;

namespace MoonpetalValley.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; } = GameState.Gameplay;

        public bool IsGameplayActive => CurrentState == GameState.Gameplay;
        public bool IsTimePaused => CurrentState != GameState.Gameplay;

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            GameEventBus.RaiseGameStateChanged(newState);
        }

        public void ReturnToGameplay()
        {
            SetState(GameState.Gameplay);
        }
    }
}
