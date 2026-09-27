using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private RectTransform slotContent;
        [SerializeField] private Image itemIconImage;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        public ItemData Item { get; private set; }
        
        private void OnDestroy()
        {
            slotContent?.DOKill();
        }
        
        public void Setup(ItemData item, int amount)
        {
            Item = item;
            itemIconImage.sprite = item.ItemIcon;
            itemAmountText.text = $"x{amount}";
            PunchAnimation();
        }
        
        public void UpdateAmount(int amount)
        {
            itemAmountText.text = $"x{amount}";
            PunchAnimation();
        }

        private void PunchAnimation()
        {
            slotContent.DOKill();
            slotContent.localScale = Vector3.one;
            slotContent.DOPunchScale(Vector3.one * 0.2f, 0.25f, 1, 0.5f);
        }
    }
}