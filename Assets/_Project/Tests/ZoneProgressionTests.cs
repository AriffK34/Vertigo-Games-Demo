using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.Tests
{
    public class ZoneProgressionTests
    {
        #region Fields

        private ZoneRuleConfig _rule;

        #endregion

        #region Setup and Teardown

        [SetUp]
        public void SetUp()
        {
            _rule = ScriptableObject.CreateInstance<ZoneRuleConfig>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_rule);
        }

        #endregion

        #region Tests

        [Test]
        public void AdvanceZone_MovesForwardOneZonePerCall()
        {
            ZoneProgression progression = CreateProgression();

            progression.AdvanceZone();
            Assert.That(progression.CurrentZone, Is.EqualTo(2));

            progression.AdvanceZone();
            Assert.That(progression.CurrentZone, Is.EqualTo(3));
        }

        [Test]
        public void ResetZone_AfterProgress_ReturnsToFirstZoneAndAllowsProgress()
        {
            ZoneProgression progression = CreateProgression();

            progression.AdvanceZone();
            progression.AdvanceZone();
            progression.ResetZone();

            Assert.That(progression.CurrentZone, Is.EqualTo(1));

            progression.AdvanceZone();

            Assert.That(progression.CurrentZone, Is.EqualTo(2));
        }

        [Test]
        public void AdvanceZone_WhenNextZoneHasNoRule_PreservesCurrentZone()
        {
            ZoneProgression progression = CreateProgression(1);

            Assert.Throws<InvalidOperationException>(
                () => progression.AdvanceZone());

            Assert.That(progression.CurrentZone, Is.EqualTo(1));
        }

        #endregion

        #region Private Methods

        private ZoneProgression CreateProgression(int lastZone = 0)
        {
            SerializedObject serializedRule = new SerializedObject(_rule);
            serializedRule.FindProperty("_lastZone").intValue = lastZone;
            serializedRule.ApplyModifiedPropertiesWithoutUndo();

            ZoneRuleResolver resolver = new ZoneRuleResolver(new[] { _rule });

            return new ZoneProgression(resolver);
        }

        #endregion
    }
}
