using System;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class LeaveButtonView : MonoBehaviour
    {
        public event Action OnLeaveClicked;

        [SerializeField] private Button _leaveButton;

        private void OnValidate()
        {
            if (_leaveButton == null) _leaveButton = GetComponent<Button>();
        }

        private void Awake()
        {
            _leaveButton.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDestroy()
        {
            if (_leaveButton != null) _leaveButton.onClick.RemoveListener(HandleButtonClicked);
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void HandleButtonClicked()
        {
            OnLeaveClicked?.Invoke();
        }
    }
}