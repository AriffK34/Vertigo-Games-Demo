namespace VertigoDemo.Core
{
    public sealed class GameStateMachine
    {
        #region Fields

        private GameState _currentState = GameState.Resetting;

        #endregion

        #region Properties

        public GameState CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        #endregion

        #region Public Methods

        public bool TryTransition(GameState nextState)
        {
            if (!IsTransitionAllowed(_currentState, nextState))
            {
                return false;
            }

            _currentState = nextState;
            return true;
        }

        #endregion

        #region Private Methods

        private static bool IsTransitionAllowed(GameState currentState, GameState nextState)
        {
            switch (currentState)
            {
                case GameState.Resetting:
                case GameState.ZoneTransition:
                    return nextState == GameState.Idle;

                case GameState.Idle:
                    return nextState == GameState.Spinning ||
                           nextState == GameState.RewardsClaiming ||
                           nextState == GameState.InventoryViewing;

                case GameState.Spinning:
                    return nextState == GameState.RewardGiving || nextState == GameState.BombExploded;

                case GameState.RewardGiving:
                    return nextState == GameState.ZoneTransition;

                case GameState.RewardsClaiming:
                    return nextState == GameState.Resetting;

                case GameState.BombExploded:
                    return nextState == GameState.Resetting || nextState == GameState.Reviving;

                case GameState.Reviving:
                case GameState.InventoryViewing:
                    return nextState == GameState.Idle;

                default:
                    return false;
            }
        }

        #endregion
    }
}
