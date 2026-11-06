using System;
using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public sealed class RewardStore
    {
        #region Fields

        private readonly Dictionary<ItemData, int> _amounts = new Dictionary<ItemData, int>();

        #endregion

        #region Properties

        public IReadOnlyDictionary<ItemData, int> Amounts
        {
            get
            {
                return _amounts;
            }
        }

        #endregion

        #region Public Methods

        public int GetAmount(ItemData item)
        {
            if (!item)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return _amounts.GetValueOrDefault(item, 0);
        }

        public void Add(ItemData item, int amount)
        {
            ValidateRequest(item, amount);
            _amounts[item] = checked(GetAmount(item) + amount);
        }

        public bool TrySpend(ItemData item, int amount)
        {
            ValidateRequest(item, amount);

            int currentAmount = GetAmount(item);
            if (currentAmount < amount)
            {
                return false;
            }

            int remainingAmount = currentAmount - amount;
            if (remainingAmount == 0)
            {
                _amounts.Remove(item);
            }
            else
            {
                _amounts[item] = remainingAmount;
            }
            return true;
        }

        public void Clear()
        {
            _amounts.Clear();
        }

        #endregion

        #region Private Methods

        private static void ValidateRequest(ItemData item, int amount)
        {
            if (!item)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            }
        }

        #endregion
    }
}
