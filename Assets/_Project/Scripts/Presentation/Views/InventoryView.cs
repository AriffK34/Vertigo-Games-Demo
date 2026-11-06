using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class InventoryView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InventorySlotView slotPrefab;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private ScrollRect scrollRect;

        private readonly Dictionary<ItemData, InventorySlotView> _slotViews =
            new Dictionary<ItemData, InventorySlotView>();

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!scrollRect)
            {
                scrollRect = GetComponent<ScrollRect>();
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
                slotView = Instantiate(slotPrefab, slotsParent);
                slotView.Setup(item, amount);
                _slotViews.Add(item, slotView);
            }

            return slotView;
        }

        public void RevealSlot(InventorySlotView slotView)
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

                scrollRect.verticalNormalizedPosition =
                    Mathf.Clamp01(1f - scrollOffset / hiddenHeight);
            }
            else
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }

            Canvas.ForceUpdateCanvases();
            scrollRect.enabled = false;
        }

        public void Clear()
        {
            scrollRect.enabled = true;
            scrollRect.StopMovement();

            foreach (InventorySlotView slotView in _slotViews.Values)
            {
                if (slotView)
                {
                    slotView.gameObject.SetActive(false);
                    Destroy(slotView.gameObject);
                }
            }

            _slotViews.Clear();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        public void CompleteRewardPresentation()
        {
            scrollRect.enabled = true;
        }

        #endregion
    }
}