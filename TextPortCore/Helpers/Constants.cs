using System;
using System.Collections.Generic;
using System.Text;

namespace TextPortCore.Helpers
{
    public static class Constants
    {
        public static decimal MonthlyNumberRenewalCost = 5.00M;

        //public static decimal BaseNumberDailyCost = 1.00M;

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

        public static class Bandwidth
        {
            public static string UserId = "u-imdmn6chhceskespwwwox7a";

            public static string AccountId = "3000006";

            public static string SubAccountId = "23092";

            public static string ApplicationId = "5abf6fa7-5e0f-4f1c-828b-c01f0c9674c1"; // TextPortV2

            public static string UserName = "richard@arionconsulting.com"; // Use userName and password when retreiving numbers

            public static string ApiToken = "091c02aae3e8dd660fc2f99a328561790da68779e83aabd1";

            public static string ApiSecret = "f015bb36ce195ed94610f2e1489dc3619e47bb5c8ffc37f1";
        }

        public static class IPData
        {
            public static string ApiKey = "3bfcba21299854a74d3d28b160fb119b9a87b142195081d99403ae79";
        }
    }
}