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
        
        protected void OnValidate()
        {
            if (spinButton == null) spinButton = GetComponentInChildren<Button>(true);
            if (sliceViews is not { Length: 8 }) sliceViews = GetComponentsInChildren<WheelSliceView>(true);
        }

        private void Awake()
        {
            spinButton.onClick.AddListener(HandleSpinClicked);
            raysTransform.DORotate(new Vector3(0, 0, -360), 30f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
        
        protected void OnDestroy()
        {
            if (spinButton != null) spinButton.onClick.RemoveListener(HandleSpinClicked);
            if (wheelBaseImage != null) wheelBaseImage.rectTransform.DOKill();
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
            float currentAngle = wheelBaseImage.rectTransform.eulerAngles.z % 360f;
            float randomOffset = Random.Range(-18f, 18f);
            float targetSliceAngle = targetIndex * 45f + randomOffset;
    
            float delta = targetSliceAngle - currentAngle;
            if (delta <= 0f) delta += 360f;
            float finalAngle = currentAngle + (fullTurns * 360f) + delta;

            int lastTickSlice = -1;

            wheelBaseImage.rectTransform
                .DORotate(new Vector3(0, 0, finalAngle), spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnUpdate(() =>
                {
                    float z = wheelBaseImage.rectTransform.eulerAngles.z;
                    int currentSlice = Mathf.FloorToInt(z / 45f);
                    if (currentSlice == lastTickSlice) return;
                    lastTickSlice = currentSlice;
                    indicatorImage.rectTransform.DOKill();
                    indicatorImage.rectTransform.localEulerAngles = Vector3.zero;
                    indicatorImage.rectTransform.DOPunchRotation(new Vector3(0, 0, -20f), 0.15f, 1, 0.5f);
                })
                .OnComplete(() =>
                {
                    indicatorImage.rectTransform.DOKill();
                    indicatorImage.rectTransform.localEulerAngles = Vector3.zero;
                    sliceViews[targetIndex].PlayPunchAnimation();
                    onComplete?.Invoke();
                });
        }
        
        public void ResetRotation(float duration, Action onComplete)
        {
            wheelBaseImage.rectTransform
                .DORotate(Vector3.zero, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => onComplete?.Invoke());
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