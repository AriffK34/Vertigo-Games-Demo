using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewWheelTheme", menuName = "Wheel/Wheel Theme")]
    public class WheelTheme : ScriptableObject
    {
        #region Fields

        [SerializeField] private Sprite wheelSprite;
        [SerializeField] private Sprite indicatorSprite;

        #endregion

        #region Properties

        public Sprite WheelSprite { get { return wheelSprite; } }
        public Sprite IndicatorSprite { get { return indicatorSprite; } }

        #endregion
    }
}