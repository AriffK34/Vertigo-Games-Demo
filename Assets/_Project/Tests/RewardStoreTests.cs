using NUnit.Framework;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.Tests
{
    public class RewardStoreTests
    {
        #region Fields

        private ItemData _item;

        #endregion

        #region Setup and Cleanup

        [SetUp]
        public void SetUp()
        {
            _item = ScriptableObject.CreateInstance<ItemData>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_item);
        }

        #endregion

        #region Tests

        [Test]
        public void TrySpend_WhenBalanceIsInsufficient_PreservesBalance()
        {
            RewardStore store = new RewardStore();
            store.Add(_item, 10);
            bool isSuccessful = store.TrySpend(_item, 15);
            Assert.That(isSuccessful, Is.False);
            Assert.That(store.GetAmount(_item), Is.EqualTo(10));
        }

        [Test]
        public void Add_WhenSameItemIsAddedAgain_AccumulatesAmount()
        {
            RewardStore store = new RewardStore();
            store.Add(_item, 10);
            store.Add(_item, 5);
            Assert.That(store.GetAmount(_item), Is.EqualTo(15));
        }

        [TestCase(4, 6)]
        [TestCase(10, 0)]
        public void TrySpend_WhenBalanceIsSufficient_DeductsAmount(int spendAmount, int expectedBalance)
        {
            RewardStore store = new RewardStore();
            store.Add(_item, 10);
            bool isSuccessful = store.TrySpend(_item, spendAmount);
            Assert.That(isSuccessful, Is.True);
            Assert.That(store.GetAmount(_item), Is.EqualTo(expectedBalance));
        }
        
        [TestCase(0)]
        [TestCase(-5)]
        public void TrySpend_WhenAmountIsNotPositive_RejectsRequest(int spendAmount)
        {
            RewardStore store = new RewardStore();
            store.Add(_item, 10);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => store.TrySpend(_item, spendAmount));
            Assert.That(store.GetAmount(_item), Is.EqualTo(10));
        }
        
        #endregion
    }
}