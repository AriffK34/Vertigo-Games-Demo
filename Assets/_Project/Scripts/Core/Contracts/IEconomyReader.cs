using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core.Contracts
{
    public interface IEconomyReader
    {
        #region Properties

        IReadOnlyDictionary<ItemData, int> Balances
        {
            get;
        }

        bool HasReviveFunds
        {
            get;
        }

        ItemData ReviveCurrency
        {
            get;
        }

        int ReviveCost
        {
            get;
        }

        #endregion

        #region Public Methods

        int GetBalance(ItemData item);

        #endregion
    }
}
