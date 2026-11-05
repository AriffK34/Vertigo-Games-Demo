using System;
using UnityEngine;
using UnityEngine.Serialization;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class InventoryController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private RewardFlightView _rewardFlightView;
        
        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!_gameManager) _gameManager = GetComponent<GameManager>();
        }

        private void Start()
        {
            inventoryView.OnRewardSlotReady += HandleRewardSlotReady;
            inventoryView.Initialize(_gameManager.Inventory);
        }

        private void OnDestroy()
        {
            if (inventoryView)
            {
                inventoryView.OnRewardSlotReady -= HandleRewardSlotReady;
                inventoryView.CompleteRewardPresentation();
            }

            if (_rewardFlightView)
            {
                _rewardFlightView.Cancel();
            }
        }

        #endregion

        #region Private Methods

        private void HandleRewardSlotReady(InventorySlotView slotView)
        {
            _rewardFlightView.Play(
                slotView.Item.ItemIcon,
                _wheelView.WinningIconPosition,
                slotView.IconPosition,
                HandleRewardFlightComplete);
        }

        private void HandleRewardFlightComplete()
        {
            inventoryView.CompleteRewardPresentation();
            _gameManager.CompleteRewardPresentation();
        }

        #endregion
    }
}

