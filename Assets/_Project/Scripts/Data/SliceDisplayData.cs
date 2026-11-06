using UnityEngine;

namespace VertigoDemo.Data
{
    public readonly struct SliceDisplayData
    {
        #region Fields

        public readonly Sprite Icon;
        public readonly string Text;

        #endregion

        #region Constructors

        public SliceDisplayData(Sprite icon, string text)
        {
            Icon = icon;
            Text = text;
        }

        #endregion
    }
}
