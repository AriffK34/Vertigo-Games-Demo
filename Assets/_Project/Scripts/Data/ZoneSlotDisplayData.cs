using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertigoDemo.Data
{
    public readonly struct ZoneSlotDisplayData
    {
        public readonly Sprite BgSprite;
        public readonly string Text;

        public ZoneSlotDisplayData(Sprite bgSprite, string text)
        {
            BgSprite = bgSprite;
            Text = text;
        }
    }

}