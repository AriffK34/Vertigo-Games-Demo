using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core.Contracts
{
    public interface IEconomyReader
    {
        IReadOnlyDictionary<ItemData, int> Balances { get; }

        bool HasReviveFunds { get; }

        ItemData ReviveCurrency { get; }

        int ReviveCost { get; }

        int GetBalance(ItemData item);
    }
}