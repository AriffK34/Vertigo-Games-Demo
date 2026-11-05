using UnityEngine;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class BombPopupController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BombPopupView bombPopupView;
        
        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
        }
        private void Start()
        {
            gameManager.OnGameStateChanged += HandleGameStateChanged;
            bombPopupView.OnGiveUpClicked += HandleGiveUpClicked;
            bombPopupView.OnReviveClicked += HandleReviveClicked;
        }

        private void OnDestroy()
        {
            if (gameManager != null) gameManager.OnGameStateChanged -= HandleGameStateChanged;
            if (bombPopupView != null)
            {
                bombPopupView.OnGiveUpClicked -= HandleGiveUpClicked;
                bombPopupView.OnReviveClicked -= HandleReviveClicked;
            }
        }
        
        private void HandleGameStateChanged()
        {
            if (gameManager.CurrentState == GameState.BombExploded)
            {
                bombPopupView.Show(gameManager.Economy.HasReviveFunds);
            }
        }
        
        private void HandleGiveUpClicked()
        {
            bombPopupView.Hide();
            gameManager.GiveUp();
        }
        private void HandleReviveClicked()
        {
            if (gameManager.TryRevive()) bombPopupView.Hide();
        }
    }
}

