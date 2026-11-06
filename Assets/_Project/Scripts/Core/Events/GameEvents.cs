namespace VertigoDemo.Core.Events
{
    public readonly struct GameStateChanged
    {
        #region Fields

        public readonly GameState State;

        #endregion

        #region Constructors

        public GameStateChanged(GameState state)
        {
            State = state;
        }

        #endregion
    }

    public readonly struct SpinStarted
    {
        #region Fields

        public readonly SpinResult Result;

        #endregion

        #region Constructors

        public SpinStarted(SpinResult result)
        {
            Result = result;
        }

        #endregion
    }

    public readonly struct RewardGranted
    {
        #region Fields

        public readonly SpinResult Result;
        public readonly int TotalAmount;

        #endregion

        #region Constructors

        public RewardGranted(SpinResult result, int totalAmount)
        {
            Result = result;
            TotalAmount = totalAmount;
        }

        #endregion
    }

    public readonly struct ZoneTransitionRequested
    {
        #region Fields

        public readonly int Zone;
        public readonly bool IsReset;

        #endregion

        #region Constructors

        public ZoneTransitionRequested(
            int zone,
            bool isReset)
        {
            Zone = zone;
            IsReset = isReset;
        }

        #endregion
    }

    public readonly struct RunRewardsCleared
    {
    }

    public readonly struct PlayerInventoryChanged
    {
    }
}
