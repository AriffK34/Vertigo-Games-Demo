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
                rect.anchoredPosition = direction * distance * 0.25f;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.sizeDelta = Vector2.one * 100f;
                particle.color = new Color(0.14f, 0.12f, 0.1f, 0.7f);
                particle.enabled = true;
                sequence.Insert(0f, rect.DOAnchorPos(direction * distance * 0.7f, duration).SetEase(Ease.OutCubic));
                sequence.Insert(0f, rect.DOSizeDelta(Vector2.one * 240f, duration).SetEase(Ease.OutQuad));
                sequence.Insert(0f, particle.DOFade(0f, duration).SetEase(Ease.InQuad));
            }

            for (int index = 0; index < _sparks.Length; index++)
            {
                Image particle = _sparks[index];
                float angle = (index + 0.25f) * Mathf.PI * 2f / _sparks.Length;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float sparkDuration = duration * (0.7f + (index % 3) * 0.1f);
                RectTransform rect = particle.rectTransform;
                rect.anchoredPosition = direction * distance * 0.35f;
                rect.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                rect.localScale = Vector3.one;
                rect.sizeDelta = new Vector2(64f, 18f);
                particle.color = index % 2 == 0
                    ? new Color(1f, 0.8f, 0.25f, 1f)
                    : new Color(1f, 0.35f, 0.05f, 1f);
                particle.enabled = true;
                sequence.Insert(0f, rect.DOAnchorPos(direction * distance, sparkDuration).SetEase(Ease.OutCubic));
                sequence.Insert(0f, rect.DOSizeDelta(new Vector2(18f, 5f), sparkDuration));
                sequence.Insert(sparkDuration * 0.2f,
                    particle.DOFade(0f, sparkDuration * 0.8f).SetEase(Ease.InQuad));
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
