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

        [SerializeField] private RectTransform _slotContent;
        [SerializeField] private Image _itemIconImage;
        [SerializeField] private TextMeshProUGUI _itemAmountText;
        [SerializeField] private UIAnimationConfig _animationConfig;

        #endregion

        #region Properties

        public ItemData Item { get; private set; }

        public Vector3 IconPosition
        {
            get
            {
                return _itemIconImage.rectTransform.position;
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
            _itemIconImage.sprite = item.ItemIcon;
            _itemAmountText.text = $"x{NumberFormatter.Format(amount)}";
            PunchAnimation();
        }

        public void UpdateAmount(int amount)
        {
            _itemAmountText.text = $"x{NumberFormatter.Format(amount)}";
            PunchAnimation();
        }

        public void ResetForPool()
        {
            StopAnimation();

            Item = null;
            _itemIconImage.sprite = null;
            _itemAmountText.text = string.Empty;
        }

        #endregion

        #region Private Methods

        private void PunchAnimation()
        {
            UIPunchAnimation.Play(
                _slotContent,
                _animationConfig.InventoryPunch,
                gameObject);
        }

        private void StopAnimation()
        {
            if (_slotContent)
            {
                _slotContent.DOKill();
                _slotContent.localScale = Vector3.one;
            }
        }

        #endregion
    }
}