using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewWheelConfig", menuName = "Wheel/Wheel Config")]
    public class WheelConfig : ScriptableObject
    {
        [SerializeField] private WheelTheme wheelTheme;
        [SerializeField] private WheelSliceData[] wheelSlices = new WheelSliceData[8];
        
        public WheelTheme WheelTheme => wheelTheme;
        public WheelSliceData[] WheelSlices => wheelSlices;

        private void OnValidate()
        {
            if (wheelSlices is not { Length: 8 })
            {
                System.Array.Resize(ref wheelSlices, 8);
            }
        }
    }
}
