using DG.Tweening;
using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(
        fileName = "WheelAnimationConfig",
        menuName = "Wheel/Wheel Animation Config")]
    public class WheelAnimationConfig : ScriptableObject
    {
        #region Fields

        [SerializeField, Min(0.01f)] private float _spinDuration = 3.5f;
        [SerializeField, Min(1)] private int _fullTurns = 5;
        [SerializeField] private Ease _spinEase = Ease.OutCubic;

        [SerializeField, Min(0.01f)] private float _resetDuration = 0.6f;
        [SerializeField] private Ease _resetEase = Ease.OutQuad;

        [SerializeField, Range(0f, 0.49f)]
        private float _landingOffsetRatio = 0.4f;

        [SerializeField, Min(0.01f)] private float _raysDuration = 30f;
        [SerializeField] private Ease _raysEase = Ease.Linear;

        [SerializeField, Min(0f)] private float _indicatorAngle = 20f;
        [SerializeField, Min(0.01f)] private float _indicatorDuration = 0.15f;
        [SerializeField, Min(1)] private int _indicatorVibrato = 1;
        [SerializeField, Range(0f, 1f)] private float _indicatorElasticity = 0.5f;
        [SerializeField] private Ease _indicatorEase = Ease.OutQuad;

        [SerializeField, Min(0f)] private float _slicePunchStrength = 0.25f;
        [SerializeField, Min(0.01f)] private float _slicePunchDuration = 0.4f;
        [SerializeField, Min(1)] private int _slicePunchVibrato = 2;
        [SerializeField, Range(0f, 1f)] private float _slicePunchElasticity = 0.5f;
        [SerializeField] private Ease _slicePunchEase = Ease.OutQuad;

        [SerializeField, Min(0f)] private float _glowEndScale = 1.5f;
        [SerializeField, Min(0.01f)] private float _glowDuration = 0.6f;
        [SerializeField] private Color _glowColor = new Color(1f, 0.95f, 0.6f, 1f);
        [SerializeField] private Ease _glowScaleEase = Ease.OutCubic;
        [SerializeField] private Ease _glowFadeEase = Ease.OutQuad;

        #endregion

        #region Properties

        public float SpinDuration { get { return _spinDuration; } }
        public int FullTurns { get { return _fullTurns; } }
        public Ease SpinEase { get { return _spinEase; } }

        public float ResetDuration { get { return _resetDuration; } }
        public Ease ResetEase { get { return _resetEase; } }

        public float LandingOffsetRatio { get { return _landingOffsetRatio; } }

        public float RaysDuration { get { return _raysDuration; } }
        public Ease RaysEase { get { return _raysEase; } }

        public float IndicatorAngle { get { return _indicatorAngle; } }
        public float IndicatorDuration { get { return _indicatorDuration; } }
        public int IndicatorVibrato { get { return _indicatorVibrato; } }
        public float IndicatorElasticity { get { return _indicatorElasticity; } }
        public Ease IndicatorEase { get { return _indicatorEase; } }

        public float SlicePunchStrength { get { return _slicePunchStrength; } }
        public float SlicePunchDuration { get { return _slicePunchDuration; } }
        public int SlicePunchVibrato { get { return _slicePunchVibrato; } }
        public float SlicePunchElasticity { get { return _slicePunchElasticity; } }
        public Ease SlicePunchEase { get { return _slicePunchEase; } }

        public float GlowEndScale { get { return _glowEndScale; } }
        public float GlowDuration { get { return _glowDuration; } }
        public Color GlowColor { get { return _glowColor; } }
        public Ease GlowScaleEase { get { return _glowScaleEase; } }
        public Ease GlowFadeEase { get { return _glowFadeEase; } }

        #endregion

        #region Validation

        private void OnValidate()
        {
            _spinDuration = Mathf.Max(0.01f, _spinDuration);
            _fullTurns = Mathf.Max(1, _fullTurns);
            _resetDuration = Mathf.Max(0.01f, _resetDuration);
            _landingOffsetRatio = Mathf.Clamp(_landingOffsetRatio, 0f, 0.49f);
            _raysDuration = Mathf.Max(0.01f, _raysDuration);

            _indicatorAngle = Mathf.Max(0f, _indicatorAngle);
            _indicatorDuration = Mathf.Max(0.01f, _indicatorDuration);
            _indicatorVibrato = Mathf.Max(1, _indicatorVibrato);
            _indicatorElasticity = Mathf.Clamp01(_indicatorElasticity);

            _slicePunchStrength = Mathf.Max(0f, _slicePunchStrength);
            _slicePunchDuration = Mathf.Max(0.01f, _slicePunchDuration);
            _slicePunchVibrato = Mathf.Max(1, _slicePunchVibrato);
            _slicePunchElasticity = Mathf.Clamp01(_slicePunchElasticity);

            _glowEndScale = Mathf.Max(0f, _glowEndScale);
            _glowDuration = Mathf.Max(0.01f, _glowDuration);
        }

        #endregion
    }
}