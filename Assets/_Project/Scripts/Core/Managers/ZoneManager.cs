using System;
using UnityEngine;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public class ZoneManager : MonoBehaviour
    {
        public event Action OnZoneChanged;
        
        [SerializeField] private WheelConfig bronzeWheel;
        [SerializeField] private WheelConfig silverWheel;
        [SerializeField] private WheelConfig goldWheel;
        
        [SerializeField] private int silverInterval = 5;
        [SerializeField] private int goldInterval = 30;

        [SerializeField] private float growthMultiplier = 10f;
        [SerializeField] private float growthExponent = 0.08f;
        [SerializeField] private float linearSlope = 0.3334f;
        
        private int _currentZone = 1;
        
        public bool IsSuperZone => CurrentZone % goldInterval == 0;
        public bool IsSafeZone => CurrentZone % silverInterval == 0 && !IsSuperZone;
        public bool CanLeave => IsSafeZone || IsSuperZone;
        
        public int CurrentZone
        {
            get => _currentZone;
            private set
            {
                if (_currentZone == value) return;
                _currentZone = value;
                OnZoneChanged?.Invoke();
            }
        }

        public bool IsZoneSuper(int zone) => zone % goldInterval == 0;
        public bool IsZoneSafe(int zone) => zone % silverInterval == 0 && !IsZoneSuper(zone);
        
        public WheelConfig GetCurrentWheel()
        {
            if (IsSuperZone) return goldWheel;
            if (IsSafeZone) return silverWheel;
            return bronzeWheel;
        }
        
        public void AdvanceZone()
        {
            CurrentZone++;
        }
        
        public void ResetZone()
        {
            CurrentZone = 1;
        }
        
        public int CalculateRewardAmount(ItemData itemData, int baseAmount)
        {
            if (itemData.ItemType == ItemType.Item)
            {
                float itemMultiplier = 1f + Mathf.Sqrt((CurrentZone - 1f) / 10f);
                return Mathf.Max(1, Mathf.RoundToInt(baseAmount * itemMultiplier));
            }

            float multiplier = GetMultiplier(CurrentZone);

            if (IsSuperZone) multiplier *= 10f;
            
            int rawAmount = Mathf.FloorToInt(baseAmount * multiplier);
            return RoundToCleanNumber(rawAmount);
        }
        private int RoundToCleanNumber(int value)
        {
            if (value < 10) return value;
            float magnitude = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(value)));
            float step = magnitude >= 100f ? magnitude / 10f : 5f;
            return Mathf.RoundToInt(value / step) * (int)step;
        }

        
        public float GetMultiplier(int zone)
        {
            if (zone <= goldInterval) return GrowthFunction(zone);
            return GrowthFunction(goldInterval) + linearSlope * (zone - goldInterval);
        }
        
        private float GrowthFunction(int zone)
        {
            return 1 + growthMultiplier * (1 - Mathf.Exp(-growthExponent * (zone - 1)));
        }
    }
}
