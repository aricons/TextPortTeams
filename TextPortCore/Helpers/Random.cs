using System;
using System.Collections.Generic;

namespace TextPortCore.Helpers
{
    public static class RandomString
    {
        private static Random rnd = new Random();

        public static int RandomNumber()
        {
            return rnd.Next(1000, 99999);
        }

        public static string RandomNumberString()
        {
            return $"{rnd.Next(1, 99999):D5}";
        }

        public static string GenerateRandomToken(int length)
        {
            const string allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] chars = new char[length];
            int setLength = allowedChars.Length;

            for (int i = 0; i < length; ++i)
            {
                chars[i] = allowedChars[rnd.Next(setLength)];
            }

            return new string(chars, 0, length);
        }

        public static string GenerateRandomTokenNoJ(int length)
        {
            const string allowedChars = "0123456789ABCDEFGHIKLMNOPQRSTUVWXYZabcdefghiklmnopqrstuvwxyz";
            char[] chars = new char[length];
            int setLength = allowedChars.Length;

            for (int i = 0; i < length; ++i)
            {
                chars[i] = allowedChars[rnd.Next(setLength)];
            }

            return new string(chars, 0, length);
        }

        public static string GenerateJDelimitedUrlParameter(List<int> paramValues)
        {
            string leader = RandomString.GenerateRandomTokenNoJ(8);
            string trailer = RandomString.GenerateRandomTokenNoJ(7);

            string urlKey = $"{leader}J";

            if (paramValues.Count > 0)
            {
                urlKey += $"{paramValues[0] + Constants.AccountIdScramblerOffset}J";
            }

            if (paramValues.Count > 1)
            {
                urlKey += $"{RandomString.GenerateRandomTokenNoJ(5)}J{paramValues[1]}J";
            }

            if (paramValues.Count > 2)
            {
                urlKey += $"{RandomString.GenerateRandomTokenNoJ(5)}J{paramValues[2]}J";
            }

            return urlKey += $"{trailer}";
        }


        public static List<int> ExtractJDelimitedValues(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string[] segments = input.Split('J');
                if (segments.Length > 0)
                {
                    List<int> output = new List<int>();
                    for (int i = 0; i < segments.Length; i++)
                    {
                        // Only extract segments 1, 3, and 5 if it exists (0-based). The rest are fillers.
                        if (i == 1 || i == 3 || i == 5)
                        {
                            int value = Conversion.StringToIntOrZero(segments[i]);
                            // First field is the account ID. Subtract AccountIdScramblerOffset from it.
                            if (i == 1)
                            {
                                value -= Constants.AccountIdScramblerOffset;
                            }
                            output.Add(value);
                        }
                    }
                    return output;
                }
            }

            return null;
        }
    }
}
