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

        #region Propertie

        public Vector3 IconPosition
        {
            get
            {
                return _itemIconImage.rectTransform.position;
            }
        }

        #endregion

        #region Unity Callbacks

        private void OnDisable()
        {
            StopPunchAnimation();
        }

        private void OnDestroy()
        {
            StopPunchAnimation();
        }

        #endregion

        #region Public Methods

        public void Setup(ItemData item, int amount)
        {
            _itemIconImage.sprite = item.ItemIcon;
            _itemAmountText.text = $"x{NumberFormatter.Format(amount)}";
            PlayPunchAnimation();
        }

        public void UpdateAmount(int amount)
        {
            _itemAmountText.text = $"x{NumberFormatter.Format(amount)}";
            PlayPunchAnimation();
        }

        public void ResetForPool()
        {
            StopPunchAnimation();

            _itemIconImage.sprite = null;
            _itemAmountText.text = string.Empty;
        }

        #endregion

        #region Private Methods

        private void PlayPunchAnimation()
        {
            UIPunchAnimation.Play(
                _slotContent,
                _animationConfig.InventoryPunch,
                gameObject);
        }

        private void StopPunchAnimation()
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
