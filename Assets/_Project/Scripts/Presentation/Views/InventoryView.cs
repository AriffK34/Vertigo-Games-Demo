using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class InventoryView : MonoBehaviour
    {
        public event Action<InventorySlotView> OnRewardSlotReady;
        
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
            if (_slotViews.TryGetValue(item, out InventorySlotView slotView))
            {
                slotView.UpdateAmount(amount);
            }
            else
            {
                slotView = Instantiate(slotPrefab, slotsParent);
                slotView.Setup(item, amount);
                _slotViews.Add(item, slotView);
            }

            RevealSlot(slotView);
            OnRewardSlotReady?.Invoke(slotView);
        }
        
        private void HandleInventoryCleared()
        {
            scrollRect.enabled = true;
            scrollRect.StopMovement();
            foreach (var slotView in _slotViews.Values)
            {
                if (slotView != null)
                {
                    Destroy(slotView.gameObject);
                }
            }
            _slotViews.Clear();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        private void RevealSlot(InventorySlotView slotView)
        {
            scrollRect.StopMovement();
            Canvas.ForceUpdateCanvases();

            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;
            RectTransform slotTransform = (RectTransform)slotView.transform;

            float hiddenHeight = content.rect.height - viewport.rect.height;
            if (hiddenHeight > 0f)
            {
                Vector3 slotCenter = content.InverseTransformPoint(
                    slotTransform.TransformPoint(slotTransform.rect.center));

                float distanceFromTop = content.rect.yMax - slotCenter.y;
                float scrollOffset = distanceFromTop - viewport.rect.height * 0.5f;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f - scrollOffset / hiddenHeight);
            }
            else
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
            Canvas.ForceUpdateCanvases();
            scrollRect.enabled = false;
        }

        public void CompleteRewardPresentation()
        {
            scrollRect.enabled = true;
        }
    }
}