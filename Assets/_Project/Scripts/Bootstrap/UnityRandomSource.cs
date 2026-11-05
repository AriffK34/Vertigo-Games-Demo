using System;
using VertigoDemo.Core.Contracts;

namespace VertigoDemo.Bootstrap
{
    public sealed class UnityRandomSource : IRandomSource
    {
        public int Next(int maxExclusive)
        {
            if (maxExclusive <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));
            }

            return UnityEngine.Random.Range(0, maxExclusive);
        }
    }
}