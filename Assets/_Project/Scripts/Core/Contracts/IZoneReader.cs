using VertigoDemo.Data;

namespace VertigoDemo.Core.Contracts
{
    public interface IZoneReader
    {
        int CurrentZone { get; }

        ZoneRuleConfig CurrentRule { get; }

        bool CanLeave { get; }

        ZoneRuleConfig GetRule(int zone);
    }
}