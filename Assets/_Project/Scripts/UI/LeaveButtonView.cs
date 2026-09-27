using System;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public class LeaveButtonView : MonoBehaviour
    {
        public event Action OnLeaveClicked;

        [SerializeField] private Button leaveButton;

        private void OnValidate()
        {
            if (leaveButton == null) leaveButton = GetComponent<Button>();
        }

        private void Awake()
        {
            leaveButton.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDestroy()
        {
            if (leaveButton != null) leaveButton.onClick.RemoveListener(HandleButtonClicked);
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