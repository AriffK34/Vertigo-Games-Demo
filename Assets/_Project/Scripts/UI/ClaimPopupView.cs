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
            if (claimButton != null) claimButton.onClick.RemoveListener(HandleClaimButtonClicked);
        }

        public void Show(IReadOnlyDictionary<ItemData, int> rewards)
        {
            ClearSlots();
            foreach (var (itemData, amount) in rewards)
            {
                InventorySlotView slot = Instantiate(slotPrefab, contentParent);
                slot.Setup(itemData, amount);
                _spawnedSlots.Add(slot);
            }
            gameObject.SetActive(true);
            
            cardTransform.DOKill();
            cardTransform.localScale = Vector3.zero;
            cardTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            ClearSlots();
            cardTransform.DOKill();
            cardTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ClearSlots();
                gameObject.SetActive(false);
            });
        }

        private void ClearSlots()
        {
            foreach (var slot in _spawnedSlots)
            {
                if (slot != null) Destroy(slot.gameObject);
            }
            _spawnedSlots.Clear();
        }
        
        private void HandleClaimButtonClicked()
        {
            OnClaimClicked?.Invoke();
        }
    }

}

