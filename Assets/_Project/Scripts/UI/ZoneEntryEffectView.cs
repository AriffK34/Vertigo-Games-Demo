using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class ZoneEntryEffectView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image _image;
        [SerializeField] private UIAnimationConfig _animationConfig;

        private Tween _effectTween;
        private Action _onComplete;

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
        }

        private void Awake()
        {
            _image.enabled = false;
        }

        private void OnDestroy()
        {
            Cancel();
        }

        #endregion

        #region Public Methods

        public void Play(Color color, Action onComplete)
        {
            Cancel();

            _image.color = color;
            _image.rectTransform.localScale = Vector3.one;
            _image.enabled = true;
            _onComplete = onComplete;
            
            Sequence sequence = DOTween.Sequence();

            sequence.Append(
                _image.rectTransform
                    .DOScale(
                        Vector3.one * _animationConfig.BurstPeakScale,
                        _animationConfig.BurstDuration)
                    .SetEase(_animationConfig.BurstEase));

            sequence.Join(
                _image
                    .DOFade(0f, _animationConfig.BurstDuration)
                    .SetEase(_animationConfig.BurstEase));

            _effectTween = sequence
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(HandleEffectComplete);
        }

        public void Cancel()
        {
            _effectTween?.Kill();
            _effectTween = null;
            _onComplete = null;

            if (_image)
            {
                _image.enabled = false;
                _image.rectTransform.localScale = Vector3.one;
            }
        }

        #endregion

        #region Private Methods

        private void HandleEffectComplete()
        {
            _effectTween = null;
            _image.enabled = false;

            Action onComplete = _onComplete;
            _onComplete = null;
            onComplete?.Invoke();
        }

        #endregion
    }
}

