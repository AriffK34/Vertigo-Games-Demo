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
        
        public SessionInventory Inventory { get; private set; }
        public EconomyService Economy { get; private set; }
        public GameState CurrentState { get; private set; }
        
        private void OnValidate()
        {
            if (zoneManager == null) zoneManager = GetComponent<ZoneManager>();
            if (wheelController == null) wheelController = GetComponent<WheelController>();
        }

        private void Awake()
        {
            Inventory = new SessionInventory();
            Economy = new EconomyService(_economyConfig);
            SetState(GameState.Idle);
        }

        private void Start()
        {
            wheelController.OnSpinStarted += HandleSpinStarted;
            wheelController.OnSpinLanded += HandleSpinLanded;
        }

        private void OnDestroy()
        {
            wheelController.OnSpinStarted -= HandleSpinStarted;
            wheelController.OnSpinLanded -= HandleSpinLanded;
        }

        private void HandleSpinStarted()
        {
            SetState(GameState.Spinning);
        }

        private void HandleSpinLanded(WheelSliceData sliceData)
        {
            if (sliceData.IsBomb)
            {
                SetState(GameState.BombExploded);
            }
            else
            {
                SetState(GameState.RewardGiving);
                int rewardAmount = zoneManager.CalculateRewardAmount(sliceData.ItemData, sliceData.ItemAmount);
                Inventory.AddItem(sliceData.ItemData, rewardAmount);
                DOVirtual.DelayedCall(0.8f, () =>
                {
                    SetState(GameState.ZoneTransition);
                    zoneManager.AdvanceZone();
                });
            }
        }
        
        public void CompleteZoneTransition()
        {
            if (CurrentState != GameState.ZoneTransition) return;
            SetState(GameState.Idle);
        }

        public void RequestLeave()
        {
            if (CurrentState != GameState.Idle) return;
            if (!zoneManager.IsSafeZone && !zoneManager.IsSuperZone) return;
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
            zoneManager.ResetZone();
            SetState(GameState.Idle);
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke();
        }
    }   
}
