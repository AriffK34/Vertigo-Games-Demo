using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image glowImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI amountText;
        
        private Vector2 _defaultIconPos;

        public Vector3 IconPosition
        {
            get
            {
                return iconImage.rectTransform.position;
            }
        }
        
        protected void OnValidate()
        {
            if (amountText == null) amountText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        private void Awake()
        {
            if (iconImage != null) _defaultIconPos = iconImage.rectTransform.anchoredPosition;
        }

        public void Setup(SliceDisplayData data)
        {
            iconImage.sprite = data.Icon;
            amountText.text = data.Text;
            
            iconImage.rectTransform.anchoredPosition = string.IsNullOrEmpty(data.Text) ? Vector2.zero : _defaultIconPos;
        }
        
        public void PlayPunchAnimation()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.25f, 0.4f, 2, 0.5f);
            
            glowImage.DOKill();
            glowImage.rectTransform.localScale = Vector3.zero;
            glowImage.color = new Color(1f, 0.95f, 0.6f, 1f);
        
            glowImage.rectTransform.DOScale(Vector3.one * 1.5f, 0.6f).SetEase(Ease.OutCubic);
            glowImage.DOFade(0f, 0.6f);
        }

    }
}