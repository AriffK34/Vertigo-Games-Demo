using UnityEngine;

namespace VertigoDemo.Data
{
    public readonly struct ZoneSlotDisplayData
    {
        #region Fields

        public readonly Sprite BackgroundSprite;
        public readonly string Text;

        #endregion

        #region Constructors

        public ZoneSlotDisplayData(Sprite backgroundSprite, string text)
        {
            BackgroundSprite = backgroundSprite;
            Text = text;
        }

        #endregion
    }
}
