using NUnit.Framework;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Data;

namespace VertigoDemo.Tests
{
    public class ItemCollectionTests
    {
        #region Fields

        private ItemData _item;

        #endregion

        #region Setup and Teardown

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
        public void TrySpend_WhenAmountIsInsufficient_PreservesAmount()
        {
            ItemCollection store = new ItemCollection();
            store.Add(_item, 10);
            bool isSuccessful = store.TrySpend(_item, 15);
            Assert.That(isSuccessful, Is.False);
            Assert.That(store.GetAmount(_item), Is.EqualTo(10));
        }

        [Test]
        public void Add_WhenSameItemIsAddedAgain_AccumulatesAmount()
        {
            ItemCollection store = new ItemCollection();
            store.Add(_item, 10);
            store.Add(_item, 5);
            Assert.That(store.GetAmount(_item), Is.EqualTo(15));
        }

        [TestCase(4, 6)]
        [TestCase(10, 0)]
        public void TrySpend_WhenAmountIsSufficient_DeductsAmount(int spendAmount, int expectedAmount)
        {
            ItemCollection store = new ItemCollection();
            store.Add(_item, 10);
            bool isSuccessful = store.TrySpend(_item, spendAmount);
            Assert.That(isSuccessful, Is.True);
            Assert.That(store.GetAmount(_item), Is.EqualTo(expectedAmount));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void TrySpend_WhenAmountIsNotPositive_RejectsRequest(int spendAmount)
        {
            ItemCollection store = new ItemCollection();
            store.Add(_item, 10);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => store.TrySpend(_item, spendAmount));
            Assert.That(store.GetAmount(_item), Is.EqualTo(10));
        }

        #endregion
    }
}
