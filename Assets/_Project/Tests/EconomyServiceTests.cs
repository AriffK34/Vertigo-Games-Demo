using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.Tests
{
    public class EconomyServiceTests
    {
        #region Constants

        private const int REVIVE_COST = 37;

        #endregion

        #region Fields

        private ItemData _gold;
        private ItemData _cash;
        private ItemData _chest;
        private EconomyConfig _config;
        private EconomyService _economy;

        #endregion

        #region Setup and Teardown

        [SetUp]
        public void SetUp()
        {
            _gold = ScriptableObject.CreateInstance<ItemData>();
            _cash = ScriptableObject.CreateInstance<ItemData>();
            _chest = ScriptableObject.CreateInstance<ItemData>();
            _config = ScriptableObject.CreateInstance<EconomyConfig>();
            SerializedObject serializedConfig = new SerializedObject(_config);
            serializedConfig.FindProperty("_reviveCurrency").objectReferenceValue = _cash;
            serializedConfig.FindProperty("_reviveCost").intValue = REVIVE_COST;
            serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            _economy = new EconomyService(_config);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_config);
            Object.DestroyImmediate(_gold);
            Object.DestroyImmediate(_cash);
            Object.DestroyImmediate(_chest);
        }

        #endregion

        #region Tests

        [Test]
        public void ClaimRewards_WithDifferentRewards_TransfersAllRewards()
        {
            SessionInventory runInventory = new SessionInventory();
            runInventory.AddItem(_gold, 100);
            runInventory.AddItem(_cash, 50);
            runInventory.AddItem(_chest, 3);
            _economy.ClaimRewards(runInventory);
            Assert.That(_economy.GetBalance(_gold), Is.EqualTo(100));
            Assert.That(_economy.GetBalance(_cash), Is.EqualTo(50));
            Assert.That(_economy.GetBalance(_chest), Is.EqualTo(3));
            Assert.That(runInventory.Inventory, Is.Empty);
        }

        [Test]
        public void ClaimRewards_WhenCalledTwice_DoesNotDuplicateRewards()
        {
            SessionInventory runInventory = new SessionInventory();
            runInventory.AddItem(_chest, 3);
            _economy.ClaimRewards(runInventory);
            _economy.ClaimRewards(runInventory);
            Assert.That(_economy.GetBalance(_chest), Is.EqualTo(3));
            Assert.That(runInventory.Inventory, Is.Empty);
        }

        [TestCase(36, false, 36)]
        [TestCase(37, true, 0)]
        [TestCase(50, true, 13)]
        public void TryRevive_UsesConfiguredCurrencyAndCost(int initialBalance, bool expectedSuccess,
            int expectedBalance)
        {
            SessionInventory runInventory = new SessionInventory();
            runInventory.AddItem(_gold, 100);
            runInventory.AddItem(_cash, initialBalance);
            runInventory.AddItem(_chest, 3);
            _economy.ClaimRewards(runInventory);
            bool hasReviveFunds = _economy.HasReviveFunds;
            bool isSuccessful = _economy.TryRevive();
            Assert.That(hasReviveFunds, Is.EqualTo(expectedSuccess));
            Assert.That(isSuccessful, Is.EqualTo(expectedSuccess));
            Assert.That(_economy.GetBalance(_cash), Is.EqualTo(expectedBalance));
            Assert.That(_economy.GetBalance(_gold), Is.EqualTo(100));
            Assert.That(_economy.GetBalance(_chest), Is.EqualTo(3));
        }

        #endregion
    }
}
