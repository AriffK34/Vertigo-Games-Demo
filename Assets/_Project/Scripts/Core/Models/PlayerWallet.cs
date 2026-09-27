using System;

namespace VertigoDemo.Core
{
    public class PlayerWallet
    {
        public event Action<int> OnGoldChanged;
        public event Action<int> OnCashChanged;
        
        public int Gold { get; private set; }
        public int Cash { get; private set; }

        public void AddGold(int amount)
        {
            Gold += amount;
            OnGoldChanged?.Invoke(Gold);
        }
        
        public void AddCash(int amount)
        {
            Cash += amount;
            OnCashChanged?.Invoke(Cash);
        }

        public bool TrySpendGold(int amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }
    }
}