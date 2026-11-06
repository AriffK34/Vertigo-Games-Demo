using System;
using UnityEngine;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Messaging;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class CurrencyBarController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CurrencyBarView _currencyBarView;
        private IPlayerInventoryReader _inventory;
        private IEventBus _eventBus;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<PlayerInventoryChanged>(HandlePlayerInventoryChanged);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(IPlayerInventoryReader inventory, IEventBus eventBus)
        {
            if (_eventBus != null)
            {
                throw new InvalidOperationException("Currency bar controller is already initialized.");
            }

            if (!_currencyBarView)
            {
                throw new InvalidOperationException($"{name}: Currency bar view must be assigned.");
            }

            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _eventBus.Subscribe<PlayerInventoryChanged>(HandlePlayerInventoryChanged);
            _currencyBarView.Refresh(_inventory.Items, false);
        }

        #endregion

        #region Private Methods

        private void HandlePlayerInventoryChanged(PlayerInventoryChanged message)
        {
            _currencyBarView.Refresh(_inventory.Items, true);
        }

        #endregion
    }
}
