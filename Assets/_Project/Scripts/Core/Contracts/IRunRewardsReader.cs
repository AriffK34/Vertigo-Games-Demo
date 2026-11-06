using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core.Contracts
{
    public interface IRunRewardsReader
    {
        #region Properties

        IReadOnlyDictionary<ItemData, int> Inventory
        {
            get;
        }

        #endregion
    }
}
