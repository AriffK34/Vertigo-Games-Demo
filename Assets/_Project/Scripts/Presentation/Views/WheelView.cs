using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;
using Random = UnityEngine.Random;

namespace VertigoDemo.UI
{
    public class WheelView : MonoBehaviour
    {
        #region Events

        public event Action OnSpinClicked;

        #endregion

        #region Fields

        private const float FULL_CIRCLE = 360f;

        [SerializeField] private Image _wheelBaseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private Button _spinButton;
        [SerializeField] private RectTransform _raysTransform;
        [SerializeField] private WheelSliceView[] _sliceViews;

        [SerializeField] private WheelAnimationConfig _animationConfig;
        [SerializeField, Min(0f)] private float _sliceRadius = 235f;

        private Tween _rotationTween;
        private Tween _raysTween;
        private Tween _indicatorTween;

        private Action _onRotationComplete;
        private int _targetSliceIndex;
        private int _lastTickSlice;

        #endregion

        #region Properties

        public Vector3 WinningIconPosition
        {
            get
            {
                return _sliceViews[_targetSliceIndex].IconPosition;
            }
        }

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!_spinButton)
            {
                _spinButton = GetComponentInChildren<Button>(true);
            }

            if (_wheelBaseImage)
            {
                _sliceViews =
                    _wheelBaseImage.GetComponentsInChildren<WheelSliceView>(true);
            }

            _sliceRadius = Mathf.Max(0f, _sliceRadius);
        }

        private void Awake()
        {
            if (!_wheelBaseImage ||
                !_indicatorImage ||
                !_spinButton ||
                !_raysTransform ||
                !_animationConfig ||
                _sliceViews == null ||
                _sliceViews.Length == 0)
            {
                throw new InvalidOperationException(
                    $"{name}: Wheel references and slices must be assigned.");
            }

            foreach (WheelSliceView slice in _sliceViews)
            {
                if (!slice)
                {
                    throw new InvalidOperationException(
                        $"{name}: Slice references cannot be empty.");
                }

                slice.Initialize(_animationConfig);
            }

            _spinButton.onClick.AddListener(HandleSpinClicked);
            _spinButton.interactable = false;

            _indicatorImage.rectTransform.localEulerAngles = Vector3.zero;

            _indicatorTween = _indicatorImage.rectTransform
                .DOPunchRotation(
                    new Vector3(0f, 0f, -_animationConfig.IndicatorAngle),
                    _animationConfig.IndicatorDuration,
                    _animationConfig.IndicatorVibrato,
                    _animationConfig.IndicatorElasticity)
                .SetEase(_animationConfig.IndicatorEase)
                .SetAutoKill(false)
                .SetRecyclable(false)
                .SetLink(gameObject)
                .Pause();

            _raysTween = _raysTransform
                .DORotate(
                    new Vector3(0f, 0f, -FULL_CIRCLE),
                    _animationConfig.RaysDuration,
                    RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(_animationConfig.RaysEase)
                .SetRecyclable(false)
                .SetLink(gameObject);
        }

        private void OnDestroy()
        {
            if (_spinButton)
            {
                _spinButton.onClick.RemoveListener(HandleSpinClicked);
            }

            CancelRotation();

            _indicatorTween?.Kill();
            _indicatorTween = null;

            _raysTween?.Kill();
            _raysTween = null;
        }

        #endregion

        #region Public Methods

        public void UpdateWheel(
            WheelTheme theme,
            IReadOnlyList<SliceDisplayData> slices)
        {
            if (!theme)
            {
                throw new ArgumentNullException(nameof(theme));
            }

            if (slices == null || slices.Count != _sliceViews.Length)
            {
                throw new ArgumentException(
                    "Wheel data and prefab slice counts must match.",
                    nameof(slices));
            }

            _wheelBaseImage.sprite = theme.WheelSprite;
            _indicatorImage.sprite = theme.IndicatorSprite;

            float sliceAngle = FULL_CIRCLE / _sliceViews.Length;

            for (int i = 0; i < _sliceViews.Length; i++)
            {
                float angle = i * sliceAngle;
                float radians = angle * Mathf.Deg2Rad;
                RectTransform rect = (RectTransform)_sliceViews[i].transform;

                rect.anchoredPosition = new Vector2(
                    Mathf.Sin(radians),
                    Mathf.Cos(radians)) * _sliceRadius;

                rect.localRotation = Quaternion.Euler(0f, 0f, -angle);
                _sliceViews[i].Setup(slices[i]);
            }
        }

        public void RotateWheel(int targetIndex, Action onComplete)
        {
            if (targetIndex < 0 || targetIndex >= _sliceViews.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(targetIndex));
            }

            CancelRotation();

            foreach (WheelSliceView slice in _sliceViews)
            {
                slice.CancelFeedback();
            }

            _targetSliceIndex = targetIndex;
            _lastTickSlice = -1;
            _onRotationComplete = onComplete;
            _indicatorTween.Rewind();

            float sliceAngle = FULL_CIRCLE / _sliceViews.Length;
            float maximumOffset =
                sliceAngle * _animationConfig.LandingOffsetRatio;

            float currentAngle =
                _wheelBaseImage.rectTransform.eulerAngles.z % FULL_CIRCLE;

            float targetAngle = targetIndex * sliceAngle +
                Random.Range(-maximumOffset, maximumOffset);

            float delta = targetAngle - currentAngle;

            if (delta <= 0f)
            {
                delta += FULL_CIRCLE;
            }

            float finalAngle = currentAngle +
                _animationConfig.FullTurns * FULL_CIRCLE +
                delta;

            _rotationTween = _wheelBaseImage.rectTransform
                .DORotate(
                    new Vector3(0f, 0f, finalAngle),
                    _animationConfig.SpinDuration,
                    RotateMode.FastBeyond360)
                .SetEase(_animationConfig.SpinEase)
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnUpdate(HandleSpinUpdate)
                .OnComplete(HandleSpinComplete);
        }

        public Tween ResetRotation(float duration, Action onComplete)
        {
            CancelRotation();
            _onRotationComplete = onComplete;
            _indicatorTween.Rewind();

            _rotationTween = _wheelBaseImage.rectTransform
                .DORotate(Vector3.zero, duration)
                .SetEase(_animationConfig.ResetEase)
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(CompleteRotation);

            return _rotationTween;
        }

        public void CancelRotation()
        {
            Tween previousTween = _rotationTween;
            _rotationTween = null;
            _onRotationComplete = null;
            previousTween?.Kill();
        }

        public void SetSpinInteractable(bool isInteractable)
        {
            if (_spinButton)
            {
                _spinButton.interactable = isInteractable;
            }
        }

        #endregion

        #region Private Methods

        private void HandleSpinUpdate()
        {
            float sliceAngle = FULL_CIRCLE / _sliceViews.Length;
            float angle = _wheelBaseImage.rectTransform.eulerAngles.z;
            int currentSlice = Mathf.FloorToInt(angle / sliceAngle);

            if (currentSlice == _lastTickSlice)
            {
                return;
            }

            _lastTickSlice = currentSlice;
            _indicatorTween.Restart();
        }

        private void HandleSpinComplete()
        {
            _indicatorTween.Rewind();
            _sliceViews[_targetSliceIndex].PlayPunchAnimation();
            CompleteRotation();
        }

        private void CompleteRotation()
        {
            _rotationTween = null;

            Action callback = _onRotationComplete;
            _onRotationComplete = null;
            callback?.Invoke();
        }

        private void HandleSpinClicked()
        {
            OnSpinClicked?.Invoke();
        }

        #endregion
    }
}