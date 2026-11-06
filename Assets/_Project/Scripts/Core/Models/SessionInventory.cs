using System.Collections.Generic;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public class SessionInventory : IRunRewardsReader
    {
        #region Fields

        private readonly ItemCollection _items = new ItemCollection();

        #endregion

        #region Properties

        public IReadOnlyDictionary<ItemData, int> Inventory
        {
            get
            {
                return _items.Amounts;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(ItemData item, int amount)
        {
            _items.Add(item, amount);
        }

        public void Clear()
        {
            _items.Clear();
        }

        #endregion
    }
}
