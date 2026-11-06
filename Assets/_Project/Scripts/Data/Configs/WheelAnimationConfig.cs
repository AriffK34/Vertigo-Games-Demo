using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "WheelAnimationConfig", menuName = "Wheel/Wheel Animation Config")]
    public class WheelAnimationConfig : ScriptableObject
    {
        #region Fields

        [SerializeField, Min(0.01f)] private float _resetDuration = 0.6f;

        #endregion

        #region Properties

        public float ResetDuration
        {
            get
            {
                return _resetDuration;
            }
        }

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            _resetDuration = Mathf.Max(0.01f, _resetDuration);
        }

        #endregion
    }
}