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
        private IEventBus _eventBus;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GameStateChanged>(HandleStateChanged);
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
            IEventBus eventBus)
        {
            if (_eventBus != null)
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
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _leaveButtonView.OnLeaveClicked += HandleLeaveClicked;
            _eventBus.Subscribe<GameStateChanged>(HandleStateChanged);

            RefreshVisibility();
        }

        #endregion

        #region Private Methods

        private void HandleLeaveClicked()
        {
            _commands.RequestLeave();
        }

        private void HandleStateChanged(GameStateChanged message)
        {
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            bool isVisible =
                _state.CurrentState == GameState.Idle &&
                _zones.IsLeaveAllowed;

            _leaveButtonView.SetVisible(isVisible);
        }

        #endregion
    }
}
