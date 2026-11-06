using UnityEngine;

namespace VertigoDemo.Data
{
    [System.Serializable]
    public class WheelSliceData
    {
        #region Fields

        [SerializeField] private ItemData _itemData;
        [SerializeField, Min(0)] private int _itemAmount;
        [SerializeField] private bool _isBomb;

        #endregion

        #region Properties

        public ItemData ItemData
        {
            get
            {
                return _itemData;
            }
        }

        public int ItemAmount
        {
            get
            {
                return _itemAmount;
            }
        }

        public bool IsBomb
        {
            get
            {
                return _isBomb;
            }
        }

        #endregion
    }
}
