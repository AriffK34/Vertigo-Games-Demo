using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Pooling;

namespace VertigoDemo.UI
{
    public class InventoryView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InventorySlotView _slotPrefab;
        [SerializeField] private Transform _slotsParent;
        [SerializeField] private ScrollRect _scrollRect;

        private readonly Dictionary<ItemData, InventorySlotView> _slotViews =
            new Dictionary<ItemData, InventorySlotView>();
        
        private InventorySlotPool _slotPool;
        
        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!_scrollRect)
            {
                _scrollRect = GetComponent<ScrollRect>();
            }
        }

        #endregion

        #region Public Methods

        public void Refresh(IReadOnlyDictionary<ItemData, int> rewards)
        {
            Clear();

            foreach (var reward in rewards)
            {
                SetItem(reward.Key, reward.Value);
            }
        }

        public InventorySlotView SetItem(ItemData item, int amount)
        {
            if (_slotViews.TryGetValue(item, out InventorySlotView slotView))
            {
                slotView.UpdateAmount(amount);
            }
            else
            {
                if (_slotPool == null)
                {
                    _slotPool = new InventorySlotPool(_slotPrefab, _slotsParent);
                }

                slotView = _slotPool.Rent(item, amount);
                _slotViews.Add(item, slotView);
            }

            return slotView;
        }
        

        public void RevealSlot(InventorySlotView slotView)
        {
            _scrollRect.StopMovement();
            Canvas.ForceUpdateCanvases();

            RectTransform content = _scrollRect.content;
            RectTransform viewport = _scrollRect.viewport;
            RectTransform slotTransform = (RectTransform)slotView.transform;

            float hiddenHeight = content.rect.height - viewport.rect.height;

            if (hiddenHeight > 0f)
            {
                Vector3 slotCenter = content.InverseTransformPoint(
                    slotTransform.TransformPoint(slotTransform.rect.center));

                float distanceFromTop = content.rect.yMax - slotCenter.y;
                float scrollOffset = distanceFromTop - viewport.rect.height * 0.5f;

                _scrollRect.verticalNormalizedPosition =
                    Mathf.Clamp01(1f - scrollOffset / hiddenHeight);
            }
            else
            {
                _scrollRect.verticalNormalizedPosition = 1f;
            }

            Canvas.ForceUpdateCanvases();
            _scrollRect.enabled = false;
        }

        public void Clear()
        {
            _scrollRect.enabled = true;
            _scrollRect.StopMovement();

            foreach (InventorySlotView slotView in _slotViews.Values)
            {
                _slotPool.Return(slotView);
            }

            _slotViews.Clear();
            _scrollRect.verticalNormalizedPosition = 1f;
        }

        public void CompleteRewardPresentation()
        {
            _scrollRect.enabled = true;
        }

        #endregion
    }
}