using UnityEngine;
using UnityEngine.Serialization;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewWheelTheme", menuName = "Wheel/Wheel Theme")]
    public class WheelTheme : ScriptableObject
    {
        [SerializeField] private string themeName;
        [SerializeField] private WheelType wheelType;
        [SerializeField] private Sprite wheelSprite;
        [SerializeField] private Sprite indicatorSprite;
        
        public string ThemeName => themeName;
        public WheelType WheelType => wheelType;
        public Sprite WheelSprite => wheelSprite;
        public Sprite IndicatorSprite => indicatorSprite;
    }
}
