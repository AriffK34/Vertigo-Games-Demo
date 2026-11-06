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
        private IEventBus _events;

        #endregion

        #region Lifecycle

        private void OnDestroy()
        {
            if (_events != null)
            {
                _events.Unsubscribe<GameStateChanged>(HandleStateChanged);
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
            IEventBus events)
        {
            if (_events != null)
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
            _events = events ?? throw new ArgumentNullException(nameof(events));

            _bombPopupView.OnGiveUpClicked += HandleGiveUpClicked;
            _bombPopupView.OnReviveClicked += HandleReviveClicked;
            _events.Subscribe<GameStateChanged>(HandleStateChanged);

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
                    _economy.ReviveCost);
            }
        }

        private void HandleGiveUpClicked()
        {
            if (_state.CurrentState != GameState.BombExploded)
            {
                return;
            }

            _bombPopupView.Hide();
            _commands.GiveUp();
        }

        private void HandleReviveClicked()
        {
            if (_commands.TryRevive())
            {
                _bombPopupView.Hide();
            }
        }

        #endregion
    }
}