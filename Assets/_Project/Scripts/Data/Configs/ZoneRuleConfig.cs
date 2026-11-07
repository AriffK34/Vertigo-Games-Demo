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
        [SerializeField] private bool _hasEntryEffect;
        [SerializeField] private Color _entryEffectColor = Color.white;

        #endregion

        #region Properties

        public WheelConfig Wheel
        {
            get
            {
                return _wheel;
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

        public bool HasEntryEffect
        {
            get
            {
                return _hasEntryEffect;
            }
        }

        public Color EntryEffectColor
        {
            get
            {
                return _entryEffectColor;
            }
        }

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            _firstZone = Mathf.Max(1, _firstZone);
            _lastZone = Mathf.Max(0, _lastZone);
            _interval = Mathf.Max(1, _interval);

            if (_lastZone > 0 && _lastZone < _firstZone)
            {
                _lastZone = _firstZone;
            }

            if (_wheel)
            {
                ValidateWheel();
            }
        }

        #endregion

        #region Public Methods

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

        [ContextMenu("Validate Wheel")]
        public void ValidateWheel()
        {
            if (!_wheel)
            {
                throw new System.InvalidOperationException($"{name}: A wheel config must be assigned.");
            }
            _wheel.ValidateConfiguration(_hasBomb);
        }

        #endregion
    }
}
