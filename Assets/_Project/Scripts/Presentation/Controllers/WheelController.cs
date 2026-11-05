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
        private WheelSliceData _spinResult;
        
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
            HandleGameStateChanged();
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
            if (gameManager.CurrentState != GameState.Idle)
            {
                return;
            }

            WheelConfig currentConfig = zoneManager.GetCurrentWheel();
            int targetIndex = Random.Range(0, currentConfig.WheelSlices.Length);
            _spinResult = currentConfig.WheelSlices[targetIndex];
            OnSpinStarted?.Invoke();
            wheelView.RotateWheel(targetIndex, HandleSpinFinished);
        }
        

        private void HandleSpinFinished()
        {
            WheelSliceData result = _spinResult;
            _spinResult = null;
            OnSpinLanded?.Invoke(result);
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
            wheelView.ResetRotation(0.6f, gameManager.CompleteZoneTransition);
        }
        
        

        private void HandleGameStateChanged()
        {
            wheelView.SetSpinInteractable(gameManager.CurrentState == GameState.Idle);
        }
    }    
}