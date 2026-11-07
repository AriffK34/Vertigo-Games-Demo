using VertigoDemo.Data;

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

        ItemData ReviveCurrency
        {
            get;
        }

        #endregion
    }
}
