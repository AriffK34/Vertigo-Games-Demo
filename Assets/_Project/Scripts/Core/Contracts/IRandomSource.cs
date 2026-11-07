namespace VertigoDemo.Core.Contracts
{
    public interface IRandomSource
    {
        #region Public Methods

        int Next(int maxExclusive);

        #endregion
    }
}
