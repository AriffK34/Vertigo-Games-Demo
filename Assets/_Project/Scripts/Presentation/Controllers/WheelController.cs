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
        private IGameFlowCallbacks _flowCallbacks;
        private IEventBus _eventBus;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<SpinStarted>(HandleSpinStarted);
                _eventBus.Unsubscribe<GameStateChanged>(HandleStateChanged);
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
            IGameFlowCallbacks flowCallbacks,
            IEventBus eventBus)
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
            _flowCallbacks = flowCallbacks ?? throw new ArgumentNullException(nameof(flowCallbacks));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _wheelView.OnSpinClicked += HandleSpinClicked;
            _eventBus.Subscribe<SpinStarted>(HandleSpinStarted);
            _eventBus.Subscribe<GameStateChanged>(HandleStateChanged);

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
                _flowCallbacks.CompleteSpin);
        }

        private void HandleStateChanged(GameStateChanged message)
        {
            _wheelView.SetSpinInteractable(
                message.State == GameState.Idle);
        }

        #endregion
    }
}
