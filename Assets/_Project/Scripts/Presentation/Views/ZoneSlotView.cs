using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class ZoneSlotView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image _zoneBackgroundImage;
        [SerializeField] private TextMeshProUGUI _zoneNumberText;

        #endregion

        #region Public Methods

        public void Setup(Sprite backgroundSprite, string text)
        {
            if (_zoneBackgroundImage)
            {
                _zoneBackgroundImage.sprite = backgroundSprite;
            }

            if (_zoneNumberText)
            {
                _zoneNumberText.text = text;
            }
        }

        #endregion
    }
}
