using System;
using System.Collections.Generic;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public sealed class EconomyService : IEconomyReader
    {
        #region Fields

        private readonly RewardStore _balances = new RewardStore();
        private readonly EconomyConfig _config;

        #endregion

        #region Properties

        public IReadOnlyDictionary<ItemData, int> Balances
        {
            get
            {
                return _balances.Amounts;
            }
        }

        public bool HasReviveFunds
        {
            get
            {
                return GetBalance(_config.ReviveCurrency) >= _config.ReviveCost;
            }
        }

        public ItemData ReviveCurrency
        {
            get
            {
                return _config.ReviveCurrency;
            }
        }

        public int ReviveCost
        {
            get
            {
                return _config.ReviveCost;
            }
        }

        #endregion

        #region Constructor

        public EconomyService(EconomyConfig config)
        {
            if (!config)
                throw new ArgumentNullException(nameof(config));
            if (!config.ReviveCurrency)
                throw new ArgumentException("Revive currency must be assigned.", nameof(config));
            if (config.ReviveCost <= 0)
                throw new ArgumentException("Revive cost must be greater than zero.", nameof(config));
            _config = config;
        }

        #endregion

        #region Methods

        public int GetBalance(ItemData item)
        {
            return _balances.GetAmount(item);
        }

        public void ClaimRewards(SessionInventory runInventory)
        {
            if (runInventory == null) throw new ArgumentNullException(nameof(runInventory));

            foreach (var reward in runInventory.Inventory)
                _ = checked(_balances.GetAmount(reward.Key) + reward.Value);
            
            foreach (var reward in runInventory.Inventory)
                _balances.Add(reward.Key, reward.Value);
            
            runInventory.Clear();
        }

        public bool TryRevive()
        {
            bool isSuccessful = _balances.TrySpend(_config.ReviveCurrency, _config.ReviveCost);
            if (!isSuccessful) return false;
            return true;
        }

        #endregion
    }
}