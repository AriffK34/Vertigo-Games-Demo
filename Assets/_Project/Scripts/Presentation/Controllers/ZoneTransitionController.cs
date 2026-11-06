using System;
using DG.Tweening;
using UnityEngine;
using VertigoDemo.Core;
using VertigoDemo.Core.Contracts;
using VertigoDemo.Core.Events;
using VertigoDemo.Data;
using VertigoDemo.Messaging;
using VertigoDemo.Presentation.Formatting;
using VertigoDemo.UI;

namespace VertigoDemo.Presentation
{
    public class ZoneTransitionController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private WheelView _wheelView;
        [SerializeField] private ZoneBarView _zoneBarView;
        [SerializeField] private ZoneEntryEffectView _entryEffectView;
        [SerializeField] private WheelAnimationConfig _wheelAnimationConfig;
        [SerializeField] private Sprite _currentZoneSprite;
        private IZoneReader _zones;
        private IGameFlowCallbacks _flowCallbacks;
        private IEventBus _eventBus;
        private RewardCalculator _rewardCalculator;
        private SliceDisplayData[] _sliceData = Array.Empty<SliceDisplayData>();
        private ZoneSlotDisplayData[] _zoneData = Array.Empty<ZoneSlotDisplayData>();
        private Tween _transitionTween;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<ZoneTransitionRequested>(HandleTransitionRequested);
            CancelTransition();
        }

        #endregion

        #region Public Methods

        public void Initialize(
            IZoneReader zones,
            IGameFlowCallbacks flowCallbacks,
            IEventBus eventBus,
            RewardCalculator rewardCalculator)
        {
            if (_eventBus != null)
            {
                throw new InvalidOperationException("Zone transition controller is already initialized.");
            }

            if (!_wheelView ||
                !_zoneBarView ||
                !_entryEffectView ||
                !_wheelAnimationConfig ||
                !_currentZoneSprite)
            {
                throw new InvalidOperationException($"{name}: Zone transition references must be assigned.");
            }

            if (_zoneBarView.SlotCount == 0)
            {
                throw new InvalidOperationException($"{name}: Zone bar requires slots.");
            }

            _zones = zones ?? throw new ArgumentNullException(nameof(zones));
            _flowCallbacks = flowCallbacks ?? throw new ArgumentNullException(nameof(flowCallbacks));
            _rewardCalculator = rewardCalculator ?? throw new ArgumentNullException(nameof(rewardCalculator));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _eventBus.Subscribe<ZoneTransitionRequested>(
                HandleTransitionRequested);
        }

        #endregion

        #region Private Methods

        private void HandleTransitionRequested(ZoneTransitionRequested message)
        {
            CancelTransition();

            ZoneRuleConfig rule = _zones.GetRule(message.Zone);
            WheelConfig wheel = rule.Wheel;

            PrepareWheelData(wheel, message.Zone);
            PrepareZoneData(message.Zone);

            _wheelView.UpdateWheel(wheel.WheelTheme, _sliceData);

            Sequence sequence = DOTween.Sequence();
            _transitionTween = sequence;

            sequence.Join(
                _wheelView.ResetRotation(_wheelAnimationConfig.ResetDuration, null));

            Tween zoneBarTween = _zoneBarView.UpdateSlots(_zoneData, !message.IsReset, null);

            if (zoneBarTween != null)
            {
                sequence.Join(zoneBarTween);
            }

            if (!message.IsReset && rule.HasEntryEffect)
            {
                sequence.Join(
                    _entryEffectView.Play(rule.EntryEffectColor, null));
            }

            sequence
                .SetRecyclable(false)
                .SetLink(gameObject)
                .OnComplete(HandleTransitionComplete);
        }

        private void PrepareWheelData(WheelConfig wheel, int zone)
        {
            if (_sliceData.Length != wheel.WheelSlices.Length)
            {
                _sliceData = new SliceDisplayData[wheel.WheelSlices.Length];
            }

            for (int i = 0; i < _sliceData.Length; i++)
            {
                WheelSliceData slice = wheel.WheelSlices[i];
                string text = string.Empty;

                if (!slice.IsBomb)
                {
                    int amount = _rewardCalculator.Calculate(
                        slice.ItemData,
                        slice.ItemAmount,
                        zone);

                    text = $"x{NumberFormatter.Format(amount)}";
                }

                _sliceData[i] = new SliceDisplayData(slice.ItemData.ItemIcon, text);
            }
        }

        private void PrepareZoneData(int currentZone)
        {
            if (_zoneData.Length != _zoneBarView.SlotCount)
            {
                _zoneData = new ZoneSlotDisplayData[_zoneBarView.SlotCount];
            }

            for (int i = 0; i < _zoneData.Length; i++)
            {
                int zone = currentZone + i;
                Sprite sprite = i == 0
                    ? _currentZoneSprite
                    : _zones.GetRule(zone).ZoneSprite;

                _zoneData[i] = new ZoneSlotDisplayData(sprite, zone.ToString());
            }
        }
        
        private void CancelTransition()
        {
            _transitionTween?.Kill();
            _transitionTween = null;

            if (_wheelView)
            {
                _wheelView.CancelRotation();
            }

            if (_zoneBarView)
            {
                _zoneBarView.CancelTransition();
            }

            if (_entryEffectView)
            {
                _entryEffectView.Cancel();
            }
        }

        private void HandleTransitionComplete()
        {
            _transitionTween = null;
            _flowCallbacks.CompleteZoneTransition();
        }

        #endregion
    }
}
