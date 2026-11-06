using System;
using UnityEngine;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Messaging;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class InventoryController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private RewardFlightView _rewardFlightView;
        private IGameFlowCallbacks _flowCallbacks;
        private IEventBus _eventBus;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<RewardGranted>(HandleRewardGranted);
                _eventBus.Unsubscribe<RunRewardsCleared>(HandleRewardsCleared);
            }

            if (_rewardFlightView)
            {
                _rewardFlightView.Cancel();
            }

            if (_inventoryView)
            {
                _inventoryView.CompleteRewardPresentation();
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(
            IRunRewardsReader rewards,
            IGameFlowCallbacks flowCallbacks,
            IEventBus eventBus)
        {
            if (!_inventoryView || !_wheelView || !_rewardFlightView)
            {
                throw new InvalidOperationException($"{name}: Inventory presentation references must be assigned.");
            }

            if (rewards == null)
            {
                throw new ArgumentNullException(nameof(rewards));
            }

            _flowCallbacks = flowCallbacks ?? throw new ArgumentNullException(nameof(flowCallbacks));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _eventBus.Subscribe<RewardGranted>(HandleRewardGranted);
            _eventBus.Subscribe<RunRewardsCleared>(HandleRewardsCleared);

            _inventoryView.Refresh(rewards.Inventory);
        }

        #endregion

        #region Private Methods

        private void HandleRewardGranted(RewardGranted message)
        {
            InventorySlotView slot = _inventoryView.SetItem(
                message.Result.Item,
                message.TotalAmount);

            _inventoryView.RevealSlot(slot);

            _rewardFlightView.Play(
                message.Result.Item.ItemIcon,
                _wheelView.WinningIconPosition,
                slot.IconPosition,
                HandleFlightComplete);
        }

        private void HandleRewardsCleared(RunRewardsCleared message)
        {
            _rewardFlightView.Cancel();
            _inventoryView.Clear();
        }

        private void HandleFlightComplete()
        {
            _inventoryView.CompleteRewardPresentation();
            _flowCallbacks.CompleteRewardPresentation();
        }

        #endregion
    }
}
