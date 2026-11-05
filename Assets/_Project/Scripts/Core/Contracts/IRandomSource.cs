namespace VertigoDemo.Core.Contracts
{
    public interface IRandomSource
    {
        int Next(int maxExclusive);
    }
}