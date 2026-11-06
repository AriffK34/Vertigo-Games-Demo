namespace VertigoDemo.Core.Contracts
{
    public interface IEconomyReader
    {
        #region Properties

        bool HasReviveFunds
        {
            get;
        }

        int ReviveCost
        {
            get;
        }

        #endregion
    }
}
