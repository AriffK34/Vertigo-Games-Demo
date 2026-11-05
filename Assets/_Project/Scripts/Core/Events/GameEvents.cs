namespace VertigoDemo.Core.Events
{
    public readonly struct GameStateChanged
    {
        public readonly GameState State;

        public GameStateChanged(GameState state)
        {
            State = state;
        }
    }

    public readonly struct SpinStarted
    {
        public readonly SpinResult Result;

        public SpinStarted(SpinResult result)
        {
            Result = result;
        }
    }

    public readonly struct RewardGranted
    {
        public readonly SpinResult Result;
        public readonly int TotalAmount;

        public RewardGranted(SpinResult result, int totalAmount)
        {
            Result = result;
            TotalAmount = totalAmount;
        }
    }

    public readonly struct ZoneTransitionRequested
    {
        public readonly int Zone;
        public readonly bool IsReset;

        public ZoneTransitionRequested(
            int zone,
            bool isReset)
        {
            Zone = zone;
            IsReset = isReset;
        }
    }

    public readonly struct RunRewardsCleared
    {
    }

    public readonly struct BalancesChanged
    {
    }
}
