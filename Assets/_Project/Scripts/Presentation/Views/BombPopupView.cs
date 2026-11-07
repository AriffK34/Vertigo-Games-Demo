using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Animations;
using VertigoDemo.Presentation.Formatting;

namespace VertigoDemo.UI
{
    public class BombPopupView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Transform _cardTransform;
        [SerializeField] private TMP_Text _reviveText;
        [SerializeField] private Image _reviveCurrencyIcon;
        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _popupTween;
        private Action _onHideComplete;
        [SerializeField] private UIBurstView _burstView;

        #endregion

        #region Events

        public event Action OnGiveUpClicked;
        public event Action OnReviveClicked;

        #endregion

        #region Unity Callbacks

        [ContextMenu("Refresh Button References")]
        private void OnValidate()
        {
            _giveUpButton = null;
            _reviveButton = null;

            PopupActionButton[] bindings =
                GetComponentsInChildren<PopupActionButton>(true);

            foreach (PopupActionButton binding in bindings)
            {
                switch (binding.Action)
                {
                    case PopupAction.GiveUp:
                        _giveUpButton = binding.Button;
                        break;

                    case PopupAction.Revive:
                        _reviveButton = binding.Button;
                        break;
                }
            }

            if (_reviveButton)
            {
                _reviveText = _reviveButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        private void Awake()
        {
            _giveUpButton.onClick.AddListener(HandleGiveUpClicked);
            _reviveButton.onClick.AddListener(HandleReviveClicked);
        }

        private void OnDestroy()
        {
            _popupTween?.Kill();
            _popupTween = null;
            _onHideComplete = null;
            if (_giveUpButton)
            {
                _giveUpButton.onClick.RemoveListener(HandleGiveUpClicked);
            }
            if (_reviveButton)
            {
                _reviveButton.onClick.RemoveListener(HandleReviveClicked);
            }
        }

        #endregion

        #region Public Methods

        public void ValidateReferences()
        {
            if (!_giveUpButton ||
                !_reviveButton ||
                _giveUpButton == _reviveButton ||
                !_cardTransform ||
                !_reviveText ||
                !_reviveCurrencyIcon ||
                !_animationConfig ||
                !_burstView)
            {
                throw new InvalidOperationException(
                    $"{name}: Bomb popup references are incomplete. Refresh button references.");
            }
        }

        public void Show(bool canAffordRevive, int reviveCost, Sprite reviveCurrencyIcon)
        {
            _reviveText.text = $"REVIVE ({NumberFormatter.Format(reviveCost)})";
            _reviveCurrencyIcon.sprite = reviveCurrencyIcon;
            _reviveCurrencyIcon.enabled = reviveCurrencyIcon != null;
            _popupTween?.Kill();
            _onHideComplete = null;
            _giveUpButton.interactable = true;
            _reviveButton.interactable = canAffordRevive;
            gameObject.SetActive(true);
            _popupTween = PopupAnimation.Show(_cardTransform, _animationConfig);
            _burstView.Play();
        }

        public void Hide(Action onComplete = null)
        {
            _burstView.Stop();
            _popupTween?.Kill();
            _onHideComplete = onComplete;
            _giveUpButton.interactable = false;
            _reviveButton.interactable = false;

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
            gameObject.SetActive(false);

            Action onComplete = _onHideComplete;
            _onHideComplete = null;
            onComplete?.Invoke();
        }

        private void HandleGiveUpClicked()
        {
            OnGiveUpClicked?.Invoke();
        }

        private void HandleReviveClicked()
        {
            OnReviveClicked?.Invoke();
        }

        #endregion
    }
}
