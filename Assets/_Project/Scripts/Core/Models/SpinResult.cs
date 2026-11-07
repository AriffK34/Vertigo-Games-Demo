using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public readonly struct SpinResult
    {
        #region Fields

        public readonly int SliceIndex;
        public readonly ItemData Item;
        public readonly int Amount;
        public readonly bool IsBomb;

        #endregion

        #region Constructors

        public SpinResult(
            int sliceIndex,
            ItemData item,
            int amount,
            bool isBomb)
        {
            SliceIndex = sliceIndex;
            Item = item;
            Amount = amount;
            IsBomb = isBomb;
        }

        #endregion
    }
}
