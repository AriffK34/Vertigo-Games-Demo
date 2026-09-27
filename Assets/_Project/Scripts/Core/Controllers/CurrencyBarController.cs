using UnityEngine;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class CurrencyBarController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CurrencyBarView currencyBarView;

        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
        }

        private void Start()
        {
            PlayerWallet wallet = gameManager.Wallet;
            wallet.OnGoldChanged += HandleGoldChanged;
            wallet.OnCashChanged += HandleCashChanged;
            currencyBarView.UpdateGold(wallet.Gold);
            currencyBarView.UpdateCash(wallet.Cash);
        }

        private void OnDestroy()
        {
            if (gameManager != null && gameManager.Wallet != null)
            {
                gameManager.Wallet.OnGoldChanged -= HandleGoldChanged;
                gameManager.Wallet.OnCashChanged -= HandleCashChanged;
            }
        }

        private void HandleGoldChanged(int amount)
        {
            currencyBarView.UpdateGold(amount);
        }
        
        private void HandleCashChanged(int amount)
        {
            currencyBarView.UpdateCash(amount);
        }
    } 
}
