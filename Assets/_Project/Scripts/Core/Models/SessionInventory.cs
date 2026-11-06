using System.Collections.Generic;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public class SessionInventory : IRunRewardsReader
    {
        #region Fields

        private readonly RewardStore _rewardStore = new RewardStore();

        #endregion

        #region Properties

        public IReadOnlyDictionary<ItemData, int> Inventory
        {
            get
            {
                return _rewardStore.Amounts;
            }
        }

        #endregion

        #region Methods

        public void AddItem(ItemData item, int amount)
        {
            _rewardStore.Add(item, amount);
        }

        public void Clear()
        {
            _rewardStore.Clear();
        }

        #endregion
    }

}

