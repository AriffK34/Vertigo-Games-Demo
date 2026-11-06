using VertigoDemo.Data;

namespace VertigoDemo.Core.Contracts
{
    public interface IZoneReader
    {
        #region Properties

        int CurrentZone
        {
            get;
        }

        ZoneRuleConfig CurrentRule
        {
            get;
        }

        bool IsLeaveAllowed
        {
            get;
        }

        #endregion

        #region Public Methods

        ZoneRuleConfig GetRule(int zone);

        #endregion
    }
}
