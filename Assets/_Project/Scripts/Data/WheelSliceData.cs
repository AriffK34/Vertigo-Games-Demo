using UnityEngine;

namespace VertigoDemo.Data
{
    [System.Serializable]
    public class WheelSliceData
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int itemAmount;
        
        public ItemData ItemData => itemData;
        public int ItemAmount => itemAmount;
        public bool IsBomb => itemData != null && itemData.ItemType == ItemType.Bomb;
    }
}
