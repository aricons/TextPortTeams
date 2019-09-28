using System;

namespace TextPortCore.Helpers
{
    public static class Conversion
    {
        public static int StringToIntOrZero(string inStr)
        {
            if (!string.IsNullOrEmpty(inStr))
            {
                if (int.TryParse(inStr, out int outInt))
                {
                    return outInt;
                }
            }
            return 0;
        }
    }
}
