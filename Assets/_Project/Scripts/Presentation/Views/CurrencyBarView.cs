using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Animations;
using VertigoDemo.Presentation.Formatting;

namespace VertigoDemo.UI
{
    public class CurrencyBarView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CurrencyBinding[] _bindings = Array.Empty<CurrencyBinding>();
        [SerializeField] private UIAnimationConfig _animationConfig;

        #endregion

        #region Unity Callbacks

        private void OnDestroy()
        {
            foreach (var binding in _bindings)
            {
                if (binding != null && binding.AmountText)
                {
                    binding.AmountText.rectTransform.DOKill();
                }
            }
        }

        #endregion

        #region Public Methods

        public void Refresh(IReadOnlyDictionary<ItemData, int> balances, bool hasAnimation)
        {
            foreach (CurrencyBinding binding in _bindings)
            {
                if (binding == null || !binding.Currency || !binding.AmountText)
                {
                    throw new InvalidOperationException("Each currency binding requires a currency and amount text.");
                }

                balances.TryGetValue(binding.Currency, out int amount);
                string amountText = NumberFormatter.Format(amount);
                if (binding.AmountText.text == amountText)
                {
                    continue;
                }

                binding.AmountText.text = amountText;
                RectTransform amountTransform = binding.AmountText.rectTransform;
                amountTransform.DOKill();
                amountTransform.localScale = Vector3.one;

                if (hasAnimation)
                {
                    UIPunchAnimation.Play(
                        amountTransform,
                        _animationConfig.CurrencyPunch,
                        gameObject);
                }
            }
        }

        #endregion

        #region Nested Types

        [Serializable]
        private sealed class CurrencyBinding
        {
            #region Fields

            [SerializeField] private ItemData _currency;
            [SerializeField] private TextMeshProUGUI _amountText;

            #endregion

            #region Properties

            public ItemData Currency
            {
                get
                {
                    return _currency;
                }
            }

            public TextMeshProUGUI AmountText
            {
                get
                {
                    return _amountText;
                }
            }

            #endregion
        }

        #endregion
    }
}
