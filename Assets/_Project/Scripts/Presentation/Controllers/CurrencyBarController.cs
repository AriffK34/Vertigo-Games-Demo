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

        private IEconomyReader _economy;
        private IEventBus _events;

        #endregion

        #region LifeCycle

        private void OnDestroy()
        {
            if (_events != null)
            {
                _events.Unsubscribe<BalancesChanged>(HandleBalancesChanged);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(IEconomyReader economy, IEventBus events)
        {
            if (_events != null)
            {
                throw new InvalidOperationException("Currency bar controller is already initialized.");
            }

            if (!_currencyBarView)
            {
                throw new InvalidOperationException($"{name}: Currency bar view must be assigned.");
            }

            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _events = events ?? throw new ArgumentNullException(nameof(events));

            _events.Subscribe<BalancesChanged>(HandleBalancesChanged);
            _currencyBarView.Refresh(_economy.Balances, false);
        }
        
        #endregion

        #region Private Methods

        private void HandleBalancesChanged(BalancesChanged message)
        {
            _currencyBarView.Refresh(_economy.Balances, true);
        }

        #endregion
    } 
}
