using System.Collections.Generic;
using VertigoDemo.Data;

namespace VertigoDemo.Core.Contracts
{
    public interface IRunRewardsReader
    {
        IReadOnlyDictionary<ItemData, int> Inventory { get; }
    }
}