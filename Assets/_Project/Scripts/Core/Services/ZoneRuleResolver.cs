using System;
using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public sealed class ZoneRuleResolver
    {
        #region Fields

        private readonly IReadOnlyList<ZoneRuleConfig> _rules;

        #endregion

        #region Constructors

        public ZoneRuleResolver(IReadOnlyList<ZoneRuleConfig> rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (rules.Count == 0)
            {
                throw new ArgumentException("At least one zone rule is required.", nameof(rules));
            }

            foreach (ZoneRuleConfig rule in rules)
            {
                if (!rule)
                {
                    throw new ArgumentException("The zone rule list contains an empty entry", nameof(rules));
                }
            }
            _rules = rules;
        }

        #endregion

        #region Public Methods

        public ZoneRuleConfig GetRule(int zone)
        {
            if (zone < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(zone));
            }

            foreach (ZoneRuleConfig rule in _rules)
            {
                if (rule.IsMatch(zone))
                {
                    return rule;
                }
            }

            throw new InvalidOperationException($"No zone rule matches zone {zone}");
        }

        #endregion
    }
}
