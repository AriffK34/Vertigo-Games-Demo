using System;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    [RequireComponent(typeof(Button))]
    public class InventoryButtonView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Button _button;

        #endregion

        #region Events

        public event Action OnClicked;

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        private void Awake()
        {
            _button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (_button)
            {
                _button.onClick.RemoveListener(HandleClicked);
            }
        }

        #endregion

        #region Public Methods

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        #endregion

        #region Private Methods

        private void HandleClicked()
        {
            OnClicked?.Invoke();
        }

        #endregion
    }
}
