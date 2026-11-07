using DG.Tweening;
using UnityEngine;
using VertigoDemo.Data;

namespace VertigoDemo.Presentation.Animations
{
    public static class PopupAnimation
    {
        #region Public Methods

        public static Tween Show(Transform animatedTransform, UIAnimationConfig config)
        {
            animatedTransform.localScale = Vector3.zero;
            return animatedTransform
                .DOScale(Vector3.one, config.PopupShowDuration)
                .SetEase(config.PopupShowEase)
                .SetRecyclable(false)
                .SetLink(animatedTransform.gameObject);
        }

        public static Tween Hide(Transform animatedTransform, UIAnimationConfig config)
        {
            return animatedTransform
                .DOScale(Vector3.zero, config.PopupHideDuration)
                .SetEase(config.PopupHideEase)
                .SetRecyclable(false)
                .SetLink(animatedTransform.gameObject);
        }

        #endregion
    }
}
