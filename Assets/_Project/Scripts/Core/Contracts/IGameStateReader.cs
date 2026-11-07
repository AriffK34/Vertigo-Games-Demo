namespace VertigoDemo.Core.Contracts
{
    public interface IGameStateReader
    {
        #region Properties

        GameState CurrentState
        {
            get;
        }

        #endregion
    }
}
