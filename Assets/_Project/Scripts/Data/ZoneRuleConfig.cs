using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "ZoneRuleConfig", menuName = "Wheel/Zone Rule Config")]
    public class ZoneRuleConfig : ScriptableObject
    {
        #region Fields

        [SerializeField, Min(1)] private int _firstZone = 1;
        [SerializeField, Min(0)] private int _lastZone;
        [SerializeField, Min(1)] private int _interval = 1;

        [SerializeField] private WheelConfig _wheel;
        [SerializeField] private bool _hasBomb = true;
        [SerializeField] private bool _isLeaveAllowed;
        [SerializeField] private Sprite _zoneSprite;

        #endregion

        #region Properties

        public WheelConfig Wheel
        {
            get
            {
                return _wheel;
            }
        }

        public bool HasBomb
        {
            get
            {
                return _hasBomb;
            }
        }
        
        public bool IsLeaveAllowed
        {
            get
            {
                return _isLeaveAllowed;
            }
        }
        
        public Sprite ZoneSprite
        {
            get
            {
                return _zoneSprite;
            }
        }

        #endregion

        #region Methods

        public bool IsMatch(int zone)
        {
            if (zone < _firstZone)
            {
                return false;
            }

            if (_lastZone > 0 && zone > _lastZone)
            {
                return false;
            }

            return _interval > 0 && zone % _interval == 0;
        }

        #endregion

        #region Validation

        private void OnValidate()
        {
            _firstZone = Mathf.Max(1, _firstZone);
            _lastZone = Mathf.Max(0, _lastZone);
            _interval = Mathf.Max(1, _interval);
            
            if (_lastZone > 0 && _lastZone < _firstZone)
            {
                _lastZone = _firstZone;
            }
        }

        #endregion
    }
}