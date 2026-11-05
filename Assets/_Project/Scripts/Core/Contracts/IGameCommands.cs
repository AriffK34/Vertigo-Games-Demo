namespace VertigoDemo.Core.Contracts
{
    public interface IGameCommands
    {
        bool TrySpin();
        void RequestLeave();
        void ClaimRewards();
        void GiveUp();
        bool TryRevive();
    }
}