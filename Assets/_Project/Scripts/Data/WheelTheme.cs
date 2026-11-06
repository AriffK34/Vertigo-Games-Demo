using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewWheelTheme", menuName = "Wheel/Wheel Theme")]
    public class WheelTheme : ScriptableObject
    {
        #region Fields

        [SerializeField] private Sprite _wheelSprite;
        [SerializeField] private Sprite _indicatorSprite;

        #endregion

        #region Properties

        public Sprite WheelSprite
        {
            get
            {
                return _wheelSprite;
            }
        }

        public Sprite IndicatorSprite
        {
            get
            {
                return _indicatorSprite;
            }
        }

        #endregion
    }
}
