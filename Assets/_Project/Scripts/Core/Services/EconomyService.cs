using System;
using System.Collections.Generic;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public sealed class EconomyService : IEconomyReader, IPlayerInventoryReader
    {
        #region Fields

        private readonly ItemCollection _ownedItems = new ItemCollection();
        private readonly EconomyConfig _config;

        #endregion

        #region Properties

        public IReadOnlyDictionary<ItemData, int> Items
        {
            get
            {
                return _ownedItems.Amounts;
            }
        }

        public bool HasReviveFunds
        {
            get
            {
                return GetAmount(_config.ReviveCurrency) >= _config.ReviveCost;
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

        #region Constructors

        public EconomyService(EconomyConfig config)
        {
            if (!config)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (!config.ReviveCurrency)
            {
                throw new ArgumentException("Revive currency must be assigned.", nameof(config));
            }
            if (config.ReviveCost <= 0)
            {
                throw new ArgumentException("Revive cost must be greater than zero.", nameof(config));
            }
            _config = config;
        }

        #endregion

        #region Public Methods

        public int GetAmount(ItemData item)
        {
            return _ownedItems.GetAmount(item);
        }

        public void ClaimRewards(SessionInventory runInventory)
        {
            if (runInventory == null)
            {
                throw new ArgumentNullException(nameof(runInventory));
            }

            foreach (var reward in runInventory.Inventory)
            {
                _ = checked(_ownedItems.GetAmount(reward.Key) + reward.Value);
            }

            foreach (var reward in runInventory.Inventory)
            {
                _ownedItems.Add(reward.Key, reward.Value);
            }

            runInventory.Clear();
        }

        public bool TryRevive()
        {
            bool isSuccessful = _ownedItems.TrySpend(_config.ReviveCurrency, _config.ReviveCost);
            if (!isSuccessful)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
