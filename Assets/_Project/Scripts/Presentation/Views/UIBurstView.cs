using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class UIBurstView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image _image;
        [SerializeField] private UIAnimationConfig _animationConfig;
        private Tween _burstTween;
        private Vector3 _initialScale;
        private Color _initialColor;

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
        }

        private void Awake()
        {
            _initialScale = _image.rectTransform.localScale;
            _initialColor = _image.color;
        }

        private void OnDestroy()
        {
            Stop();
        }

        #endregion

        #region Public Methods

        public void Play()
        {
            Stop();

            float halfDuration = _animationConfig.BurstDuration * 0.5f;
            Color peakColor = Color.Lerp(_initialColor, Color.white, _animationConfig.BurstBrightness);
            peakColor.a = _initialColor.a;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                _image.rectTransform
                    .DOScale(_initialScale * _animationConfig.BurstPeakScale, halfDuration)
                    .SetEase(_animationConfig.BurstEase));
            sequence.Join(
                _image
                    .DOColor(peakColor, halfDuration)
                    .SetEase(_animationConfig.BurstEase));
            sequence.Append(
                _image.rectTransform
                .DOScale(_initialScale, halfDuration)
                .SetEase(_animationConfig.BurstEase));
            sequence.Join(
                _image
                    .DOColor(_initialColor, halfDuration)
                    .SetEase(_animationConfig.BurstEase));
            _burstTween = sequence
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(HandleBurstComplete);
        }

        public void Stop()
        {
            _burstTween?.Kill();
            _burstTween = null;

            if (_image)
            {
                _image.rectTransform.localScale = _initialScale;
                _image.color = _initialColor;
            }
        }

        #endregion

        #region Private Methods

        private void HandleBurstComplete()
        {
            _burstTween = null;
        }

        #endregion
    }
}
