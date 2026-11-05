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

        private ZoneProgression _zoneProgression;
        private readonly RewardCalculator _rewardCalculator = new RewardCalculator();
        
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

        #region Public Methods

        public int CalculateRewardAmount(ItemData itemData, int baseAmount)
        {
            return _rewardCalculator.Calculate(itemData, baseAmount, CurrentZone);
        }
        
        #endregion
    }
}
