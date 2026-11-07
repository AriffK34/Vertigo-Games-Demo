using DG.Tweening;
using UnityEngine;
using VertigoDemo.Data;

namespace VertigoDemo.Presentation.Animations
{
    public static class UIPunchAnimation
    {
        #region Public Methods

        public static Tween Play(
            RectTransform target,
            PunchAnimationSettings settings,
            GameObject owner)
        {
            target.DOKill();
            target.localScale = Vector3.one;

            return target
                .DOPunchScale(
                    Vector3.one * settings.Strength,
                    settings.Duration,
                    settings.Vibrato,
                    settings.Elasticity)
                .SetEase(settings.Ease)
                .SetRecyclable(false)
                .SetLink(owner);
        }

        #endregion
    }
}
