using System;
using UnityEngine;
using VertigoDemo.Data;
using VertigoDemo.UI;

namespace VertigoDemo.Core
{
    public class ZoneEntryEffectController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameManager _gameManager;
        [SerializeField] private ZoneManager _zoneManager;
        [SerializeField] private ZoneEntryEffectView _effectView;

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (!_gameManager)
            {
                _gameManager = GetComponent<GameManager>();
            }

            if (!_zoneManager)
            {
                _zoneManager = GetComponent<ZoneManager>();
            }
        }

        private void Start()
        {
            _zoneManager.OnZoneChanged += HandleZoneChanged;
            HandleZoneChanged();
        }

        private void OnDestroy()
        {
            if (_zoneManager)
            {
                _zoneManager.OnZoneChanged -= HandleZoneChanged;
            }

            if (_effectView)
            {
                _effectView.Cancel();
            }
        }

        #endregion

        #region Private Methods

        private void HandleZoneChanged()
        {
            _effectView.Cancel();

            ZoneRuleConfig rule = _zoneManager.CurrentRule;

            if (_gameManager.CurrentState == GameState.ZoneTransition && rule.HasEntryEffect)
            {
                _effectView.Play(rule.EntryEffectColor, _gameManager.CompleteZoneEntryEffect);
                return;
            }
            _gameManager.CompleteZoneEntryEffect();
        }

        #endregion
    }
}