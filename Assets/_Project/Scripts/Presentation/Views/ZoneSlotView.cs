using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class ZoneSlotView : MonoBehaviour
    {
        [SerializeField] private Image _zoneBgImage;
        [SerializeField] private TextMeshProUGUI _zoneNumberText;

        public void Setup(Sprite bgSprite, string text)
        {
            if (_zoneBgImage != null) _zoneBgImage.sprite = bgSprite;
            if (_zoneNumberText != null) _zoneNumberText.text = text;
            
        }
    }
}