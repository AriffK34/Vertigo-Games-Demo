using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Data;
using VertigoDemo.Messaging;

namespace VertigoDemo.Tests
{
    public class GameSessionTests
    {
        #region Fields

        private readonly List<ScriptableObject> _assets = new List<ScriptableObject>();
        private ItemData _reward;
        private ZoneRuleConfig _rule;
        private SessionInventory _inventory;
        private EconomyService _economy;
        private FixedRandomSource _random;
        private GameSession _session;
        private IEventBus _bus;
        private ZoneTransitionRequested _transition;
        private SpinResult _spin;
        private int _transitionCount;
        private int _rewardCount;

        #endregion

        #region Setup and Teardown

        [SetUp]
        public void SetUp()
        {
            _transition = default;
            _spin = default;
            _transitionCount = 0;
            _rewardCount = 0;

            _reward = CreateAsset<ItemData>();
            ItemData bomb = CreateAsset<ItemData>();
            RewardGrowthConfig growth = CreateAsset<RewardGrowthConfig>();

            SerializedObject serializedGrowth = new SerializedObject(growth);
            serializedGrowth.FindProperty("_growthPerZone").floatValue = 0f;
            serializedGrowth.ApplyModifiedPropertiesWithoutUndo();

            SetReference(_reward, "_growthProfile", growth);

            WheelConfig wheel = CreateAsset<WheelConfig>();
            SerializedObject serializedWheel = new SerializedObject(wheel);
            SerializedProperty slices = serializedWheel.FindProperty("_wheelSlices");
            slices.arraySize = 8;

            for (int i = 0; i < slices.arraySize; i++)
            {
                bool isBomb = i == 1;
                SerializedProperty slice = slices.GetArrayElementAtIndex(i);
                slice.FindPropertyRelative("_itemData").objectReferenceValue = isBomb ? bomb : _reward;
                slice.FindPropertyRelative("_itemAmount").intValue = isBomb ? 0 : 10;
                slice.FindPropertyRelative("_isBomb").boolValue = isBomb;
            }

            serializedWheel.ApplyModifiedPropertiesWithoutUndo();

            _rule = CreateAsset<ZoneRuleConfig>();
            SerializedObject serializedRule = new SerializedObject(_rule);
            serializedRule.FindProperty("_wheel").objectReferenceValue = wheel;
            serializedRule.FindProperty("_isLeaveAllowed").boolValue = true;
            serializedRule.ApplyModifiedPropertiesWithoutUndo();

            EconomyConfig economyConfig = CreateAsset<EconomyConfig>();
            SetReference(economyConfig, "_reviveCurrency", _reward);
            SetInt(economyConfig, "_reviveCost", 5);

            ZoneProgression zones = new ZoneProgression(new ZoneRuleResolver(new[] { _rule }));
            _inventory = new SessionInventory();
            _economy = new EconomyService(economyConfig);
            _random = new FixedRandomSource();
            _bus = new EventBus();

            _bus.Subscribe<ZoneTransitionRequested>(HandleTransitionRequested);
            _bus.Subscribe<SpinStarted>(HandleSpinStarted);
            _bus.Subscribe<RewardGranted>(HandleRewardGranted);

            _session = new GameSession(
                zones,
                _inventory,
                _economy,
                new RewardCalculator(),
                _random,
                _bus);
        }

        [TearDown]
        public void TearDown()
        {
            _bus.Unsubscribe<ZoneTransitionRequested>(HandleTransitionRequested);
            _bus.Unsubscribe<SpinStarted>(HandleSpinStarted);
            _bus.Unsubscribe<RewardGranted>(HandleRewardGranted);

            foreach (ScriptableObject asset in _assets)
            {
                Object.DestroyImmediate(asset);
            }

            _assets.Clear();
        }

        #endregion

        #region Tests

