using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class ZoneBarView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ZoneSlotView[] _zoneSlots;
        [SerializeField] private RectTransform _content;
        [SerializeField] private GridLayoutGroup _layout;
        [SerializeField] private UIAnimationConfig _animationConfig;

        private Tween _slideTween;
        private ZoneSlotDisplayData[] _pendingSlotsData;
        private Action _onTransitionComplete;

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (_content)
            {
                _zoneSlots = _content.GetComponentsInChildren<ZoneSlotView>(true);
            }
        }

        private void OnDestroy()
        {
            StopSlide();
        }

        #endregion
        
        #region Public Methods

        public void UpdateSlots(ZoneSlotDisplayData[] slotsData, bool hasAnimation, Action onComplete)
        {
            StopSlide();
            _content.anchoredPosition = Vector2.zero;

            if (slotsData == null || slotsData.Length != _zoneSlots.Length)
            {
                throw new ArgumentException(
                    "Zone display data must match the number of zone slots.");
            }

            if (!hasAnimation)
            {
                ApplySlots(slotsData);
                onComplete?.Invoke();

                return;
            }

            _pendingSlotsData = slotsData;
            _onTransitionComplete = onComplete;

            float slideDistance = _layout.cellSize.x + _layout.spacing.x;

            _slideTween = _content
                .DOAnchorPosX(-slideDistance, _animationConfig.ZoneSlideDuration)
                .SetEase(_animationConfig.ZoneSlideEase)
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(HandleSlideComplete);
        }
        
        #endregion

        #region Private Methods

        private void ApplySlots(ZoneSlotDisplayData[] slotsData)
        {
            for (int i = 0; i < _zoneSlots.Length; i++)
            {
                _zoneSlots[i].Setup(slotsData[i].BgSprite, slotsData[i].Text);
            }
        }

        private void HandleSlideComplete()
        {
            _slideTween = null;
            ApplySlots(_pendingSlotsData);
            _content.anchoredPosition = Vector2.zero;
            _pendingSlotsData = null;

            Action onComplete = _onTransitionComplete;
            _onTransitionComplete = null;
            onComplete?.Invoke();
        }

        private void StopSlide()
        {
            _slideTween?.Kill();
            _slideTween = null;
            _pendingSlotsData = null;
            _onTransitionComplete = null;
        }

        #endregion

    }

}