using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Pooling;

namespace VertigoDemo.UI
{
    public class ClaimPopupView : MonoBehaviour
    {
        public event Action OnClaimClicked;
        
        [SerializeField] private Transform _contentParent;
        [SerializeField] private InventorySlotView _slotPrefab;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Transform _cardTransform;

        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _popupTween;
        
        private readonly List<InventorySlotView> _spawnedSlots = new();
        private InventorySlotPool _slotPool;
        
        private void OnValidate()
        {
            if (!_claimButton) _claimButton = GetComponentInChildren<Button>(true);
        }

        private void Awake()
        {
            _claimButton.onClick.AddListener(HandleClaimButtonClicked);
        }
        
        private void OnDestroy()
        {
            _popupTween?.Kill();
            _popupTween = null;
            if (_claimButton != null) _claimButton.onClick.RemoveListener(HandleClaimButtonClicked);
        }

        public void Show(IReadOnlyDictionary<ItemData, int> rewards)
        {
            _popupTween?.Kill();

            if (_slotPool == null)
            {
                _slotPool = new InventorySlotPool(_slotPrefab, _contentParent);
            }

            ClearSlots();

            foreach (var reward in rewards)
            {
                InventorySlotView slot = _slotPool.Rent(reward.Key, reward.Value);
                _spawnedSlots.Add(slot);
            }

            gameObject.SetActive(true);
            _popupTween = PopupAnimation.Show(_cardTransform, _animationConfig);
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
                .Hide(_cardTransform, _animationConfig)
                .OnComplete(HandleHideComplete);
        }

        private void ClearSlots()
        {
            foreach (InventorySlotView slot in _spawnedSlots)
            {
                _slotPool.Return(slot);
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

