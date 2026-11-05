using System;
using UnityEngine;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public class ZoneManager : MonoBehaviour, IZoneReader
    {
        #region Events

        public event Action OnZoneChanged;

        #endregion

        #region Fields

        [SerializeField] private ZoneProgressionConfig _progressionConfig;
        [SerializeField, Min(0f)] private float _growthMultiplier = 10f;
        [SerializeField, Min(0f)] private float _growthExponent = 0.08f;
        [SerializeField, Min(0f)] private float _linearSlope = 0.3334f;
        [SerializeField, Min(1)] private int _growthEndZone = 30;

        private ZoneProgression _zoneProgression;
        
        #endregion

        #region Properties

        public int CurrentZone
        {
            get
            {
                return _zoneProgression.CurrentZone;
            }
        }

        public ZoneRuleConfig CurrentRule
        {
            get
            {
                return _zoneProgression.CurrentRule;
            }
        }

        public bool CanLeave
        {
            get
            {
                return _zoneProgression.CanLeave;
            }
        }

        #endregion

        #region Lifecycle

        private void Awake()
        {
            if (!_progressionConfig)
            {
                throw new InvalidOperationException("Zone progression config must be assigned.");
            }
            ZoneRuleResolver ruleResolver = new ZoneRuleResolver(_progressionConfig.Rules);
            foreach (ZoneRuleConfig rule in _progressionConfig.Rules)
            {
                rule.ValidateWheel();
            }

            _zoneProgression = new ZoneProgression(ruleResolver);

        }

        #endregion

        #region Public Methods

        public ZoneRuleConfig GetRule(int zone)
        {
            return _zoneProgression.GetRule(zone);
        }

        public WheelConfig GetCurrentWheel()
        {
            return CurrentRule.Wheel;
        }

        public void AdvanceZone()
        {
            _zoneProgression.AdvanceZone();
            OnZoneChanged?.Invoke();
        }
        
        public void ResetZone()
        {
            _zoneProgression.ResetZone();
            OnZoneChanged?.Invoke();
        }

        #endregion

        #region Reward Calculation

        public int CalculateRewardAmount(ItemData itemData, int baseAmount)
        {
            if (itemData.ItemType == ItemType.Item)
            {
                float itemMultiplier = 1f + Mathf.Sqrt((CurrentZone - 1f) / 10f);
                return Mathf.Max(1, Mathf.RoundToInt(baseAmount * itemMultiplier));
            }

            float multiplier = GetMultiplier(CurrentZone);
            int rawAmount = Mathf.FloorToInt(baseAmount * multiplier);
            
            return RoundToCleanNumber(rawAmount);
        }

        private int GetRoundedStep(float magnitude)
        {
            if (magnitude >= 100f)
            {
                return (int)(magnitude / 10f);
            }

            return 5;
        }

        private int RoundToCleanNumber(int value)
        {
            if (value < 10)
            {
                return value;
            }

            float magnitude = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(value)));
            int step = GetRoundedStep(magnitude);
            return Mathf.RoundToInt((float)value / step) * step;
        }

        public float GetMultiplier(int zone)
        {
            if (zone <= _growthEndZone)
            {
                return GrowthFunction(zone);
            }

            return GrowthFunction(_growthEndZone) + _linearSlope * (zone - _growthEndZone);
        }
        
        private float GrowthFunction(int zone)
        {
            return 1f + _growthMultiplier * (1f - Mathf.Exp(-_growthExponent * (zone - 1)));
        }

        #endregion
    }
}
