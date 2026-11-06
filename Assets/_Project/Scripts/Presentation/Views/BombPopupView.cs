using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Formatting;

namespace VertigoDemo.UI
{
    public class BombPopupView : MonoBehaviour
    {
        public event Action OnGiveUpClicked;
        public event Action OnReviveClicked;
        
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Transform _cardTransform;
        [SerializeField] private TMP_Text _reviveText;

        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _popupTween;

        [SerializeField] private UIBurstView _burstView;
        
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
        
        public void ValidateReferences()
        {
            if (!_giveUpButton ||
                !_reviveButton ||
                _giveUpButton == _reviveButton ||
                !_cardTransform ||
                !_reviveText ||
                !_animationConfig ||
                !_burstView)
            {
                throw new InvalidOperationException(
                    $"{name}: Bomb popup references are incomplete. Refresh button references.");
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
            if (_giveUpButton != null) _giveUpButton.onClick.RemoveListener(HandleGiveUpClicked);
            if (_reviveButton != null) _reviveButton.onClick.RemoveListener(HandleReviveClicked);
        }
        
        public void Show(bool canAffordRevive, int reviveCost)
        {
            _reviveText.text = $"REVIVE ({NumberFormatter.Format(reviveCost)})";
            _popupTween?.Kill();
            _reviveButton.interactable = canAffordRevive;
            gameObject.SetActive(true);
            _popupTween = PopupAnimation.Show(_cardTransform, _animationConfig);
            _burstView.Play();
        }
        
        public void Hide()
        {
            _burstView.Stop();
            _popupTween?.Kill();

            if (!gameObject.activeInHierarchy)
            {
                _popupTween = null;
                return;
            }

            _popupTween = PopupAnimation
                .Hide(_cardTransform, _animationConfig)
                .OnComplete(HandleHideComplete);
        }

        private void HandleHideComplete()
        {
            _popupTween = null;
            gameObject.SetActive(false);
        }
        
        private void HandleGiveUpClicked()
        {
            OnGiveUpClicked?.Invoke();
        }
        
        private void HandleReviveClicked()
        {
            OnReviveClicked?.Invoke();
        }
    }

}
