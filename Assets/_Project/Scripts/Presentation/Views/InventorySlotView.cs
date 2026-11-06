using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Formatting;

namespace VertigoDemo.UI
{
    public class InventorySlotView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private RectTransform slotContent;
        [SerializeField] private Image itemIconImage;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        #endregion

        #region Properties

        public ItemData Item { get; private set; }

        public Vector3 IconPosition
        {
            get
            {
                return itemIconImage.rectTransform.position;
            }
        }

        #endregion

        #region Lifecycle

        private void OnDisable()
        {
            StopAnimation();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        #endregion

        #region Public Methods

        public void Setup(ItemData item, int amount)
        {
            Item = item;
            itemIconImage.sprite = item.ItemIcon;
            itemAmountText.text = $"x{NumberFormatter.Format(amount)}";
            PunchAnimation();
        }

        public void UpdateAmount(int amount)
        {
            itemAmountText.text = $"x{NumberFormatter.Format(amount)}";
            PunchAnimation();
        }

        public void ResetForPool()
        {
            StopAnimation();

            Item = null;
            itemIconImage.sprite = null;
            itemAmountText.text = string.Empty;
        }

        #endregion

        #region Private Methods

        private void PunchAnimation()
        {
            StopAnimation();

            slotContent
                .DOPunchScale(Vector3.one * 0.2f, 0.25f, 1, 0.5f)
                .SetRecyclable(false)
                .SetLink(gameObject);
        }

        private void StopAnimation()
        {
            if (slotContent)
            {
                slotContent.DOKill();
                slotContent.localScale = Vector3.one;
            }
        }

        #endregion
    }
}