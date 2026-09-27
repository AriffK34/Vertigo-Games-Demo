using UnityEngine;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private GameManager gameManager;

        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
        }

        private void Start()
        {
            inventoryView.Initialize(gameManager.Inventory);
        }
    }
}

