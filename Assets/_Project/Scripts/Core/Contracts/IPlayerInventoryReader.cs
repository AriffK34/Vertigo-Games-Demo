using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core.Contracts
{
    public interface IPlayerInventoryReader
    {
        #region Properties

        IReadOnlyDictionary<ItemData, int> Items
        {
            get;
        }

        #endregion
    }
}