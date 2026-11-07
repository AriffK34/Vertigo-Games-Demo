using System;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Messaging;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class BombPopupController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private BombPopupView _bombPopupView;
        private IGameCommands _commands;
        private IGameStateReader _state;
        private IEconomyReader _economy;
        private IGameFlowCallbacks _flowCallbacks;
        private IEventBus _eventBus;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GameStateChanged>(HandleStateChanged);
            }

            if (_bombPopupView)
            {
                _bombPopupView.OnGiveUpClicked -= HandleGiveUpClicked;
                _bombPopupView.OnReviveClicked -= HandleReviveClicked;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(
            IGameCommands commands,
            IGameStateReader state,
            IEconomyReader economy,
            IGameFlowCallbacks flowCallbacks,
            IEventBus eventBus)
        {
            if (_eventBus != null)
            {
                throw new InvalidOperationException("Bomb popup controller is already initialized.");
            }

            if (!_bombPopupView)
            {
                throw new InvalidOperationException($"{name}: Bomb popup view must be assigned.");
            }

            _bombPopupView.ValidateReferences();

            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _flowCallbacks = flowCallbacks ?? throw new ArgumentNullException(nameof(flowCallbacks));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _bombPopupView.OnGiveUpClicked += HandleGiveUpClicked;
            _bombPopupView.OnReviveClicked += HandleReviveClicked;
            _eventBus.Subscribe<GameStateChanged>(HandleStateChanged);

            _bombPopupView.gameObject.SetActive(false);
        }

        #endregion

        #region Private Methods

        private void HandleStateChanged(GameStateChanged message)
        {
            if (message.State == GameState.BombExploded)
            {
                _bombPopupView.Show(
                    _economy.HasReviveFunds,
                    _economy.ReviveCost,
                    _economy.ReviveCurrency.ItemIcon);
            }
            else if (message.State == GameState.Reviving)
            {
                _bombPopupView.Hide(HandleReviveComplete);
            }
        }

        private void HandleGiveUpClicked()
        {
            if (_state.CurrentState != GameState.BombExploded)
            {
                return;
            }

            _bombPopupView.Hide(_commands.GiveUp);
        }

        private void HandleReviveClicked()
        {
            _commands.TryRevive();
        }

        private void HandleReviveComplete()
        {
            _flowCallbacks.CompleteRevive();
        }

        #endregion
    }
}
