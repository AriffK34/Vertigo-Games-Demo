using System;
using System.Collections.Generic;
using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "ZoneProgressionConfig", menuName = "Wheel/Zone Progression Config")]
    public class ZoneProgressionConfig : ScriptableObject
    {
        #region Fields

        [SerializeField] private ZoneRuleConfig[] _rules = Array.Empty<ZoneRuleConfig>();

        #endregion

        #region Properties

        public IReadOnlyList<ZoneRuleConfig> Rules
        {
            get
            {
                return _rules;
            }
        }

        #endregion
    }
}