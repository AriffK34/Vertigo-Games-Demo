using System;
using UnityEngine;
using VertigoDemo.Data;
using VertigoDemo.UI;
using Random = UnityEngine.Random;

namespace VertigoDemo.Core
{
    public class WheelController : MonoBehaviour
    {
        public event Action OnSpinStarted;
        public event Action<WheelSliceData> OnSpinLanded;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private ZoneManager zoneManager;
        [SerializeField] private WheelView wheelView;

        private readonly SliceDisplayData[] _sliceDisplayCache = new SliceDisplayData[8];
        private bool _isSpinning;

        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
            if (zoneManager == null) zoneManager = GetComponent<ZoneManager>();
        }
        
        private void Start()
        {
            gameManager.OnGameStateChanged += HandleGameStateChanged;
            zoneManager.OnZoneChanged += UpdateWheel;
            wheelView.OnSpinClicked += HandleSpinClicked;
            UpdateWheel();
        }

        private void OnDestroy()
        {
            if (gameManager != null) gameManager.OnGameStateChanged -= HandleGameStateChanged;
            if (zoneManager != null) zoneManager.OnZoneChanged -= UpdateWheel;
            if (wheelView != null) wheelView.OnSpinClicked -= HandleSpinClicked;
        }

        private void HandleSpinClicked()
        {
            if (_isSpinning) return;
            _isSpinning = true;
            wheelView.SetSpinInteractable(false);
            OnSpinStarted?.Invoke();
            int targetIndex = Random.Range(0, 8);
            wheelView.RotateWheel(targetIndex, () => OnSpinFinished(targetIndex));
        }
        

        private void OnSpinFinished(int landedIndex)
        {
            WheelConfig currentConfig = zoneManager.GetCurrentWheel();
            WheelSliceData sliceData = currentConfig.WheelSlices[landedIndex];
            
            _isSpinning = false;
            OnSpinLanded?.Invoke(sliceData);
        }
        
        private void UpdateWheel()
        {
            WheelConfig currentConfig = zoneManager.GetCurrentWheel();
            for (int i = 0; i < 8; i++)
            {
                WheelSliceData slice = currentConfig.WheelSlices[i];
                string text = slice.IsBomb ? "" : $"x{zoneManager.CalculateRewardAmount(slice.ItemData, slice.ItemAmount)}";
                _sliceDisplayCache[i] = new SliceDisplayData(slice.ItemData.ItemIcon, text);
            }
            wheelView.UpdateWheel(currentConfig.WheelTheme, _sliceDisplayCache);
            wheelView.ResetRotation(0.6f, () =>
            {
                gameManager.CompleteZoneTransition();
            });
        }
        
        

        private void HandleGameStateChanged()
        {
            wheelView.SetSpinInteractable(gameManager.CurrentState == GameState.Idle);
        }
    }    
}