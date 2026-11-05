using UnityEngine;
using UnityEngine.Serialization;
using VertigoDemo.Data;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class ZoneBarController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameManager _gameManager;
        [SerializeField] private ZoneManager _zoneManager;
        [SerializeField] private ZoneBarView _zoneBarView;
        [SerializeField] private Sprite _currentZoneSprite;

        private const int VISIBLE_SLOT_COUNT = 7;
        private const int TOTAL_SLOT_COUNT = VISIBLE_SLOT_COUNT + 1;

        private readonly ZoneSlotDisplayData[] _slotsData = new ZoneSlotDisplayData[TOTAL_SLOT_COUNT];

        private int _displayedZone;

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!_gameManager)
            {
                _gameManager = GetComponent<GameManager>();
            }

            if (!_zoneManager)
            {
                _zoneManager = GetComponent<ZoneManager>();
            }
        }

        private void Start()
        {
            _zoneManager.OnZoneChanged += UpdateZoneBar;
            UpdateZoneBar();
        }

        private void OnDestroy()
        {
            if (_zoneManager)
            {
                _zoneManager.OnZoneChanged -= UpdateZoneBar;
            }
        }

        #endregion

        #region Private Methods

        private void UpdateZoneBar()
        {
            int currentZone = _zoneManager.CurrentZone;
            bool hasAnimation = _displayedZone > 0 && currentZone == _displayedZone + 1;
            
            for (int i = 0; i < TOTAL_SLOT_COUNT; i++)
            {
                int zoneNumber = currentZone + i;
                Sprite sprite = i == 0
                    ? _currentZoneSprite
                    : _zoneManager.GetRule(zoneNumber).ZoneSprite;
                
                _slotsData[i] = new ZoneSlotDisplayData(sprite, zoneNumber.ToString());
            }

            _displayedZone = currentZone;
            _zoneBarView.UpdateSlots(_slotsData, hasAnimation, _gameManager.CompleteZoneBarTransition);
        }

        #endregion
    }

}
