using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class ClaimPopupController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ClaimPopupView claimPopupView;
        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
        }
        
        private void Start()
        {
            gameManager.OnGameStateChanged += HandleGameStateChanged;
            claimPopupView.OnClaimClicked += HandleClaimClicked;
            claimPopupView.Hide();
        }
        
        private void OnDestroy()
        {
            if (gameManager != null) gameManager.OnGameStateChanged -= HandleGameStateChanged;
            if (claimPopupView != null) claimPopupView.OnClaimClicked -= HandleClaimClicked;
        }
        private void HandleGameStateChanged()
        {
            if (gameManager.CurrentState == GameState.RewardsClaiming)
            {
                claimPopupView.Show(gameManager.Inventory.Inventory);
            }
        }
        private void HandleClaimClicked()
        {
            claimPopupView.Hide();
            gameManager.ClaimRewards();
        }
    }    
}
