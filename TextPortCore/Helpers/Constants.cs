using System;
using System.Collections.Generic;
using System.Text;

namespace TextPortCore.Helpers
{
    public static class Constants
    {
        public static decimal MonthlyNumberRenewalCost = 5.00M;

        public static decimal BaseSMSSegmentCost = 0.015M;

        public static decimal BaseMMSSegmentCost = 0.025M;

        public static decimal InitialBalanceAllocation = 0.50M; // Aproximately 30 messages @ $0.015 each.

        public static decimal InitialFreeTrialBalanceAllocation = 0.15M; // 20 messages @ $0.015 each.

        public static decimal BaseSMSMessageCharge = 0.004M;

        public static decimal BaseMMSMessageCharge = 0.015M;

        public static decimal Free = 0M;

        public static string RC4Key = "tH33nCrYpT10nK3y";

        public static int NumberOfNumbersToPullFromBandwidth = 20;

        public static int NumberOfNumbersToPullFromBandwidthForAPI = 100;

        public static int MaximumEmailToSMSMessageLength = 640; // 4 Segments.

        public static int MinutesInDay = 1440; // 60 * 24

        public static int AccountIdScramblerOffset = 1967;

        public static class IPData
        {
            public static string ApiKey = "3bfcba21299854a74d3d28b160fb119b9a87b142195081d99403ae79";
        }
    }
}