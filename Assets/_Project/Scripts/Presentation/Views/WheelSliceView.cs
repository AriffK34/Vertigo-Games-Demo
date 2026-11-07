using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class WheelSliceView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private RectTransform _animationRoot;
        [SerializeField] private Image _glowImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;
        private WheelAnimationConfig _animationConfig;
        private Vector2 _defaultIconPosition;
        private Tween _feedbackTween;
        private Action _onFeedbackComplete;

        #endregion

        #region Properties

        public Vector3 IconPosition
        {
            get
            {
                return _iconImage.rectTransform.position;
            }
        }

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            if (!_amountText)
            {
                _amountText = GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }

        private void OnDisable()
        {
            CancelFeedback();
        }

        private void OnDestroy()
        {
            CancelFeedback();
        }

        #endregion

        #region Public Methods

        public void Initialize(WheelAnimationConfig animationConfig)
        {
            if (!animationConfig)
            {
                throw new ArgumentNullException(nameof(animationConfig));
            }

            if (!_animationRoot || !_glowImage || !_iconImage || !_amountText)
            {
                throw new InvalidOperationException($"{name}: Slice view references must be assigned.");
            }

            _animationConfig = animationConfig;
            _defaultIconPosition = _iconImage.rectTransform.anchoredPosition;
            CancelFeedback();
        }

        public void Setup(SliceDisplayData data)
        {
            CancelFeedback();

            _iconImage.sprite = data.Icon;
            _amountText.text = data.Text;

            _iconImage.rectTransform.anchoredPosition =
                string.IsNullOrEmpty(data.Text)
                    ? Vector2.zero
                    : _defaultIconPosition;
        }

        public void PlayPunchAnimation(Action onComplete)
        {
            CancelFeedback();
            _onFeedbackComplete = onComplete;
            _glowImage.color = _animationConfig.GlowColor;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(
                _animationRoot
                    .DOPunchScale(
                        Vector3.one * _animationConfig.SlicePunchStrength,
                        _animationConfig.SlicePunchDuration,
                        _animationConfig.SlicePunchVibrato,
                        _animationConfig.SlicePunchElasticity)
                    .SetEase(_animationConfig.SlicePunchEase));

            sequence.Join(
                _glowImage.rectTransform
                    .DOScale(
                        Vector3.one * _animationConfig.GlowEndScale,
                        _animationConfig.GlowDuration)
                    .SetEase(_animationConfig.GlowScaleEase));

            sequence.Join(
                _glowImage
                    .DOFade(0f, _animationConfig.GlowDuration)
                    .SetEase(_animationConfig.GlowFadeEase));

            _feedbackTween = sequence
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(HandleFeedbackComplete);
        }

        public void CancelFeedback()
        {
            _feedbackTween?.Kill();
            _feedbackTween = null;
            _onFeedbackComplete = null;

            if (_animationRoot)
            {
                _animationRoot.localScale = Vector3.one;
            }

            if (_glowImage)
            {
                Color color = _glowImage.color;
                color.a = 0f;
                _glowImage.color = color;
                _glowImage.rectTransform.localScale = Vector3.zero;
            }
        }

        #endregion

        #region Private Methods

        private void HandleFeedbackComplete()
        {
            _feedbackTween = null;

            Action onComplete = _onFeedbackComplete;
            _onFeedbackComplete = null;
            onComplete?.Invoke();
        }

        #endregion
    }
}
