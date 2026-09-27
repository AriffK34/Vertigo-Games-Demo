using System;
using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public class SessionInventory
    {
        public event Action<ItemData, int> OnItemAdded;
        public event Action OnInventoryCleared;
        
        private Dictionary<ItemData, int> _inventory = new();
        public IReadOnlyDictionary<ItemData, int> Inventory => _inventory;
        
        public void AddItem(ItemData item, int amount)
        {
            if (!_inventory.TryAdd(item, amount))
            {
                _inventory[item] += amount;
            }
            OnItemAdded?.Invoke(item, _inventory[item]);
        }

        public void Clear()
        {
            _inventory.Clear();
            OnInventoryCleared?.Invoke();
        }
    }

}

