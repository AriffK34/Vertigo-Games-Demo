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
        public event Action OnSpinClicked;
        
        [SerializeField] private Image wheelBaseImage;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private Button spinButton;
        [SerializeField] private RectTransform raysTransform;
        [SerializeField] private WheelSliceView[] sliceViews;
        
        [SerializeField] private float spinDuration = 3.5f;
        [SerializeField] private int fullTurns = 5;

        private Tween _rotationTween;
        private Tween _raysTween;
        private Tween _indicatorTween;

        private Action _onRotationComplete;
        private int _targetSliceIndex;
        private int _lastTickSlice;
        
        protected void OnValidate()
        {
            if (spinButton == null) spinButton = GetComponentInChildren<Button>(true);
            if (sliceViews is not { Length: 8 }) sliceViews = GetComponentsInChildren<WheelSliceView>(true);
        }

        private void Awake()
        {
            spinButton.onClick.AddListener(HandleSpinClicked);
            spinButton.interactable = false;
            indicatorImage.rectTransform.localEulerAngles = Vector3.zero;
            _indicatorTween = indicatorImage.rectTransform
                .DOPunchRotation(new Vector3(0f, 0f, -20f), 0.15f, 1, 0.5f)
                .SetAutoKill(false)
                .SetRecyclable(false)
                .SetLink(gameObject)
                .Pause();

            _raysTween = raysTransform
                .DORotate(new Vector3(0, 0, -360), 30f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear)
                .SetRecyclable(false)
                .SetLink(gameObject);
        }
        
        protected void OnDestroy()
        {
            if (spinButton) spinButton.onClick.RemoveListener(HandleSpinClicked);
            StopRotation();
            
            _indicatorTween?.Kill();
            _indicatorTween = null;
            _raysTween?.Kill();
            _raysTween = null;
        }
        
        public void UpdateWheel(WheelTheme theme, IReadOnlyList<SliceDisplayData> slices)
        {
            wheelBaseImage.sprite = theme.WheelSprite;
            indicatorImage.sprite = theme.IndicatorSprite;

            for (int i = 0; i < 8; i++)
            {
                SliceDisplayData data = slices[i];
                sliceViews[i].Setup(data);
            }
        }
        
        public void RotateWheel(int targetIndex, Action onComplete)
        {
            StopRotation();

            _targetSliceIndex = targetIndex;
            _lastTickSlice = -1;
            _onRotationComplete = onComplete;
            _indicatorTween.Rewind();
            
            float currentAngle = wheelBaseImage.rectTransform.eulerAngles.z % 360f;
            float randomOffset = Random.Range(-18f, 18f);
            float targetSliceAngle = targetIndex * 45f + randomOffset;
    
            float delta = targetSliceAngle - currentAngle;
            if (delta <= 0f)
            {
                delta += 360f;
            }
            float finalAngle = currentAngle + fullTurns * 360f + delta;

            _rotationTween = wheelBaseImage.rectTransform
                .DORotate(new Vector3(0, 0, finalAngle), spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnUpdate(HandleSpinUpdate)
                .OnComplete(HandleSpinComplete);
        }
        
        public void ResetRotation(float duration, Action onComplete)
        {
            StopRotation();
            _onRotationComplete = onComplete;
            _indicatorTween.Rewind();
            _rotationTween = wheelBaseImage.rectTransform
                .DORotate(Vector3.zero, duration)
                .SetEase(Ease.OutQuad)
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(CompleteRotation);
        }


        private void HandleSpinUpdate()
        {
            float angle = wheelBaseImage.rectTransform.eulerAngles.z;
            int currentSlice = Mathf.FloorToInt(angle / 45f);

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

        private void StopRotation()
        {
            Tween previousTween = _rotationTween;
            _rotationTween = null;
            _onRotationComplete = null;
            previousTween?.Kill();
        }
        
        public void SetSpinInteractable(bool interactable)
        {
            if (spinButton != null) spinButton.interactable = interactable;
        }

        private void HandleSpinClicked()
        {
            OnSpinClicked?.Invoke();
        }
    }
}