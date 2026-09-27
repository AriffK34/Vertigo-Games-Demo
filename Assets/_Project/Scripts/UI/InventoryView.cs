using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private InventorySlotView slotPrefab;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private ScrollRect scrollRect;
        
        private readonly Dictionary<ItemData, InventorySlotView> _slotViews = new();
        private SessionInventory _inventory;

        private void OnValidate()
        {
            if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        }

        public void Initialize(SessionInventory inventory)
        {
            if (_inventory != null)
            {
                _inventory.OnItemAdded -= HandleItemAdded;
                _inventory.OnInventoryCleared -= HandleInventoryCleared;
            }

            _inventory = inventory;
            _inventory.OnItemAdded += HandleItemAdded;
            _inventory.OnInventoryCleared += HandleInventoryCleared;
        }

        private void OnDestroy()
        {
            if (_inventory != null)
            {
                _inventory.OnItemAdded -= HandleItemAdded;
                _inventory.OnInventoryCleared -= HandleInventoryCleared;
            }
        }

        private void HandleItemAdded(ItemData item, int amount)
        {
            if (_slotViews.TryGetValue(item, out var slotView))
            {
                slotView.UpdateAmount(amount);
            }
            else
            {
                var newSlot = Instantiate(slotPrefab, slotsParent);
                newSlot.Setup(item, amount);
                _slotViews.Add(item, newSlot);
                
                Canvas.ForceUpdateCanvases(); // Grid boyutunu anında hesaplat
                scrollRect.DOHorizontalNormalizedPos(1f, 0.4f).SetEase(Ease.OutQuad);
            }
        }
        
        private void HandleInventoryCleared()
        {
            foreach (var slotView in _slotViews.Values)
            {
                if (slotView != null) Destroy(slotView.gameObject);
            }
            _slotViews.Clear();
        }
    }
}