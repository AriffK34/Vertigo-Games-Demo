using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Animations;
using VertigoDemo.Presentation.Formatting;

namespace VertigoDemo.UI
{
    public class InventorySlotView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private RectTransform slotContent;
        [SerializeField] private Image itemIconImage;
        [SerializeField] private TextMeshProUGUI itemAmountText;
        [SerializeField] private UIAnimationConfig _animationConfig;

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
            UIPunchAnimation.Play(
                slotContent,
                _animationConfig.InventoryPunch,
                gameObject);
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