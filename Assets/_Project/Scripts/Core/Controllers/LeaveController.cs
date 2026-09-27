using UnityEngine;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class LeaveController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ZoneManager zoneManager;
        [SerializeField] private LeaveButtonView leaveButtonView;
        
        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
            if (zoneManager == null) zoneManager = GetComponent<ZoneManager>();
        }
        
        private void Start()
        {
            leaveButtonView.OnLeaveClicked += HandleLeaveClicked;
            gameManager.OnGameStateChanged += HandleGameStateChanged;
            zoneManager.OnZoneChanged += HandleZoneChanged;
        }

        private void OnDestroy()
        {
            if (leaveButtonView != null) leaveButtonView.OnLeaveClicked -= HandleLeaveClicked;
            if (gameManager != null) gameManager.OnGameStateChanged -= HandleGameStateChanged;
            if (zoneManager != null) zoneManager.OnZoneChanged -= HandleZoneChanged;
        }
        
        private void HandleLeaveClicked()
        {
            gameManager.RequestLeave();
        }
        
        private void HandleGameStateChanged()
        {
            UpdateButtonState();
        }
        
        private void HandleZoneChanged()
        {
            UpdateButtonState();
        }
        
        private void UpdateButtonState()
        {
            bool canLeave = zoneManager.CanLeave && gameManager.CurrentState == GameState.Idle;
            leaveButtonView.SetVisible(canLeave);
        }
    }
}
