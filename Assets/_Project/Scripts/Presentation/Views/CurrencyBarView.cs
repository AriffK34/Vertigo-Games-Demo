using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VertigoDemo.Data;
using VertigoDemo.Presentation.Formatting;

namespace VertigoDemo.UI
{
    public class CurrencyBarView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CurrencyBinding[] _bindings = Array.Empty<CurrencyBinding>();
        [SerializeField, Min(0.01f)] private float _punchDuration = 0.3f;
        [SerializeField, Min(0f)] private float _punchStrength = 0.35f;
        [SerializeField, Min(1)] private int _punchVibrato = 2;
        [SerializeField, Range(0f, 1f)] private float _punchElasticity = 0.5f;
        
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

        #region Methods

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
                    amountTransform.DOPunchScale(
                            Vector3.one * _punchStrength,
                            _punchDuration,
                            _punchVibrato,
                            _punchElasticity)
                        .SetLink(gameObject);
                }
            }
        }

        #endregion

        #region Binding Definition

        [Serializable]
        private sealed class CurrencyBinding
        {
            [SerializeField] private ItemData _currency;
            [SerializeField] private TextMeshProUGUI _amountText;

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
        }

        #endregion
    }
}