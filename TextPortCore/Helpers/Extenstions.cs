using System;
using System.Text.RegularExpressions;

namespace TextPortCore.Helpers
{
    public static class Extensions
    {
        public static string ToE164(this string number)
        {
            string globalNumber = string.Empty;

            if (!string.IsNullOrEmpty(number))
            {
                try
                {
                    globalNumber = Regex.Replace(number, @"\D", "");
                    if (!globalNumber.StartsWith("1"))
                    {
                        globalNumber = $"1{globalNumber}";
                    }
                }
                catch (Exception ex)
                {
                    string foo = ex.Message;
                }
            }

            return globalNumber;
        }
    }
}