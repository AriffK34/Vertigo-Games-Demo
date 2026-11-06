using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.UI
{
    public enum PopupAction
    {
        GiveUp,
        Revive
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class PopupActionButton : MonoBehaviour
    {
        #region Fields

        [SerializeField] private PopupAction _action;
        [SerializeField, HideInInspector] private Button _button;

        #endregion

        #region Properties

        public PopupAction Action { get { return _action; } }
        public Button Button { get { return _button; } }

        #endregion

        #region Validation

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        #endregion
    }
}