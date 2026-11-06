using System;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Messaging;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class LeaveController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private LeaveButtonView _leaveButtonView;

        private IGameCommands _commands;
        private IGameStateReader _state;
        private IZoneReader _zones;
        private IEventBus _events;

        #endregion

        #region Lifecycle

        private void OnDestroy()
        {
            if (_events != null)
            {
                _events.Unsubscribe<GameStateChanged>(HandleStateChanged);
            }

            if (_leaveButtonView)
            {
                _leaveButtonView.OnLeaveClicked -= HandleLeaveClicked;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(
            IGameCommands commands,
            IGameStateReader state,
            IZoneReader zones,
            IEventBus events)
        {
            if (_events != null)
            {
                throw new InvalidOperationException("Leave controller is already initialized.");
            }

            if (!_leaveButtonView)
            {
                throw new InvalidOperationException($"{name}: Leave button view must be assigned.");
            }

            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _zones = zones ?? throw new ArgumentNullException(nameof(zones));
            _events = events ?? throw new ArgumentNullException(nameof(events));

            _leaveButtonView.OnLeaveClicked += HandleLeaveClicked;
            _events.Subscribe<GameStateChanged>(HandleStateChanged);

            Refresh();
        }

        #endregion

        #region Private Methods

        private void HandleLeaveClicked()
        {
            _commands.RequestLeave();
        }

        private void HandleStateChanged(GameStateChanged message)
        {
            Refresh();
        }

        private void Refresh()
        {
            bool isVisible =
                _state.CurrentState == GameState.Idle &&
                _zones.CanLeave;

            _leaveButtonView.SetVisible(isVisible);
        }

        #endregion
    }
}