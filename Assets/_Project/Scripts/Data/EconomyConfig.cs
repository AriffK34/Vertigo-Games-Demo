using System;
using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "Wheel/Economy Config")]
    public class EconomyConfig : ScriptableObject
    {
        #region Fields

        [SerializeField] private ItemData _reviveCurrency;
        [SerializeField, Min(1)] private int _reviveCost = 25;

        #endregion

        #region Properties

        public ItemData ReviveCurrency
        {
            get
            {
                return _reviveCurrency;
            }
        }

        public int ReviveCost
        {
            get
            {
                return _reviveCost;
            }
        }

        #endregion

        #region Validation

        private void OnValidate()
        {
            _reviveCost = Math.Max(1, _reviveCost);
        }
        
        #endregion
    }
}