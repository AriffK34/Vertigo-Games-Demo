using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class BombPopupView : MonoBehaviour
    {
        public event Action OnGiveUpClicked;
        public event Action OnReviveClicked;
        
        [SerializeField] private Button giveUpButton;
        [SerializeField] private Button reviveButton;
        [SerializeField] private Transform cardTransform;
        [SerializeField] private TMP_Text _reviveText;

        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _popupTween;

        [SerializeField] private UIBurstView _burstView;
        
        private void OnValidate()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                string btnName = btn.gameObject.name.ToLower();
        
                if (btnName.Contains("give_up")) giveUpButton = btn;
                else if (btnName.Contains("revive")) reviveButton = btn;
            }
        }
        
        private void Awake()
        {
            giveUpButton.onClick.AddListener(HandleGiveUpClicked);
            reviveButton.onClick.AddListener(HandleReviveClicked);
        }
        
        private void OnDestroy()
        {
            _popupTween?.Kill();
            _popupTween = null;
            if (giveUpButton != null) giveUpButton.onClick.RemoveListener(HandleGiveUpClicked);
            if (reviveButton != null) reviveButton.onClick.RemoveListener(HandleReviveClicked);
        }
        
        public void Show(bool canAffordRevive, int reviveCost)
        {
            _reviveText.text = $"REVIVE ({reviveCost})";
            _popupTween?.Kill();
            reviveButton.interactable = canAffordRevive;
            gameObject.SetActive(true);
            _popupTween = PopupAnimation.Show(cardTransform, _animationConfig);
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
                .Hide(cardTransform, _animationConfig)
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
