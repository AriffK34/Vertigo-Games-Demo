using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "RuntimeConfig", menuName = "Wheel/Runtime Config")]
    public class RuntimeConfig : ScriptableObject
    {
        #region Fields

        [SerializeField, Min(1)] private int _targetFrameRate = 60;

        #endregion

        #region Properties

        public int TargetFrameRate
        {
            get
            {
                return _targetFrameRate;
            }
        }

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            _targetFrameRate = Mathf.Max(1, _targetFrameRate);
        }

        #endregion
    }
}
