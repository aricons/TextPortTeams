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
    }
}
