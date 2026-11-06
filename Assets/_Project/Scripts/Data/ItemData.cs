using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Wheel/Item Data")]
    public class ItemData : ScriptableObject
    {
        #region Fields

        [SerializeField] private Sprite _itemIcon;
        [SerializeField] private RewardGrowthConfig _growthProfile;

        #endregion

        #region Properties

        public Sprite ItemIcon
        {
            get
            {
                return _itemIcon;
            }
        }

        public RewardGrowthConfig GrowthProfile
        {
            get
            {
                return _growthProfile;
            }
        }

        #endregion
    }
}
