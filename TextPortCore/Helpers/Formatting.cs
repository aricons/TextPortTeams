using System;

namespace TextPortCore.Helpers
{
    public static class Formatting
    {
        public static string DisplayMoney(decimal? decValue)
        {
            if (decValue != null)
            {
                return $"${decValue:F2}"; // Don't use C2 because it will use parentheses for nagative numbers instead of a neagtive (-) sign.
            }
            return "$0.00";
        }
    }
}
