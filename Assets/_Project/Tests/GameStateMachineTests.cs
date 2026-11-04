using NUnit.Framework;
using VertigoDemo.Core;

namespace VertigoDemo.Tests
{
    public class GameStateMachineTests
    {
        #region Fields

        private GameStateMachine _machine;

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            _machine = new GameStateMachine();
        }

        #endregion

        #region Tests

        [Test]
        public void InitialReset_RejectSpin()
        {
            bool isSuccessful = _machine.TryTransition(GameState.Spinning);
            Assert.That(isSuccessful, Is.False);
            Assert.That(_machine.CurrentState, Is.EqualTo(GameState.Resetting));
        }

        [TestCase(GameState.Spinning)]
        [TestCase(GameState.RewardsClaiming)]
        public void WhileSpinning_RejectsInvalidActions(GameState requestedState)
        {
            MoveThrough(GameState.Idle, GameState.Spinning);
            bool isSuccessful = _machine.TryTransition(requestedState);
            Assert.That(isSuccessful, Is.False);
            Assert.That(_machine.CurrentState, Is.EqualTo(GameState.Spinning));
        }

        [Test]
        public void RewardFlow_WaitForZoneTransitionBeforeReturningToIdle()
        {
            MoveThrough(
                GameState.Idle,
                GameState.Spinning,
                GameState.RewardGiving,
                GameState.ZoneTransition);
            
            Assert.That(_machine.CurrentState, Is.EqualTo(GameState.ZoneTransition));
            MoveThrough(GameState.Idle);
            Assert.That(_machine.CurrentState, Is.EqualTo(GameState.Idle));
        }

        [TestCase(GameState.Idle)]
        [TestCase(GameState.Resetting)]
        public void BombFlow_AllowReviveOrRestart(GameState nextState)
        {
            MoveThrough(
                GameState.Idle,
                GameState.Spinning,
                GameState.BombExploded);

            bool isSuccessful = _machine.TryTransition(nextState);
            Assert.That(isSuccessful, Is.True);
            Assert.That(_machine.CurrentState, Is.EqualTo(nextState));
        }

        [Test]
        public void ClaimFlow_RequiresResetCompletionBeforeNextSpin()
        {
            MoveThrough(
                GameState.Idle,
                GameState.RewardsClaiming,
                GameState.Resetting);
            
            Assert.That(_machine.TryTransition(GameState.Spinning), Is.False);
            Assert.That(_machine.CurrentState, Is.EqualTo(GameState.Resetting));
            MoveThrough(GameState.Idle, GameState.Spinning);
            Assert.That(_machine.CurrentState, Is.EqualTo(GameState.Spinning));
        }

        #endregion

        #region Helpers

        private void MoveThrough(params GameState[] states)
        {
            foreach (var state in states)
            {
                Assert.That(
                    _machine.TryTransition(state), Is.True, $"Could not enter state {state}.");
            }
        }

        #endregion
    }
}