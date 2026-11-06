namespace VertigoDemo.Core.Contracts
{
    public interface IGameCommands
    {
        #region Public Methods

        bool TrySpin();

        void RequestLeave();

        void ClaimRewards();

        void GiveUp();

        bool TryRevive();

        #endregion
    }
}
