﻿using System;

namespace TextPortCore.Helpers
{
    public static class TimeFunctions
    {


        public static DateTime GetUsersLocalTime(DateTime messageTimeUtc, int timeZoneId)
        {
            TimeZoneInfo usersTimeZone = TimeZoneInfo.FindSystemTimeZoneById(GetTimeZoneNameTimeZoneId(timeZoneId));
            DateTimeOffset dtOffset = TimeZoneInfo.ConvertTime(messageTimeUtc, TimeZoneInfo.Utc, usersTimeZone);
            return dtOffset.DateTime;
        }


//        select* from TimeZones


//Dateline Standard Time	-12
//UTC-11	-11
//Aleutian Standard Time	-10
//Hawaiian Standard Time	-10
//Marquesas Standard Time	-9
//Alaskan Standard Time	-9
//UTC-09	-9
//Pacific Standard Time(Mexico)  -8
//UTC-08	-8
//Pacific Standard Time	-8
//US Mountain Standard Time	-7
//Mountain Standard Time(Mexico) -7
//Mountain Standard Time	-7
//Central America Standard Time	-6
//Central Standard Time	-6
//Easter Island Standard Time	-6
//Central Standard Time(Mexico)  -6
//Canada Central Standard Time	-6
//SA Pacific Standard Time	-5
//Eastern Standard Time(Mexico)  -5
//Eastern Standard Time	-5
//Haiti Standard Time	-5
//Cuba Standard Time	-5
//US Eastern Standard Time	-5
//Turks And Caicos Standard Time	-5
//Paraguay Standard Time	-4
//Atlantic Standard Time	-4
//Venezuela Standard Time	-4
//Central Brazilian Standard Time	-4
//SA Western Standard Time	-4
//Pacific SA Standard Time	-4
//Newfoundland Standard Time	-3
//Tocantins Standard Time	-3
//E.South America Standard Time	-3
//SA Eastern Standard Time	-3
//Argentina Standard Time	-3
//Greenland Standard Time	-3
//Montevideo Standard Time	-3
//Magallanes Standard Time	-3
//Saint Pierre Standard Time	-3
//Bahia Standard Time	-3
//UTC-02	-2
//Mid-Atlantic Standard Time	-2
//Azores Standard Time	-1
//Cape Verde Standard Time	-1
//UTC	0
//GMT Standard Time	0
//Greenwich Standard Time	0
//Sao Tome Standard Time	0
//Morocco Standard Time	0
//W.Europe Standard Time	1
//Central Europe Standard Time	1
//Romance Standard Time	1
//Central European Standard Time	1
//W.Central Africa Standard Time	1
//Jordan Standard Time	2
//GTB Standard Time	2
//Middle East Standard Time	2
//Egypt Standard Time	2
//E.Europe Standard Time	2
//Syria Standard Time	2
//West Bank Standard Time	2
//South Africa Standard Time	2
//FLE Standard Time	2
//Israel Standard Time	2
//Kaliningrad Standard Time	2
//Sudan Standard Time	2
//Libya Standard Time	2
//Namibia Standard Time	2
//Arabic Standard Time	3
//Turkey Standard Time	3
//Arab Standard Time	3
//Belarus Standard Time	3
//Russian Standard Time	3
//E.Africa Standard Time	3
//Iran Standard Time	3
//Arabian Standard Time	4
//Astrakhan Standard Time	4
//Azerbaijan Standard Time	4
//Russia Time Zone 3	4
//Mauritius Standard Time	4
//Saratov Standard Time	4
//Georgian Standard Time	4
//Volgograd Standard Time	4
//Caucasus Standard Time	4
//Afghanistan Standard Time	4
//West Asia Standard Time	5
//Ekaterinburg Standard Time	5
//Pakistan Standard Time	5
//Qyzylorda Standard Time	5
//India Standard Time	5
//Sri Lanka Standard Time	5
//Nepal Standard Time	5
//Central Asia Standard Time	6
//Bangladesh Standard Time	6
//Omsk Standard Time	6
//Myanmar Standard Time	6
//SE Asia Standard Time	7
//Altai Standard Time	7
//W.Mongolia Standard Time	7
//North Asia Standard Time	7
//N.Central Asia Standard Time	7
//Tomsk Standard Time	7
//China Standard Time	8
//North Asia East Standard Time	8
//Singapore Standard Time	8
//W.Australia Standard Time	8
//Taipei Standard Time	8
//Ulaanbaatar Standard Time	8
//Aus Central W.Standard Time	8
//Transbaikal Standard Time	9
//Tokyo Standard Time	9
//North Korea Standard Time	9
//Korea Standard Time	9
//Yakutsk Standard Time	9
//Cen.Australia Standard Time	9
//AUS Central Standard Time	9
//E.Australia Standard Time	10
//AUS Eastern Standard Time	10
//West Pacific Standard Time	10
//Tasmania Standard Time	10
//Vladivostok Standard Time	10
//Lord Howe Standard Time	10
//Bougainville Standard Time	11
//Russia Time Zone 10	11
//Magadan Standard Time	11
//Norfolk Standard Time	11
//Sakhalin Standard Time	11
//Central Pacific Standard Time	11
//Russia Time Zone 11	12
//New Zealand Standard Time	12
//UTC+12	12
//Fiji Standard Time	12
//Kamchatka Standard Time	12
//Chatham Islands Standard Time	12
//UTC+13	13
//Tonga Standard Time	13
//Samoa Standard Time	13
//Line Islands Standard Time	14

