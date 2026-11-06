using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class ZoneSlotView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image _zoneBgImage;
        [SerializeField] private TextMeshProUGUI _zoneNumberText;

        #endregion

        #region Public Methods

        public void Setup(Sprite backgroundSprite, string text)
        {
            if (_zoneBgImage)
            {
                _zoneBgImage.sprite = backgroundSprite;
            }

            if (_zoneNumberText)
            {
                _zoneNumberText.text = text;
            }
        }

        #endregion
    }
}
