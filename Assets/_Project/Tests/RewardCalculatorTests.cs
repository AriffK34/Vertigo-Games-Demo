using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.Tests
{
    public class RewardCalculatorTests
    {
        #region Fields

        private ItemData _item;
        private RewardGrowthConfig _profile;
        private RewardCalculator _calculator;

        #endregion

        #region Setup and Cleanup

        [SetUp]
        public void SetUp()
        {
            _item = ScriptableObject.CreateInstance<ItemData>();
            _profile = ScriptableObject.CreateInstance<RewardGrowthConfig>();
            _calculator = new RewardCalculator();

            SetProfile(_profile);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_item);
            UnityEngine.Object.DestroyImmediate(_profile);
        }

        #endregion

        #region Tests

        [TestCase(100, 1, 0.05f, 100)]
        [TestCase(100, 2, 0.05f, 105)]
        [TestCase(100, 11, 0.05f, 150)]
        [TestCase(10, 2, 0.05f, 11)]
        [TestCase(1, 20, 0.03f, 2)]
        [TestCase(5, 10, 0f, 5)]
        public void Calculate_UsesConfiguredGrowth(
            int baseAmount,
            int zone,
            float growthPerZone,
            int expectedAmount)
        {
            SerializedObject serializedProfile =
                new SerializedObject(_profile);

            serializedProfile.FindProperty("_growthPerZone").floatValue =
                growthPerZone;

            serializedProfile.ApplyModifiedPropertiesWithoutUndo();

            int result = _calculator.Calculate(_item, baseAmount, zone);

            Assert.That(result, Is.EqualTo(expectedAmount));
        }

        [TestCase(0, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, 0)]
        public void Calculate_WithInvalidInput_Throws(
            int baseAmount,
            int zone)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _calculator.Calculate(_item, baseAmount, zone));
        }

        [Test]
        public void Calculate_WithoutGrowthProfile_Throws()
        {
            SetProfile(null);

            Assert.Throws<InvalidOperationException>(
                () => _calculator.Calculate(_item, 100, 1));
        }

        #endregion

        #region Helpers

        private void SetProfile(RewardGrowthConfig profile)
        {
            SerializedObject serializedItem = new SerializedObject(_item);

            serializedItem.FindProperty("_growthProfile").objectReferenceValue =
                profile;

            serializedItem.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion
    }
}