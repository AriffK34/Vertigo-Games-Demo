using System;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Messaging;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class ClaimPopupController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ClaimPopupView _claimPopupView;

        private IGameCommands _commands;
        private IGameStateReader _state;
        private IRunRewardsReader _rewards;
        private IEventBus _events;

        #endregion

        #region Lifecycle

        private void OnDestroy()
        {
            if (_events != null)
            {
                _events.Unsubscribe<GameStateChanged>(HandleStateChanged);
            }

            if (_claimPopupView)
            {
                _claimPopupView.OnClaimClicked -= HandleClaimClicked;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(
            IGameCommands commands,
            IGameStateReader state,
            IRunRewardsReader rewards,
            IEventBus events)
        {
            if (_events != null)
            {
                throw new InvalidOperationException("Claim popup controller is already initialized.");
            }

            if (!_claimPopupView)
            {
                throw new InvalidOperationException($"{name}: Claim popup view must be assigned.");
            }

            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            
            _claimPopupView.OnClaimClicked += HandleClaimClicked;
            _events.Subscribe<GameStateChanged>(HandleStateChanged);

            _claimPopupView.gameObject.SetActive(false);
        }

        #endregion

        #region Private Methods

        private void HandleStateChanged(GameStateChanged message)
        {
            if (message.State == GameState.RewardsClaiming)
            {
                _claimPopupView.Show(_rewards.Inventory);
            }
        }

        private void HandleClaimClicked()
        {
            if (_state.CurrentState != GameState.RewardsClaiming)
            {
                return;
            }

            _claimPopupView.Hide();
            _commands.ClaimRewards();
        }

        #endregion
    }
}