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
        private Vector2 _defaultIconPos;
        private Tween _punchTween;
        private Tween _glowTween;

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

        #region Lifecycle

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
            _defaultIconPos = _iconImage.rectTransform.anchoredPosition;
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
                    : _defaultIconPos;
        }

        public void PlayPunchAnimation()
        {
            CancelFeedback();

            _punchTween = _animationRoot
                .DOPunchScale(
                    Vector3.one * _animationConfig.SlicePunchStrength,
                    _animationConfig.SlicePunchDuration,
                    _animationConfig.SlicePunchVibrato,
                    _animationConfig.SlicePunchElasticity)
                .SetEase(_animationConfig.SlicePunchEase)
                .SetRecyclable(false)
                .SetLink(gameObject);

            _glowImage.color = _animationConfig.GlowColor;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(
                _glowImage.rectTransform
                    .DOScale(
                        Vector3.one * _animationConfig.GlowEndScale,
                        _animationConfig.GlowDuration)
                    .SetEase(_animationConfig.GlowScaleEase));

            sequence.Join(
                _glowImage
                    .DOFade(0f, _animationConfig.GlowDuration)
                    .SetEase(_animationConfig.GlowFadeEase));

            _glowTween = sequence
                .SetRecyclable(false)
                .SetLink(gameObject);
        }

        public void CancelFeedback()
        {
            _punchTween?.Kill();
            _punchTween = null;

            _glowTween?.Kill();
            _glowTween = null;

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
    }
}