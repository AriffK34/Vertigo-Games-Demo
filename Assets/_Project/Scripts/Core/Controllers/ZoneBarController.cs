using UnityEngine;
using UnityEngine.Serialization;
using VertigoDemo.Data;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class ZoneBarController : MonoBehaviour
    {
        [SerializeField] private ZoneManager _zoneManager;
        [SerializeField] private ZoneBarView _zoneBarView;
        [SerializeField] private Sprite _currentZoneSprite;

        private const int SlotCount = 7;

        private readonly ZoneSlotDisplayData[] _slotsData = new ZoneSlotDisplayData[SlotCount];

        private void OnValidate()
        {
            if (!_zoneManager) _zoneManager = GetComponent<ZoneManager>();
        }

        private void Start()
        {
            _zoneManager.OnZoneChanged += UpdateZoneBar;
            UpdateZoneBar();
        }

        private void OnDestroy()
        {
            if (_zoneManager) _zoneManager.OnZoneChanged -= UpdateZoneBar;
        }

        private void UpdateZoneBar()
        {
            int currentZone = _zoneManager.CurrentZone;
            for (int i = 0; i < SlotCount; i++)
            {
                int zoneNumber = currentZone + i;
                Sprite spriteToUse = i == 0
                    ? _currentZoneSprite
                    : _zoneManager.GetRule(zoneNumber).ZoneSprite;
                
                _slotsData[i] = new ZoneSlotDisplayData(spriteToUse, zoneNumber.ToString());
            }
            _zoneBarView.UpdateSlots(_slotsData);
        }
    }

}
