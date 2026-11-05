using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class RewardFlightView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image _iconImage;
        [SerializeField] private UIAnimationConfig _animationConfig;

        private Tween _flighTween;
        private Action _onFlightComplete;

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!_iconImage)
            {
                _iconImage = GetComponent<Image>();
            }
        }

        private void Awake()
        {
            _iconImage.enabled = false;
        }

        private void OnDestroy()
        {
            Cancel();
        }

        #endregion

        #region Public Methods

        public void Play(Sprite icon, Vector3 startPosition, Vector3 endPosition, Action onComplete)
        {
            Cancel();
            _iconImage.sprite = icon;
            _iconImage.rectTransform.position = startPosition;
            _iconImage.enabled = true;
            _onFlightComplete = onComplete;

            RectTransform iconTransform = _iconImage.rectTransform;
            iconTransform.localScale = Vector3.one;

            float duration = _animationConfig.RewardFlightDuration;
            float halfDuration = duration * 0.5f;

            Sequence flightSequence = DOTween.Sequence();

            flightSequence.Append(
                iconTransform
                    .DOMove(endPosition, duration)
                    .SetEase(_animationConfig.RewardFlightEase));

            flightSequence.Insert(
                0f,
                iconTransform
                    .DOScale(Vector3.one * _animationConfig.RewardFlightPeakScale, halfDuration)
                    .SetEase(_animationConfig.RewardFlightEase));

            flightSequence.Insert(
                halfDuration,
                iconTransform
                    .DOScale(Vector3.one, halfDuration)
                    .SetEase(_animationConfig.RewardFlightEase));

            _flighTween = flightSequence
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(HandleFlightComplete);
        }

        public void Cancel()
        {
            _flighTween?.Kill();
            _flighTween = null;
            _onFlightComplete = null;
            if (_iconImage)
            {
                _iconImage.enabled = false;
                _iconImage.rectTransform.localScale = Vector3.one;

            }
        }

        #endregion

        #region Private Methods

        private void HandleFlightComplete()
        {
            _flighTween = null;
            _iconImage.enabled = false;

            Action onComplete = _onFlightComplete;
            _onFlightComplete = null;
            onComplete?.Invoke();
        }

        #endregion
    }
}
