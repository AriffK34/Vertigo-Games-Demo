using System;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Data;
using VertigoDemo.Messaging;
using VertigoDemo.Presentation;

namespace VertigoDemo.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ZoneProgressionConfig _progressionConfig;
        [SerializeField] private EconomyConfig _economyConfig;
        [SerializeField] private RuntimeConfig _runtimeConfig;
        [SerializeField] private WheelController _wheelController;
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private ZoneTransitionController _zoneTransitionController;
        [SerializeField] private CurrencyBarController _currencyBarController;
        [SerializeField] private LeaveController _leaveController;
        [SerializeField] private ClaimPopupController _claimPopupController;
        [SerializeField] private BombPopupController _bombPopupController;

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            _wheelController = GetComponent<WheelController>();
            _inventoryController = GetComponent<InventoryController>();
            _zoneTransitionController = GetComponent<ZoneTransitionController>();
            _currencyBarController = GetComponent<CurrencyBarController>();
            _leaveController = GetComponent<LeaveController>();
            _claimPopupController = GetComponent<ClaimPopupController>();
            _bombPopupController = GetComponent<BombPopupController>();
        }

        private void Start()
        {
            ValidateConfiguration();
            Application.targetFrameRate = _runtimeConfig.TargetFrameRate;

            IEventBus events = new EventBus();

            ZoneProgression zones = new ZoneProgression(new ZoneRuleResolver(_progressionConfig.Rules));

            SessionInventory inventory = new SessionInventory();
            EconomyService economy = new EconomyService(_economyConfig);
            RewardCalculator calculator = new RewardCalculator();

            GameSession session = new GameSession(
                zones,
                inventory,
                economy,
                calculator,
                new UnityRandomSource(),
                events);

            _zoneTransitionController.Initialize(zones, session, events, calculator);
            _wheelController.Initialize(session, session, session, events);
            _inventoryController.Initialize(inventory, session, events);
            _currencyBarController.Initialize(economy, events);
            _leaveController.Initialize(session, session, zones, events);
            _claimPopupController.Initialize(session, session, inventory, events);
            _bombPopupController.Initialize(session, session, economy, events);

            session.StartGame();
        }

        #endregion

        #region Private Methods

        private void ValidateConfiguration()
        {
            if (!_progressionConfig || !_economyConfig || !_runtimeConfig)
            {
                throw new InvalidOperationException(
                    $"{name}: Progression, economy and runtime configs must be assigned.");
            }

            if (!_wheelController ||
                !_inventoryController ||
                !_zoneTransitionController ||
                !_currencyBarController ||
                !_leaveController ||
                !_claimPopupController ||
                !_bombPopupController)
            {
                throw new InvalidOperationException(
                    $"{name}: All presentation controllers must be on the bootstrap object.");
            }

            foreach (ZoneRuleConfig rule in _progressionConfig.Rules)
            {
                if (!rule)
                {
                    throw new InvalidOperationException($"{_progressionConfig.name}: Zone rules cannot be empty.");
                }

                rule.ValidateWheel();

                if (!rule.Wheel.WheelTheme)
                {
                    throw new InvalidOperationException($"{rule.Wheel.name}: Wheel theme must be assigned.");
                }

                foreach (WheelSliceData slice in rule.Wheel.WheelSlices)
                {
                    if (!slice.IsBomb && !slice.ItemData.GrowthProfile)
                    {
                        throw new InvalidOperationException($"{slice.ItemData.name}: Growth profile must be assigned.");
                    }
                }
            }
        }

        #endregion
    }
}
