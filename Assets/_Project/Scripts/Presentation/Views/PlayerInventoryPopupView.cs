using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Animations;

namespace VertigoDemo.UI
{
    public class PlayerInventoryPopupView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _cardTransform;
        [SerializeField] private TMP_Text _emptyMessage;
        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _popupTween;
        private Action _onClosed;

        #endregion

        #region Events

        public event Action OnCloseClicked;

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            if (!_closeButton)
            {
                _closeButton = GetComponentInChildren<Button>(true);
            }
        }

        private void Awake()
        {
            _closeButton.onClick.AddListener(HandleCloseClicked);
        }

        private void OnDestroy()
        {
            _popupTween?.Kill();
            _popupTween = null;
            _onClosed = null;

            if (_closeButton)
            {
                _closeButton.onClick.RemoveListener(HandleCloseClicked);
            }
        }

        #endregion

        #region Public Methods

        public void ValidateReferences()
        {
            if (!_inventoryView || !_closeButton || !_cardTransform ||
                !_emptyMessage || !_animationConfig)
            {
                throw new InvalidOperationException(
                    $"{name}: Player inventory popup references must be assigned.");
            }
        }

        public void Show(IReadOnlyDictionary<ItemData, int> items)
        {
            _popupTween?.Kill();
            _onClosed = null;
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _inventoryView.Refresh(items);
            _emptyMessage.gameObject.SetActive(items.Count == 0);
            _closeButton.interactable = true;
            _popupTween = PopupAnimation.Show(_cardTransform, _animationConfig);
        }

        public void Hide(Action onClosed)
        {
            _popupTween?.Kill();
            _onClosed = onClosed;
            _closeButton.interactable = false;

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

        private void HandleHideComplete()
        {
            _popupTween = null;
            _inventoryView.Clear();
            gameObject.SetActive(false);

            Action onClosed = _onClosed;
            _onClosed = null;
            onClosed?.Invoke();
        }

        private void HandleCloseClicked()
        {
            OnCloseClicked?.Invoke();
        }

        #endregion
    }
}
