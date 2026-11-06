using System;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Messaging;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class PlayerInventoryController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InventoryButtonView _openButtonView;
        [SerializeField] private PlayerInventoryPopupView _popupView;
        private IGameCommands _commands;
        private IGameStateReader _state;
        private IGameFlowCallbacks _flowCallbacks;
        private IPlayerInventoryReader _inventory;
        private IEventBus _eventBus;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GameStateChanged>(HandleStateChanged);
            }

            if (_openButtonView)
            {
                _openButtonView.OnClicked -= HandleOpenClicked;
            }

            if (_popupView)
            {
                _popupView.OnCloseClicked -= HandleCloseClicked;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(
            IGameCommands commands,
            IGameStateReader state,
            IGameFlowCallbacks flowCallbacks,
            IPlayerInventoryReader inventory,
            IEventBus eventBus)
        {
            if (_eventBus != null)
            {
                throw new InvalidOperationException("Player inventory controller is already initialized.");
            }

            if (!_openButtonView || !_popupView)
            {
                throw new InvalidOperationException($"{name}: Player inventory views must be assigned.");
            }

            _popupView.ValidateReferences();
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _flowCallbacks = flowCallbacks ?? throw new ArgumentNullException(nameof(flowCallbacks));
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _openButtonView.OnClicked += HandleOpenClicked;
            _popupView.OnCloseClicked += HandleCloseClicked;
            _eventBus.Subscribe<GameStateChanged>(HandleStateChanged);
            _popupView.gameObject.SetActive(false);
            _openButtonView.SetInteractable(_state.CurrentState == GameState.Idle);
        }

        #endregion

        #region Private Methods

        private void HandleStateChanged(GameStateChanged message)
        {
            _openButtonView.SetInteractable(message.State == GameState.Idle);

            if (message.State == GameState.InventoryViewing)
            {
                _popupView.Show(_inventory.Items);
            }
        }

        private void HandleOpenClicked()
        {
            _commands.TryOpenInventory();
        }

        private void HandleCloseClicked()
        {
            if (_state.CurrentState == GameState.InventoryViewing)
            {
                _popupView.Hide(_flowCallbacks.CompleteInventoryClose);
            }
        }

        #endregion
    }
}
