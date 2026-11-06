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

        [SerializeField] private BombPopupView bombPopupView;

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

            if (bombPopupView)
            {
                bombPopupView.OnGiveUpClicked -= HandleGiveUpClicked;
                bombPopupView.OnReviveClicked -= HandleReviveClicked;
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
                throw new InvalidOperationException(
                    "Bomb popup controller is already initialized.");
            }

            if (!bombPopupView)
            {
                throw new InvalidOperationException(
                    $"{name}: Bomb popup view must be assigned.");
            }

            _commands = commands
                ?? throw new ArgumentNullException(nameof(commands));

            _state = state
                ?? throw new ArgumentNullException(nameof(state));

            _economy = economy
                ?? throw new ArgumentNullException(nameof(economy));

            _events = events
                ?? throw new ArgumentNullException(nameof(events));

            bombPopupView.OnGiveUpClicked += HandleGiveUpClicked;
            bombPopupView.OnReviveClicked += HandleReviveClicked;
            _events.Subscribe<GameStateChanged>(HandleStateChanged);

            bombPopupView.gameObject.SetActive(false);
        }

        #endregion

        #region Private Methods

        private void HandleStateChanged(GameStateChanged message)
        {
            if (message.State == GameState.BombExploded)
            {
                bombPopupView.Show(
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

            bombPopupView.Hide();
            _commands.GiveUp();
        }

        private void HandleReviveClicked()
        {
            if (_commands.TryRevive())
            {
                bombPopupView.Hide();
            }
        }

        #endregion
    }
}