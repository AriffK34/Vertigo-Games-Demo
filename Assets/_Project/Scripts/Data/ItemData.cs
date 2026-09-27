using UnityEngine;

namespace VertigoDemo.Data
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Wheel/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private ItemType itemType;
        [SerializeField] private Sprite itemIcon;
        
        public string ItemName => itemName;
        public ItemType ItemType => itemType;
        public Sprite ItemIcon => itemIcon;
    }
}
