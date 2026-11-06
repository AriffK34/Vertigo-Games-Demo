using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Animations;
using VertigoDemo.Presentation.Pooling;

namespace VertigoDemo.UI
{
    public class ClaimPopupView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Transform _contentParent;
        [SerializeField] private InventorySlotView _slotPrefab;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Transform _cardTransform;
        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _popupTween;
        private Action _onHideComplete;
        private readonly List<InventorySlotView> _activeSlots = new();
        private InventorySlotPool _slotPool;

        #endregion

        #region Events

        public event Action OnClaimClicked;

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            if (!_claimButton)
            {
                _claimButton = GetComponentInChildren<Button>(true);
            }
        }

        private void Awake()
        {
            _claimButton.onClick.AddListener(HandleClaimButtonClicked);
        }

        private void OnDestroy()
        {
            _popupTween?.Kill();
            _popupTween = null;
            _onHideComplete = null;
            if (_claimButton)
            {
                _claimButton.onClick.RemoveListener(HandleClaimButtonClicked);
            }
        }

        #endregion

        #region Public Methods

        public void Show(IReadOnlyDictionary<ItemData, int> rewards)
        {
            _popupTween?.Kill();
            _onHideComplete = null;
            _claimButton.interactable = true;

            if (_slotPool == null)
            {
                _slotPool = new InventorySlotPool(_slotPrefab, _contentParent);
            }

            ClearSlots();

            foreach (var reward in rewards)
            {
                InventorySlotView slot = _slotPool.Rent(reward.Key, reward.Value);
                _activeSlots.Add(slot);
            }

            gameObject.SetActive(true);
            _popupTween = PopupAnimation.Show(_cardTransform, _animationConfig);
        }

        public void Hide(Action onComplete)
        {
            _popupTween?.Kill();
            _onHideComplete = onComplete;
            _claimButton.interactable = false;
            if (!gameObject.activeInHierarchy)
            {
                HandleHideComplete();
                return;
            }

            _popupTween = PopupAnimation
                .Hide(_cardTransform, _animationConfig)
                .OnComplete(HandleHideComplete);
        }

        #endregion

        #region Private Methods

        private void ClearSlots()
        {
            foreach (InventorySlotView slot in _activeSlots)
            {
                _slotPool.Return(slot);
            }

            _activeSlots.Clear();
        }

        private void HandleHideComplete()
        {
            _popupTween = null;
            ClearSlots();
            gameObject.SetActive(false);
            Action onComplete = _onHideComplete;
            _onHideComplete = null;
            onComplete?.Invoke();
        }

        private void HandleClaimButtonClicked()
        {
            OnClaimClicked?.Invoke();
        }

        #endregion
    }
}
