using System;
using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "RewardGrowthConfig", menuName = "Wheel/Reward Growth Config")]
    public class RewardGrowthConfig : ScriptableObject
    {
        #region Fields

        [SerializeField, Min(0f)] private float _growthPerZone = 0.05f;

        #endregion

        #region Properties

        public float GrowthPerZone
        {
            get
            {
                return _growthPerZone;
            }
        }

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            _growthPerZone = Math.Max(0f, _growthPerZone);
        }

        #endregion
    }
}