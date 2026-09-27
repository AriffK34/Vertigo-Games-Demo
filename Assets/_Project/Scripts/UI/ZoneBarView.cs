using UnityEngine;
using VertigoDemo.Data;

namespace VertigoDemo.UI
{
    public class ZoneBarView : MonoBehaviour
    {
        [SerializeField] private ZoneSlotView[] zoneSlots;

        private void OnValidate()
        {
            if (zoneSlots == null || zoneSlots.Length == 0) zoneSlots = GetComponentsInChildren<ZoneSlotView>();
        }

        public void UpdateSlots(ZoneSlotDisplayData[] slotsData)
        {
            if (zoneSlots == null || slotsData == null) return;
            int count = Mathf.Min(zoneSlots.Length, slotsData.Length);
            for (int i = 0; i < count; i++)
            {
                zoneSlots[i].Setup(slotsData[i].BgSprite, slotsData[i].Text);
            }
        }
    }

}