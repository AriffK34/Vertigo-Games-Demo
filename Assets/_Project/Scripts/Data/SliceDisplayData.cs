using UnityEngine;

namespace VertigoDemo.Data
{
    public readonly struct SliceDisplayData
    {
        public readonly Sprite Icon;
        public readonly string Text;

        public SliceDisplayData(Sprite icon, string text)
        {
            Icon = icon;
            Text = text;
        }
    }
}