using System;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class LeaveButtonView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Button _leaveButton;

        #endregion

        #region Events

        public event Action OnLeaveClicked;

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            if (!_leaveButton)
            {
                _leaveButton = GetComponent<Button>();
            }
        }

        private void Awake()
        {
            _leaveButton.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDestroy()
        {
            if (_leaveButton)
            {
                _leaveButton.onClick.RemoveListener(HandleButtonClicked);
            }
        }

        #endregion

        #region Public Methods

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        #endregion

        #region Private Methods

        private void HandleButtonClicked()
        {
            OnLeaveClicked?.Invoke();
        }

        #endregion
    }
}
