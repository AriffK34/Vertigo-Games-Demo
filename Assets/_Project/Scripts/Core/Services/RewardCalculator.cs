using System;
using VertigoDemo.Data;

namespace VertigoDemo.Core
{
    public sealed class RewardCalculator
    {
        #region Public Methods

        public int Calculate(ItemData item, int baseAmount, int zone)
        {
            if (!item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (baseAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseAmount));
            }

            if (zone < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(zone));
            }

            RewardGrowthConfig profile = item.GrowthProfile;
            if (!profile)
            {
                throw new InvalidOperationException($"{item.name}: Reward growth profile must be assigned.");
            }

            float growthPerZone = profile.GrowthPerZone;
            if (growthPerZone < 0f ||
                float.IsNaN(growthPerZone) ||
                float.IsInfinity(growthPerZone))
            {
                throw new InvalidOperationException(
                    $"{profile.name}: Growth per zone must be finite and non-negative.");
            }

            double multiplier = 1d + (double)growthPerZone * (zone - 1);
            double amount = baseAmount * multiplier;

            return checked((int)Math.Round(amount, MidpointRounding.AwayFromZero));
        }

        #endregion
    }
}