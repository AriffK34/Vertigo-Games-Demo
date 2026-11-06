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

        [SerializeField] private Image wheelBaseImage;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private Button spinButton;
        [SerializeField] private RectTransform raysTransform;
        [SerializeField] private WheelSliceView[] sliceViews;

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
                return sliceViews[_targetSliceIndex].IconPosition;
            }
        }

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!spinButton)
            {
                spinButton = GetComponentInChildren<Button>(true);
            }

            if (wheelBaseImage)
            {
                sliceViews =
                    wheelBaseImage.GetComponentsInChildren<WheelSliceView>(true);
            }

            _sliceRadius = Mathf.Max(0f, _sliceRadius);
        }

        private void Awake()
        {
            if (!wheelBaseImage ||
                !indicatorImage ||
                !spinButton ||
                !raysTransform ||
                !_animationConfig ||
                sliceViews == null ||
                sliceViews.Length == 0)
            {
                throw new InvalidOperationException(
                    $"{name}: Wheel references and slices must be assigned.");
            }

            foreach (WheelSliceView slice in sliceViews)
            {
                if (!slice)
                {
                    throw new InvalidOperationException(
                        $"{name}: Slice references cannot be empty.");
                }

                slice.Initialize(_animationConfig);
            }

            spinButton.onClick.AddListener(HandleSpinClicked);
            spinButton.interactable = false;

            indicatorImage.rectTransform.localEulerAngles = Vector3.zero;

            _indicatorTween = indicatorImage.rectTransform
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

            _raysTween = raysTransform
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
            if (spinButton)
            {
                spinButton.onClick.RemoveListener(HandleSpinClicked);
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

            if (slices == null || slices.Count != sliceViews.Length)
            {
                throw new ArgumentException(
                    "Wheel data and prefab slice counts must match.",
                    nameof(slices));
            }

            wheelBaseImage.sprite = theme.WheelSprite;
            indicatorImage.sprite = theme.IndicatorSprite;

            float sliceAngle = FULL_CIRCLE / sliceViews.Length;

            for (int i = 0; i < sliceViews.Length; i++)
            {
                float angle = i * sliceAngle;
                float radians = angle * Mathf.Deg2Rad;
                RectTransform rect = (RectTransform)sliceViews[i].transform;

                rect.anchoredPosition = new Vector2(
                    Mathf.Sin(radians),
                    Mathf.Cos(radians)) * _sliceRadius;

                rect.localRotation = Quaternion.Euler(0f, 0f, -angle);
                sliceViews[i].Setup(slices[i]);
            }
        }

        public void RotateWheel(int targetIndex, Action onComplete)
        {
            if (targetIndex < 0 || targetIndex >= sliceViews.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(targetIndex));
            }

            CancelRotation();

            foreach (WheelSliceView slice in sliceViews)
            {
                slice.CancelFeedback();
            }

            _targetSliceIndex = targetIndex;
            _lastTickSlice = -1;
            _onRotationComplete = onComplete;
            _indicatorTween.Rewind();

            float sliceAngle = FULL_CIRCLE / sliceViews.Length;
            float maximumOffset =
                sliceAngle * _animationConfig.LandingOffsetRatio;

            float currentAngle =
                wheelBaseImage.rectTransform.eulerAngles.z % FULL_CIRCLE;

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

            _rotationTween = wheelBaseImage.rectTransform
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

            _rotationTween = wheelBaseImage.rectTransform
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
            if (spinButton)
            {
                spinButton.interactable = isInteractable;
            }
        }

        #endregion

        #region Private Methods

        private void HandleSpinUpdate()
        {
            float sliceAngle = FULL_CIRCLE / sliceViews.Length;
            float angle = wheelBaseImage.rectTransform.eulerAngles.z;
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
            sliceViews[_targetSliceIndex].PlayPunchAnimation();
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