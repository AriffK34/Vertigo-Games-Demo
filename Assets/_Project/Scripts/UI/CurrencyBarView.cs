using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VertigoDemo.UI
{
    public class CurrencyBarView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI cashText;

        public void UpdateGold(int amount)
        {
            goldText.text = amount.ToString();
            
            goldText.rectTransform.DOKill();
            goldText.rectTransform.localScale = Vector3.one;
            goldText.rectTransform.DOPunchScale(Vector3.one * 0.35f, 0.3f, 2, 0.5f);
        }
        
        public void UpdateCash(int amount)
        {
            cashText.text = amount.ToString();
            cashText.rectTransform.DOKill();
            cashText.rectTransform.localScale = Vector3.one;
            cashText.rectTransform.DOPunchScale(Vector3.one * 0.35f, 0.3f, 2, 0.5f);
        }
    }
}