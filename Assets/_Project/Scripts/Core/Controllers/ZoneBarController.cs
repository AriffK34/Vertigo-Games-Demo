using UnityEngine;
using VertigoDemo.Data;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class ZoneBarController : MonoBehaviour
    {
        [SerializeField] private ZoneManager zoneManager;
        [SerializeField] private ZoneBarView zoneBarView;

        [SerializeField] private Sprite currentZoneSprite;
        [SerializeField] private Sprite normalZoneSprite;
        [SerializeField] private Sprite safeZoneSprite;
        [SerializeField] private Sprite superZoneSprite;

        private const int SlotCount = 7;

        private readonly ZoneSlotDisplayData[] _slotsData = new ZoneSlotDisplayData[SlotCount];

        private void OnValidate()
        {
            if (zoneManager == null) zoneManager = GetComponent<ZoneManager>();
        }

        private void Start()
        {
            zoneManager.OnZoneChanged += UpdateZoneBar;
            UpdateZoneBar();
        }

        private void OnDestroy()
        {
            if (zoneManager != null) zoneManager.OnZoneChanged -= UpdateZoneBar;
        }

        private void UpdateZoneBar()
        {
            int currentZone = zoneManager.CurrentZone;
            for (int i = 0; i < SlotCount; i++)
            {
                int zoneNumber = currentZone + i;
                Sprite spriteToUse;
                if (i == 0)
                {
                    spriteToUse = currentZoneSprite;
                }
                else if (zoneManager.IsZoneSuper(zoneNumber))
                {
                    spriteToUse = superZoneSprite;
                }
                else if (zoneManager.IsZoneSafe(zoneNumber))
                {
                    spriteToUse = safeZoneSprite;
                }
                else
                {
                    spriteToUse = normalZoneSprite;
                }
                _slotsData[i] = new ZoneSlotDisplayData(spriteToUse, zoneNumber.ToString());
            }
            zoneBarView.UpdateSlots(_slotsData);
        }
    }

}
