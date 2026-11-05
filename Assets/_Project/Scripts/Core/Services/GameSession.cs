using System;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Data;
using VertigoDemo.Messaging;

namespace VertigoDemo.Core
{
    public sealed class GameSession : IGameCommands, IGameStateReader, IGameFlowCallbacks
    {
        #region Fields

        private readonly ZoneProgression _zones;
        private readonly SessionInventory _inventory;
        private readonly EconomyService _economy;
        private readonly RewardCalculator _rewardCalculator;
        private readonly IRandomSource _random;
        private readonly IEventBus _eventBus;

        private readonly GameStateMachine _stateMachine = new GameStateMachine();

        private SpinResult _pendingSpin;
        private bool _isStarted;

        #endregion

        #region Properties

        public GameState CurrentState
        {
            get
            {
                return _stateMachine.CurrentState;
            }
        }

        #endregion

        #region Constructor

        public GameSession(
            ZoneProgression zones,
            SessionInventory inventory,
            EconomyService economy,
            RewardCalculator rewardCalculator,
            IRandomSource random,
            IEventBus eventBus)
        {
            _zones = zones ?? throw new ArgumentNullException(nameof(zones));
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _rewardCalculator = rewardCalculator ?? throw new ArgumentNullException(nameof(rewardCalculator));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        #endregion

        #region Startup

        public void StartGame()
        {
            if (_isStarted)
            {
                return;
            }
            
            _zones.ResetZone();
            _isStarted = true;

            PublishStateChanged();
            PublishZoneTransition(true);
        }

        #endregion

        #region Player Commands

        public bool TrySpin()
        {
            if (CurrentState != GameState.Idle)
            {
                return false;
            }

            WheelConfig wheel = _zones.CurrentRule.Wheel;
            int sliceIndex = _random.Next(wheel.WheelSlices.Length);

            if (sliceIndex < 0 || sliceIndex >= wheel.WheelSlices.Length)
            {
                throw new InvalidOperationException("Random source returned an invalid slice index."); 
            }

            WheelSliceData slice = wheel.WheelSlices[sliceIndex];
            int amount = slice.IsBomb
                ? 0
                : _rewardCalculator.Calculate(slice.ItemData, slice.ItemAmount, _zones.CurrentZone);

            _pendingSpin = new SpinResult(sliceIndex, slice.ItemData, amount, slice.IsBomb);

            SetState(GameState.Spinning);
            _eventBus.Publish(new SpinStarted(_pendingSpin));
            
            return true;
        }

        public void RequestLeave()
        {
            if (CurrentState != GameState.Idle || !_zones.CanLeave)
            {
                return;
            }

            SetState(GameState.RewardsClaiming);
        }

        public void ClaimRewards()
        {
            if (CurrentState != GameState.RewardsClaiming)
            {
                return;
            }

            _economy.ClaimRewards(_inventory);
            RestartRun(true);
        }

        public void GiveUp()
        {
            if (CurrentState != GameState.BombExploded)
            {
                return;
            }
            
            _inventory.Clear();
            RestartRun(false);
        }

        public bool TryRevive()
        {
            if (CurrentState != GameState.BombExploded)
            {
                return false;
            }

            if (!_economy.TryRevive())
            {
                return false;
            }

            SetState(GameState.Idle);
            _eventBus.Publish(new BalancesChanged());

            return true;
        }

        #endregion

        #region Presentation Callbacks

        public void CompleteSpin()
        {
            if (CurrentState != GameState.Spinning)
            {
                return;
            }

            if (_pendingSpin.IsBomb)
            {
                SetState(GameState.BombExploded);
                return;
            }

            _inventory.Inventory.TryGetValue(_pendingSpin.Item, out int currentAmount);
            _ = checked(currentAmount + _pendingSpin.Amount);

            SetState(GameState.RewardGiving);
            _inventory.AddItem(_pendingSpin.Item, _pendingSpin.Amount);

            int totalAmount = _inventory.Inventory[_pendingSpin.Item];
            _eventBus.Publish(new RewardGranted(_pendingSpin, totalAmount));
        }

        public void CompleteRewardPresentation()
        {
            if (CurrentState != GameState.RewardGiving)
            {
                return;
            }
            
            _zones.AdvanceZone();

            SetState(GameState.ZoneTransition);
            PublishZoneTransition(false);
        }

        public void CompleteZoneTransition()
        {
            if (!_isStarted)
            {
                return;
            }

            if (CurrentState != GameState.Resetting && CurrentState != GameState.ZoneTransition)
            {
                return;
            }

            SetState(GameState.Idle);
        }

        #endregion

        #region PrivateMethots

        private void RestartRun(bool hasBalanceChange)
        {
            _zones.ResetZone();

            SetState(GameState.Resetting);

            if (hasBalanceChange)
            {
                _eventBus.Publish(new BalancesChanged());
            }
            
            _eventBus.Publish(new RunRewardsCleared());
            PublishZoneTransition(true);
        }

        private void SetState(GameState nextState)
        {
            if (!_stateMachine.TryTransition(nextState))
            {
                throw new InvalidOperationException($"Invalid game state transition: {CurrentState} -> {nextState}");
            }

            PublishStateChanged();
        }

        private void PublishStateChanged()
        {
            _eventBus.Publish(new GameStateChanged(CurrentState));
        }

        private void PublishZoneTransition(bool isReset)
        {
            _eventBus.Publish(new ZoneTransitionRequested(_zones.CurrentZone, isReset));
        }

        #endregion
    }
}
