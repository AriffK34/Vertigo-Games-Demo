using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class BombPopupView : MonoBehaviour
    {
        public event Action OnGiveUpClicked;
        public event Action OnReviveClicked;
        
        [SerializeField] private Button giveUpButton;
        [SerializeField] private Button reviveButton;
        [SerializeField] private Transform cardTransform;

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
            if (giveUpButton != null) giveUpButton.onClick.RemoveListener(HandleGiveUpClicked);
            if (reviveButton != null) reviveButton.onClick.RemoveListener(HandleReviveClicked);
        }
        
        public void Show(bool canAffordRevive)
        {
            reviveButton.interactable = canAffordRevive;
            gameObject.SetActive(true);
            cardTransform.DOKill();
            cardTransform.localScale = Vector3.zero;
            cardTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }
        
        public void Hide()
        {
            cardTransform.DOKill();
            cardTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
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