        public static string GetTimeZoneNameTimeZoneId(int timeZoneId)
        {
            switch (timeZoneId)
            {
                case 1:
                    return "Dateline Standard Time";
                case 2:
                    return "UTC-11";
                case 3:
                    return "Hawaiian Standard Time";
                case 4:
                    return "Alaskan Standard Time";
                case 5:
                    return "Pacific Standard Time";
                case 6:
                case 7:
                case 8:
                    return "Mountain Standard Time";
                case 9:
                    return "Central America Standard Time";
                case 10:
                    return "Central Standard Time";
                case 11:
                    return "Central Standard Time (Mexico)";
                case 12:
                    return "Canada Central Standard Time";
                case 13:
                    return "SA Pacific Standard Time";
                case 14:
                case 15:
                    return "Eastern Standard Time";
                case 16:
                    return "Atlantic Standard Time";
                case 17:
                    return "Venezuela Standard Time";
                case 18:
                    return "Central Brazilian Standard Time";
                case 19:
                    return "Newfoundland Standard Time";
                case 20:
                    return "E. South America Standard Time";
                case 21:
                    return "Argentina Standard Time";
                case 22:
                    return "Greenland Standard Time";
                case 23:
                    return "Mid-Atlantic Standard Time";
                case 24:
                    return "Azores Standard Time";
                case 25:
                    return "Cape Verde Standard Time";
                case 26:
                case 27:
                    return "Greenwich Standard Time";
                case 28:
                case 29:
                case 30:
                case 31:
                    return "Central Europe Standard Time";
                case 32:
                    return "W. Central Africa Standard Time";
                case 33:
                    return "TIME";
                case 34:
                    return "TIME";
                case 35:
                    return "TIME";
                case 36:
                    return "TIME";
                case 37:
                    return "TIME";
                case 38:
                    return "TIME";
                case 39:
                    return "TIME";
                case 40:
                    return "TIME";
                case 41:
                    return "TIME";
                case 42:
                    return "TIME";
                case 43:
                    return "TIME";
                case 44:
                    return "TIME";
                case 45:
                    return "TIME";
                case 46:
                    return "TIME";
                case 47:
                    return "TIME";
                case 48:
                    return "TIME";
                case 49:
                    return "TIME";
                case 50:
                    return "TIME";
                default:
                    return "UTC";
            }
        }
    }
}
