using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class ZoneSlotView : MonoBehaviour
    {
        [SerializeField] private Image zoneBgImage;
        [SerializeField] private TextMeshProUGUI zoneNumberText;

        public void Setup(Sprite bgSprite, string text)
        {
            if (zoneBgImage != null) zoneBgImage.sprite = bgSprite;
            if (zoneNumberText != null) zoneNumberText.text = text;
            
        }
    }
}