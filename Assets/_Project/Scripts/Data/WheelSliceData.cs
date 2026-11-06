using UnityEngine;

namespace VertigoDemo.Data
{
    [System.Serializable]
    public class WheelSliceData
    {
        #region Fields

        [SerializeField] private ItemData itemData;
        [SerializeField, Min(0)] private int itemAmount;
        [SerializeField] private bool _isBomb;

        #endregion

        #region Properties

        public ItemData ItemData { get { return itemData; } }
        public int ItemAmount { get { return itemAmount; } }
        public bool IsBomb { get { return _isBomb; } }

        #endregion
    }
}