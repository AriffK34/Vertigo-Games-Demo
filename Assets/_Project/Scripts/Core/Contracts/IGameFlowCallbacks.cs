namespace VertigoDemo.Core.Contracts
{
    public interface IGameFlowCallbacks
    {
        void CompleteSpin();
        void CompleteRewardPresentation();
        void CompleteZoneTransition();
    }
}
