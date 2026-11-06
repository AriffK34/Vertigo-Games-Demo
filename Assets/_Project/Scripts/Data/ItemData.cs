using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Wheel/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private RewardGrowthConfig _growthProfile;

        public Sprite ItemIcon { get { return itemIcon; } }
        public RewardGrowthConfig GrowthProfile { get { return _growthProfile; } }
    }
}
