using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoDemo.Data;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation.Pooling
{
    public sealed class InventorySlotPool
    {
        #region Fields

        private readonly InventorySlotView _prefab;
        private readonly Transform _parent;
        private readonly Stack<InventorySlotView> _available = new Stack<InventorySlotView>();

        #endregion

        #region Constructors

        public InventorySlotPool(InventorySlotView prefab, Transform parent)
        {
            if (!prefab)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            if (!parent)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            _prefab = prefab;
            _parent = parent;
        }

        #endregion

        #region Public Methods

        public InventorySlotView Rent(ItemData item, int amount)
        {
            InventorySlotView slot = _available.Count > 0
                ? _available.Pop()
                : UnityEngine.Object.Instantiate(_prefab, _parent);

            slot.transform.SetAsLastSibling();
            slot.gameObject.SetActive(true);
            slot.Setup(item, amount);

            return slot;
        }

        public void Return(InventorySlotView slot)
        {
            if (!slot)
            {
                return;
            }

            slot.ResetForPool();
            slot.gameObject.SetActive(false);
            _available.Push(slot);
        }

        #endregion
    }
}
