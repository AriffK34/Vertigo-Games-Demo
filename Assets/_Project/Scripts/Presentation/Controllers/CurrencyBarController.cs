using UnityEngine;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class CurrencyBarController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameManager _gameManager;
        [SerializeField] private CurrencyBarView _currencyBarView;

        private EconomyService _economy;

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            if (!_gameManager) _gameManager = GetComponent<GameManager>();
        }

        private void Start()
        {
            _economy = _gameManager.Economy;
            _economy.OnBalancesChanged += RefreshBalances;
            _currencyBarView.Refresh(_economy.Balances, false);
        }

        private void OnDestroy()
        {
            if (_economy != null) _economy.OnBalancesChanged -= RefreshBalances;
        }

        #endregion

        #region Methods

        private void RefreshBalances()
        {
            _currencyBarView.Refresh(_economy.Balances, true);

        }

        #endregion
    } 
}
