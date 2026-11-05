namespace VertigoDemo.Core.Contracts
{
    public interface IGameStateReader
    {
        GameState CurrentState { get; }
    }
}