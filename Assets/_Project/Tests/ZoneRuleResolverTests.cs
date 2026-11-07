using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.Tests
{
    public class ZoneRuleResolverTests
    {
        #region Fields

        private readonly List<ZoneRuleConfig> _createdRules = new List<ZoneRuleConfig>();
        private ZoneRuleConfig _normal;
        private ZoneRuleConfig _safe;
        private ZoneRuleConfig _super;
        private ZoneRuleResolver _resolver;

        #endregion

        #region Setup and Teardown

        [SetUp]
        public void SetUp()
        {
            _normal = CreateRule(1, 0, 1);
            _safe = CreateRule(1, 0, 5);
            _super = CreateRule(1, 0, 30);
            _resolver = new ZoneRuleResolver(new[] { _super, _safe,  _normal });
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var rule in _createdRules)
            {
                Object.DestroyImmediate(rule);
            }
            _createdRules.Clear();
        }

        #endregion

        #region Tests

        [TestCase(1)]
        [TestCase(29)]
        public void GetRule_ForNormalZone_ReturnsNormalRule(int zone)
        {
            ZoneRuleConfig result = _resolver.GetRule(zone);
            Assert.That(result, Is.SameAs(_normal));
        }

        [TestCase(5)]
        [TestCase(10)]
        public void GetRule_ForSafeZone_ReturnsSafeRule(int zone)
        {
            ZoneRuleConfig result = _resolver.GetRule(zone);
            Assert.That(result, Is.SameAs(_safe));
        }

        [TestCase(30)]
        [TestCase(60)]
        public void GetRule_ForSuperZone_ReturnsSuperRule(int zone)
        {
            ZoneRuleConfig result = _resolver.GetRule(zone);
            Assert.That(result, Is.SameAs(_super));
        }

        [Test]
        public void GetRule_WithAdditionalRangeRule_RespectsRangeAndOrder()
        {
            ZoneRuleConfig advanced = CreateRule(11, 29, 1);
            ZoneRuleResolver resolver = new ZoneRuleResolver(new[] { _super, _safe, advanced, _normal });
            Assert.That(resolver.GetRule(9), Is.SameAs(_normal));
            Assert.That(resolver.GetRule(11), Is.SameAs(advanced));
            Assert.That(resolver.GetRule(29), Is.SameAs(advanced));
            Assert.That(resolver.GetRule(31), Is.SameAs(_normal));
            Assert.That(resolver.GetRule(15), Is.SameAs(_safe));
        }

        #endregion

        #region Private Methods

        private ZoneRuleConfig CreateRule(int firstZone, int lastZone, int interval)
        {
            ZoneRuleConfig rule = ScriptableObject.CreateInstance<ZoneRuleConfig>();
            _createdRules.Add(rule);
            SerializedObject serializedRule = new SerializedObject(rule);
            serializedRule.FindProperty("_firstZone").intValue = firstZone;
            serializedRule.FindProperty("_lastZone").intValue = lastZone;
            serializedRule.FindProperty("_interval").intValue = interval;
            serializedRule.ApplyModifiedPropertiesWithoutUndo();
            return rule;
        }

        #endregion
    }
}
