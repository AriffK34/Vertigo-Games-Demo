using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class ClaimPopupView : MonoBehaviour
    {
        public event Action OnClaimClicked;
        
        [SerializeField] private Transform contentParent;
        [SerializeField] private InventorySlotView slotPrefab;
        [SerializeField] private Button claimButton;
        [SerializeField] private Transform cardTransform;

        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _popupTween;
        
        private readonly List<InventorySlotView> _spawnedSlots = new();

        private void OnValidate()
        {
            if (claimButton == null) claimButton = GetComponentInChildren<Button>();
        }

        private void Awake()
        {
            claimButton.onClick.AddListener(HandleClaimButtonClicked);
        }
        
        private void OnDestroy()
        {
            _popupTween?.Kill();
            _popupTween = null;
            if (claimButton != null) claimButton.onClick.RemoveListener(HandleClaimButtonClicked);
        }

        public void Show(IReadOnlyDictionary<ItemData, int> rewards)
        {
            _popupTween?.Kill();
            ClearSlots();
            foreach (var (itemData, amount) in rewards)
            {
                InventorySlotView slot = Instantiate(slotPrefab, contentParent);
                slot.Setup(itemData, amount);
                _spawnedSlots.Add(slot);
            }
            gameObject.SetActive(true);
            _popupTween = PopupAnimation.Show(cardTransform, _animationConfig);
        }

        public void Hide()
        {
            _popupTween?.Kill();
            if (!gameObject.activeInHierarchy)
            {
                _popupTween = null;
                ClearSlots();
                return;
            }

            _popupTween = PopupAnimation
                .Hide(cardTransform, _animationConfig)
                .OnComplete(HandleHideComplete);
        }

        private void ClearSlots()
        {
            foreach (var slot in _spawnedSlots)
            {
                if (slot != null) Destroy(slot.gameObject);
            }
            _spawnedSlots.Clear();
        }

        private void HandleHideComplete()
        {
            _popupTween = null;
            ClearSlots();
            gameObject.SetActive(false);
        }
        
        private void HandleClaimButtonClicked()
        {
            OnClaimClicked?.Invoke();
        }
    }

}

