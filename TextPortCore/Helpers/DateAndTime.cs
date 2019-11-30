using System;

namespace TextPortCore.Helpers
{
    public static class DateAndTime
    {
        public static int GetRemainingHoursBetweenTwoDates(DateTime earlierDate, DateTime laterDate)
        {
            double daysDiff = (laterDate - earlierDate).TotalDays;
            double partialDays = daysDiff - (int)daysDiff;

            return (int)(partialDays * 24);
        }

        public static int GetDaysBetweenTwoDates(DateTime earlierDate, DateTime laterDate)
        {
            double daysDiff = (laterDate - earlierDate).TotalDays;

            return (int)daysDiff;
        }
    }
}
