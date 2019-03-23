using System;

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
    }
}
