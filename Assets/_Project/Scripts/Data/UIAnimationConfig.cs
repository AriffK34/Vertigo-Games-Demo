using DG.Tweening;
using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "UIAnimationConfig", menuName = "Wheel/UI Animation Config")]
    public class UIAnimationConfig : ScriptableObject
    {
        #region Fields

        [SerializeField, Min(0.01f)] private float _zoneSlideDuration = 0.35f;
        [SerializeField] private Ease _zoneSlideEase = Ease.OutCubic;
        [SerializeField, Min(0.01f)] private float _popupShowDuration = 0.35f;
        [SerializeField] private Ease _popupShowEase = Ease.OutBack;
        [SerializeField, Min(0.01f)] private float _popupHideDuration = 0.2f;
        [SerializeField] private Ease _popupHideEase = Ease.InBack;
        [SerializeField, Min(0.01f)] private float _rewardFlightDuration = 0.5f;
        [SerializeField, Min(1f)] private float _rewardFlightPeakScale = 1.4f;
        [SerializeField] private Ease _rewardFlightEase = Ease.InOutQuad;
        [SerializeField, Min(0.01f)] private float _burstDuration = 0.5f;
        [SerializeField, Min(1f)] private float _burstPeakScale = 1.15f;
        [SerializeField, Range(0f, 1f)] private float _burstBrightness = 0.3f;
        [SerializeField] private Ease _burstEase = Ease.OutQuad;
        #endregion

        #region Properties

        public float ZoneSlideDuration
        {
            get
            {
                return _zoneSlideDuration;
            }
        }

        public Ease ZoneSlideEase
        {
            get
            {
                return _zoneSlideEase;
            }
        }

        public float PopupShowDuration
        {
            get
            {
                return _popupShowDuration;
            }
        }

        public Ease PopupShowEase
        {
            get
            {
                return _popupShowEase;
            }
        }

        public float PopupHideDuration
        {
            get
            {
                return _popupHideDuration;
            }
        }

        public Ease PopupHideEase
        {
            get
            {
                return _popupHideEase;
            }
        }

        public float RewardFlightDuration
        {
            get
            {
                return _rewardFlightDuration;
            }
        }

        public float RewardFlightPeakScale
        {
            get
            {
                return _rewardFlightPeakScale;
            }
        }

        public Ease RewardFlightEase
        {
            get
            {
                return _rewardFlightEase;
            }
        }
        
        public float BurstDuration
        {
            get
            {
                return _burstDuration;
            }
        }

        public float BurstPeakScale
        {
            get
            {
                return _burstPeakScale;
            }
        }

        public float BurstBrightness
        {
            get
            {
                return _burstBrightness;
            }
        }

        public Ease BurstEase
        {
            get
            {
                return _burstEase;
            }
        }

        #endregion
        
        #region Validation

        private void OnValidate()
        {
            _zoneSlideDuration = Mathf.Max(0.01f, _zoneSlideDuration);
            _popupShowDuration = Mathf.Max(0.01f, _popupShowDuration);
            _popupHideDuration = Mathf.Max(0.01f, _popupHideDuration);
            _rewardFlightPeakScale = Mathf.Max(1f, _rewardFlightPeakScale);
            _rewardFlightDuration = Mathf.Max(0.01f, _rewardFlightDuration);
            _burstDuration = Mathf.Max(0.01f, _burstDuration);
            _burstPeakScale = Mathf.Max(1f, _burstPeakScale);
            _burstBrightness = Mathf.Clamp01(_burstBrightness);
        }

        #endregion
    }
}