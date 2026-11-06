using System;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Messaging;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class WheelController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private WheelView _wheelView;

        private IGameCommands _commands;
        private IGameFlowCallbacks _callbacks;
        private IEventBus _events;

        #endregion

        #region Lifecycle

        private void OnDestroy()
        {
            if (_events != null)
            {
                _events.Unsubscribe<SpinStarted>(HandleSpinStarted);
                _events.Unsubscribe<GameStateChanged>(HandleStateChanged);
            }

            if (_wheelView)
            {
                _wheelView.OnSpinClicked -= HandleSpinClicked;
                _wheelView.CancelRotation();
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(
            IGameCommands commands,
            IGameStateReader state,
            IGameFlowCallbacks callbacks,
            IEventBus events)
        {
            if (!_wheelView)
            {
                throw new InvalidOperationException($"{name}: Wheel view must be assigned.");
            }

            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _callbacks = callbacks ?? throw new ArgumentNullException(nameof(callbacks));
            _events = events ?? throw new ArgumentNullException(nameof(events));

            _wheelView.OnSpinClicked += HandleSpinClicked;
            _events.Subscribe<SpinStarted>(HandleSpinStarted);
            _events.Subscribe<GameStateChanged>(HandleStateChanged);

            _wheelView.SetSpinInteractable(
                state.CurrentState == GameState.Idle);
        }

        #endregion

        #region Private Methods

        private void HandleSpinClicked()
        {
            _commands.TrySpin();
        }

        private void HandleSpinStarted(SpinStarted message)
        {
            _wheelView.RotateWheel(
                message.Result.SliceIndex,
                _callbacks.CompleteSpin);
        }

        private void HandleStateChanged(GameStateChanged message)
        {
            _wheelView.SetSpinInteractable(
                message.State == GameState.Idle);
        }

        #endregion
    }
}