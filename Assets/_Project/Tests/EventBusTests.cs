using NUnit.Framework;
using VertigoDemo.Messaging;

namespace VertigoDemo.Tests
{
    public class EventBusTests
    {
        #region Fields

        private IEventBus _bus;
        private int _firstCallCount;
        private int _secondCallCount;
        private int _otherCallCount;
        private int _receivedAmount;

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            _bus = new EventBus();
            _firstCallCount = 0;
            _secondCallCount = 0;
            _otherCallCount = 0;
            _receivedAmount = 0;
        }

        #endregion

        #region Tests

        [Test]
        public void Publish_OnlyMatchingSubscribersReceiveMessage()
        {
            _bus.Subscribe<TestRewardEvent>(HandleFirstReward);
            _bus.Subscribe<OtherEvent>(HandleOtherEvent);

            _bus.Publish(new TestRewardEvent(42));

            Assert.That(_firstCallCount, Is.EqualTo(1));
            Assert.That(_receivedAmount, Is.EqualTo(42));
            Assert.That(_otherCallCount, Is.Zero);
        }

        [Test]
        public void Publish_NotifiesAllSubscribersOfTheSameType()
        {
            _bus.Subscribe<TestRewardEvent>(HandleFirstReward);
            _bus.Subscribe<TestRewardEvent>(HandleSecondReward);

            _bus.Publish(new TestRewardEvent(10));

            Assert.That(_firstCallCount, Is.EqualTo(1));
            Assert.That(_secondCallCount, Is.EqualTo(1));
        }

        [Test]
        public void Unsubscribe_RemovesOnlyTheSpecifiedSubscriber()
        {
            _bus.Subscribe<TestRewardEvent>(HandleFirstReward);
            _bus.Subscribe<TestRewardEvent>(HandleSecondReward);

            _bus.Unsubscribe<TestRewardEvent>(HandleFirstReward);
            _bus.Publish(new TestRewardEvent(10));

            Assert.That(_firstCallCount, Is.Zero);
            Assert.That(_secondCallCount, Is.EqualTo(1));
        }

        [Test]
        public void Publish_WithoutSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(
                () => _bus.Publish(new TestRewardEvent(10)));
        }

        #endregion

        #region Handlers

        private void HandleFirstReward(TestRewardEvent message)
        {
            _firstCallCount++;
            _receivedAmount = message.Amount;
        }

        private void HandleSecondReward(TestRewardEvent message)
        {
            _secondCallCount++;
        }

        private void HandleOtherEvent(OtherEvent message)
        {
            _otherCallCount++;
        }

        #endregion

        #region Test Messages

        private readonly struct TestRewardEvent
        {
            public readonly int Amount;

            public TestRewardEvent(int amount)
            {
                Amount = amount;
            }
        }

        private readonly struct OtherEvent
        {
        }

        #endregion
    }
}