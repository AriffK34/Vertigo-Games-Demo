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

        private IGameFlowCallbacks _callbacks;
        private IEventBus _events;

        #endregion

        #region Lifecycle

        private void OnDestroy()
        {
            if (_events != null)
            {
                _events.Unsubscribe<RewardGranted>(HandleRewardGranted);
                _events.Unsubscribe<RunRewardsCleared>(HandleRewardsCleared);
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
            IGameFlowCallbacks callbacks,
            IEventBus events)
        {
            if (!_inventoryView || !_wheelView || !_rewardFlightView)
            {
                throw new InvalidOperationException($"{name}: Inventory presentation references must be assigned.");
            }

            if (rewards == null)
            {
                throw new ArgumentNullException(nameof(rewards));
            }

            _callbacks = callbacks ?? throw new ArgumentNullException(nameof(callbacks));
            _events = events ?? throw new ArgumentNullException(nameof(events));

            _events.Subscribe<RewardGranted>(HandleRewardGranted);
            _events.Subscribe<RunRewardsCleared>(HandleRewardsCleared);

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
            _callbacks.CompleteRewardPresentation();
        }

        #endregion
    }
}