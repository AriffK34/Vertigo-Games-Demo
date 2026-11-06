namespace VertigoDemo.Core.Contracts
{
    public interface IGameFlowCallbacks
    {
        #region Public Methods

        void CompleteSpin();

        void CompleteRewardPresentation();

        void CompleteZoneTransition();

        #endregion
    }
}
