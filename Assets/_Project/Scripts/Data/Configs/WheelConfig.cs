using System;
using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewWheelConfig", menuName = "Wheel/Wheel Config")]
    public class WheelConfig : ScriptableObject
    {
        #region Fields

        [SerializeField] private WheelTheme _wheelTheme;
        [SerializeField] private WheelSliceData[] _wheelSlices = new WheelSliceData[8];

        #endregion

        #region Properties

        public WheelTheme WheelTheme
        {
            get
            {
                return _wheelTheme;
            }
        }

        public WheelSliceData[] WheelSlices
        {
            get
            {
                return _wheelSlices;
            }
        }

        #endregion

        #region Public Methods

        public void ValidateConfiguration(bool hasBomb)
        {
            if (_wheelSlices == null || _wheelSlices.Length == 0)
            {
                throw new InvalidOperationException($"{name}: Wheel must contain slices.");
            }
            int bombCount = 0;
            for (int i = 0; i < _wheelSlices.Length; i++)
            {
                WheelSliceData slice = _wheelSlices[i];
                if (slice == null || !slice.ItemData)
                {
                    throw new InvalidOperationException($"{name}: Slice {i} requires an item.");
                }

                if (slice.ItemAmount < 0)
                {
                    throw new InvalidOperationException($"{name}: Reward slice {i} must have a positive amount.");
                }

                if (slice.IsBomb)
                {
                    bombCount++;
                }
                else if (slice.ItemAmount == 0)
                {
                    throw new InvalidOperationException($"{name}: Reward slice {i} must have a positive amount.");
                }
            }

            int expectedBombCount = hasBomb ? 1 : 0;
            if (bombCount != expectedBombCount)
            {
                throw new InvalidOperationException(
                    $"{name}: Expected {expectedBombCount} bomb slice(s), but found {bombCount}");
            }
        }

        #endregion
    }
}
