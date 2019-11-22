using System;

namespace TextPortCore.Helpers
{
    public static class Formatting
    {
        public static string DisplayMoney(decimal? decValue)
        {
            if (decValue != null)
            {
                return $"{decValue:C2}";
            }
            return "$0.00";
        }
    }
}