        [Test]
        public void BeforeStartup_CompletionDoesNotUnlockSpin()
        {
            _session.CompleteZoneTransition();

            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Resetting));
            Assert.That(_session.TrySpin(), Is.False);
        }

        [Test]
        public void StartGame_WaitsForInitialTransitionAndStartsOnlyOnce()
        {
            _session.StartGame();
            _session.StartGame();

            Assert.That(_transitionCount, Is.EqualTo(1));
            Assert.That(_transition.Zone, Is.EqualTo(1));
            Assert.That(_transition.IsReset, Is.True);
            Assert.That(_session.TrySpin(), Is.False);

            _session.CompleteZoneTransition();

            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Idle));
            Assert.That(_session.TrySpin(), Is.True);

            _session.StartGame();

            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Spinning));
            Assert.That(_transitionCount, Is.EqualTo(1));
        }

        [Test]
        public void RewardFlow_GrantsOnceAndWaitsForPresentationAndTransition()
        {
            StartReadySession();
            Assert.That(_session.TrySpin(), Is.True);
            Assert.That(_spin.SliceIndex, Is.Zero);
            Assert.That(_spin.Item, Is.SameAs(_reward));
            Assert.That(_spin.Amount, Is.EqualTo(10));
            Assert.That(_session.TrySpin(), Is.False);

            _session.CompleteSpin();
            _session.CompleteSpin();

            Assert.That(_inventory.Inventory[_reward], Is.EqualTo(10));
            Assert.That(_rewardCount, Is.EqualTo(1));
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.RewardGiving));
            Assert.That(_session.TrySpin(), Is.False);

            _session.CompleteZoneTransition();
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.RewardGiving));

            _session.CompleteRewardPresentation();
            _session.CompleteRewardPresentation();

            Assert.That(_transition.Zone, Is.EqualTo(2));
            Assert.That(_transition.IsReset, Is.False);
            Assert.That(_transitionCount, Is.EqualTo(2));
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.ZoneTransition));
            Assert.That(_session.TrySpin(), Is.False);

            _session.CompleteZoneTransition();
            _session.CompleteZoneTransition();

            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Idle));
        }

        [Test]
        public void RequestLeave_RespectsConfiguredPermissionAndState()
        {
            SerializedObject serializedRule = new SerializedObject(_rule);
            serializedRule.FindProperty("_isLeaveAllowed").boolValue = false;
            serializedRule.ApplyModifiedPropertiesWithoutUndo();

            StartReadySession();
            _session.RequestLeave();
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Idle));

            serializedRule.FindProperty("_isLeaveAllowed").boolValue = true;
            serializedRule.ApplyModifiedPropertiesWithoutUndo();

            Assert.That(_session.TrySpin(), Is.True);
            _session.RequestLeave();
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Spinning));
        }

        [Test]
        public void ClaimRewards_TransfersOnceAndWaitsForReset()
        {
            StartReadySession();
            CompleteRewardSpin();
            _session.CompleteZoneTransition();
            _session.RequestLeave();

            Assert.That(_session.CurrentState, Is.EqualTo(GameState.RewardsClaiming));

            _session.ClaimRewards();
            _session.ClaimRewards();

            Assert.That(_economy.GetBalance(_reward), Is.EqualTo(10));
            Assert.That(_inventory.Inventory, Is.Empty);
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Resetting));
            Assert.That(_transition.Zone, Is.EqualTo(1));
            Assert.That(_transition.IsReset, Is.True);
            Assert.That(_transitionCount, Is.EqualTo(3));
            Assert.That(_session.TrySpin(), Is.False);

            _session.CompleteZoneTransition();
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Idle));
        }

        [Test]
        public void GiveUp_ClearsRunRewardsAndWaitsForReset()
        {
            StartReadySession();
            CompleteRewardSpin();
            _session.CompleteZoneTransition();

            _random.Index = 1;
            Assert.That(_session.TrySpin(), Is.True);
            _session.CompleteSpin();

            Assert.That(_session.CurrentState, Is.EqualTo(GameState.BombExploded));
            Assert.That(_rewardCount, Is.EqualTo(1));

            _session.GiveUp();

            Assert.That(_inventory.Inventory, Is.Empty);
            Assert.That(_economy.GetBalance(_reward), Is.Zero);
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Resetting));
            Assert.That(_transition.Zone, Is.EqualTo(1));
            Assert.That(_session.TrySpin(), Is.False);
        }

        [TestCase(0, false)]
        [TestCase(5, true)]
        public void TryRevive_RequiresFundsAndPreservesRunRewards(int initialBalance, bool expectedSuccess)
        {
            if (initialBalance > 0)
            {
                SessionInventory seedInventory = new SessionInventory();
                seedInventory.AddItem(_reward, initialBalance);
                _economy.ClaimRewards(seedInventory);
            }

            StartReadySession();
            _inventory.AddItem(_reward, 3);
            _random.Index = 1;
            Assert.That(_session.TrySpin(), Is.True);
            _session.CompleteSpin();

            bool result = _session.TryRevive();

            Assert.That(result, Is.EqualTo(expectedSuccess));
            Assert.That(_inventory.Inventory[_reward], Is.EqualTo(3));
            Assert.That(_economy.GetBalance(_reward), Is.Zero);

            GameState expectedState = expectedSuccess ? GameState.Idle : GameState.BombExploded;
            Assert.That(_session.CurrentState, Is.EqualTo(expectedState));
        }

        #endregion

        #region Private Methods

        private void StartReadySession()
        {
            _session.StartGame();
            _session.CompleteZoneTransition();
            Assert.That(_session.CurrentState, Is.EqualTo(GameState.Idle));
        }

        private void CompleteRewardSpin()
        {
            _random.Index = 0;
            Assert.That(_session.TrySpin(), Is.True);
            _session.CompleteSpin();
            _session.CompleteRewardPresentation();
        }

        private void HandleTransitionRequested(ZoneTransitionRequested message)
        {
            _transition = message;
            _transitionCount++;
        }

        private void HandleSpinStarted(SpinStarted message)
        {
            _spin = message.Result;
        }

        private void HandleRewardGranted(RewardGranted message)
        {
            _rewardCount++;
        }

        private T CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            _assets.Add(asset);
            return asset;
        }

        private static void SetReference(Object target, string property, Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            serialized.FindProperty(property).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInt(Object target, string property, int value)
        {
            SerializedObject serialized = new SerializedObject(target);
            serialized.FindProperty(property).intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Nested Types

        private sealed class FixedRandomSource : IRandomSource
        {
            #region Fields

            public int Index;

            #endregion

            #region Public Methods

            public int Next(int maxExclusive)
            {
                return Index;
            }

            #endregion
        }

        #endregion
    }
}
