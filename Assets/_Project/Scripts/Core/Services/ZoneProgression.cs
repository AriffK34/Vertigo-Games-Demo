using System;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public sealed class ZoneProgression : IZoneReader
    {
        #region Fields

        private const int FIRST_ZONE = 1;

        private readonly ZoneRuleResolver _ruleResolver;
        private int _currentZone = FIRST_ZONE;

        #endregion

        #region Properties

        public int CurrentZone
        {
            get
            {
                return _currentZone;
            }
        }

        public ZoneRuleConfig CurrentRule
        {
            get
            {
                return GetRule(_currentZone);
            }
        }

        public bool CanLeave
        {
            get
            {
                return CurrentRule.IsLeaveAllowed;
            }
        }

        #endregion

        #region Constructor

        public ZoneProgression(ZoneRuleResolver ruleResolver)
        {
            if (ruleResolver == null)
            {
                throw new ArgumentNullException(nameof(ruleResolver));
            }

            _ruleResolver = ruleResolver;
            _ruleResolver.GetRule(FIRST_ZONE);
        }
 
        #endregion

        #region Public Methods

        public ZoneRuleConfig GetRule(int zone)
        {
            return _ruleResolver.GetRule(zone);
        }

        public void AdvanceZone()
        {
            int nextZone = checked(_currentZone + 1);
            _ruleResolver.GetRule(nextZone);
            _currentZone = nextZone;
        }

        public void ResetZone()
        {
            _currentZone = FIRST_ZONE;
        }

        #endregion
    }
}