using System.Globalization;

namespace VertigoDemo.Presentation.Formatting
{
    public static class NumberFormatter
    {
        #region Public Methods

        public static string Format(int amount)
        {
            return amount.ToString(
                "N0",
                CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
