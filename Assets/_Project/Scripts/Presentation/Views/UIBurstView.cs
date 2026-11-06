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
        [SerializeField] private Sprite _particleSprite;
        private Tween _burstTween;
        private Vector3 _initialScale;
        private Color _initialColor;
        private Image[] _sparks;
        private Image[] _smoke;

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
            _smoke = CreateParticles(_animationConfig.BombSmokeCount, "ui_image_bomb_smoke_value");
            _sparks = CreateParticles(_animationConfig.BombSparkCount, "ui_image_bomb_spark_value");
        }

        private void OnDisable()
        {
            Stop();
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
            AddParticles(sequence);
            _burstTween = sequence
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(HandleBurstComplete);
        }

        public void Stop()
        {
            _burstTween?.Kill();
            _burstTween = null;
            HideParticles(_sparks);
            HideParticles(_smoke);

            if (_image)
            {
                _image.rectTransform.localScale = _initialScale;
                _image.color = _initialColor;
            }
        }

        #endregion

        #region Private Methods

        private Image[] CreateParticles(int count, string objectName)
        {
            Image[] particles = new Image[count];
            for (int index = 0; index < count; index++)
            {
                GameObject particleObject = new GameObject(
                    objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                particleObject.layer = gameObject.layer;
                particleObject.transform.SetParent(_image.transform, false);
                Image particle = particleObject.GetComponent<Image>();
                particle.sprite = _particleSprite;
                particle.raycastTarget = false;
                particle.maskable = false;
                particle.preserveAspect = false;
                particle.enabled = false;
                particles[index] = particle;
            }
            return particles;
        }

        private void AddParticles(Sequence sequence)
        {
            float duration = _animationConfig.BombParticleDuration;
            float distance = _animationConfig.BombParticleDistance;

            for (int index = 0; index < _smoke.Length; index++)
            {
                Image particle = _smoke[index];
                float angle = (index + 0.5f) * Mathf.PI * 2f / _smoke.Length;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                RectTransform rect = particle.rectTransform;
                rect.anchoredPosition = direction * distance * _animationConfig.BombSmokeStartDistanceRatio;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.sizeDelta = _animationConfig.BombSmokeStartSize;
                particle.color = _animationConfig.BombSmokeColor;
                particle.enabled = true;
                sequence.Insert(0f, rect.DOAnchorPos(direction * distance * _animationConfig.BombSmokeEndDistanceRatio, duration).SetEase(_animationConfig.BombParticleMoveEase));
                sequence.Insert(0f, rect.DOSizeDelta(_animationConfig.BombSmokeEndSize, duration).SetEase(_animationConfig.BombParticleSizeEase));
                sequence.Insert(0f, particle.DOFade(0f, duration).SetEase(_animationConfig.BombParticleFadeEase));
            }

            for (int index = 0; index < _sparks.Length; index++)
            {
                Image particle = _sparks[index];
                float angle = (index + 0.25f) * Mathf.PI * 2f / _sparks.Length;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float sparkDuration = duration * Mathf.Lerp(_animationConfig.BombSparkLifetimeRange.x,
                    _animationConfig.BombSparkLifetimeRange.y, (index % 3) / 2f);
                RectTransform rect = particle.rectTransform;
                rect.anchoredPosition = direction * distance * _animationConfig.BombSparkStartDistanceRatio;
                rect.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                rect.localScale = Vector3.one;
                rect.sizeDelta = _animationConfig.BombSparkStartSize;
                particle.color = index % 2 == 0
                    ? _animationConfig.BombSparkPrimaryColor
                    : _animationConfig.BombSparkSecondaryColor;
                particle.enabled = true;
                sequence.Insert(0f, rect.DOAnchorPos(direction * distance, sparkDuration).SetEase(_animationConfig.BombParticleMoveEase));
                sequence.Insert(0f, rect.DOSizeDelta(_animationConfig.BombSparkEndSize, sparkDuration).SetEase(_animationConfig.BombSparkSizeEase));
                sequence.Insert(sparkDuration * _animationConfig.BombSparkFadeStartRatio,
                    particle.DOFade(0f, sparkDuration * (1f - _animationConfig.BombSparkFadeStartRatio)).SetEase(_animationConfig.BombParticleFadeEase));
            }
        }

        private static void HideParticles(Image[] particles)
        {
            if (particles == null)
            {
                return;
            }
            foreach (Image particle in particles)
            {
                if (particle)
                {
                    particle.enabled = false;
                }
            }
        }

        private void HandleBurstComplete()
        {
            _burstTween = null;
            HideParticles(_sparks);
            HideParticles(_smoke);
        }

        #endregion
    }
}
