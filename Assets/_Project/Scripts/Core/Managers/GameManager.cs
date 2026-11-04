using System;
using DG.Tweening;
using UnityEngine;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public class GameManager : MonoBehaviour
    {
        public event Action OnGameStateChanged;
        
        [SerializeField] private ZoneManager zoneManager;
        [SerializeField] private WheelController wheelController;
        [SerializeField] private EconomyConfig _economyConfig;

        private readonly GameStateMachine _stateMachine = new GameStateMachine();
        private Tween _rewardDelayTween;
        
        public SessionInventory Inventory { get; private set; }
        public EconomyService Economy { get; private set; }

        public GameState CurrentState
        {
            get
            {
                return _stateMachine.CurrentState;
            }
        }
        
        private void OnValidate()
        {
            if (zoneManager == null) zoneManager = GetComponent<ZoneManager>();
            if (wheelController == null) wheelController = GetComponent<WheelController>();
        }

        private void Awake()
        {
            Inventory = new SessionInventory();
            Economy = new EconomyService(_economyConfig);
        }

        private void Start()
        {
            wheelController.OnSpinStarted += HandleSpinStarted;
            wheelController.OnSpinLanded += HandleSpinLanded;
        }

        private void OnDestroy()
        {
            _rewardDelayTween?.Kill();
            if (wheelController)
            {
                wheelController.OnSpinStarted -= HandleSpinStarted;
                wheelController.OnSpinLanded -= HandleSpinLanded;
            }
        }

        private void HandleSpinStarted()
        {
            SetState(GameState.Spinning);
        }

        private void HandleSpinLanded(WheelSliceData sliceData)
        {
            if (CurrentState != GameState.Spinning)
            {
                return;
            }

            if (sliceData.IsBomb)
            {
                SetState(GameState.BombExploded);
                return;
            }
            
            SetState(GameState.RewardGiving);
            int rewardAmount = zoneManager.CalculateRewardAmount(sliceData.ItemData, sliceData.ItemAmount);
            Inventory.AddItem(sliceData.ItemData, rewardAmount);
            _rewardDelayTween?.Kill();
            _rewardDelayTween = DOVirtual.DelayedCall(
                    0.8f,
                    AdvanceAfterReward)
                .SetLink(gameObject);
        }

        private void AdvanceAfterReward()
        {
            _rewardDelayTween = null;
            if (CurrentState != GameState.RewardGiving)
            {
                return;
            }
            SetState(GameState.ZoneTransition);
            zoneManager.AdvanceZone();
        }
        
        public void CompleteZoneTransition()
        {
            if (CurrentState != GameState.ZoneTransition &&
                CurrentState != GameState.Resetting) return;
            SetState(GameState.Idle);
        }

        public void RequestLeave()
        {
            if (CurrentState != GameState.Idle)
            {
                return;
            }

            if (!zoneManager.CanLeave)
            {
                return;
            }
            SetState(GameState.RewardsClaiming);
        }

        public void ClaimRewards()
        {
            if (CurrentState != GameState.RewardsClaiming) return;
            Economy.ClaimRewards(Inventory);
            RestartGame();
        }

        public void GiveUp()
        {
            if (CurrentState != GameState.BombExploded) return;
            Inventory.Clear();
            RestartGame();
        }

        public bool TryRevive()
        {
            if (CurrentState != GameState.BombExploded) return false;
            if (!Economy.TryRevive()) return false;
            SetState(GameState.Idle);
            return true;
        }

        private void RestartGame()
        {
            _rewardDelayTween?.Kill();
            _rewardDelayTween = null;
            SetState(GameState.Resetting);
            zoneManager.ResetZone();
        }

        private void SetState(GameState nextState)
        {
            if (!_stateMachine.TryTransition(nextState))
            {
                throw new InvalidOperationException(
                    $"Invalid game state transition: {CurrentState} -> {nextState}");
            }
            OnGameStateChanged?.Invoke();
        }
    }   
}
