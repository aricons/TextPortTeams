using System;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;

namespace TextPortCore.Helpers
{
    public static class Utilities
    {
        public static int GetAccountIdFromClaim(ClaimsPrincipal claim)
        {
            if (claim != null)
            {
                string accountIdStr = claim.FindFirst("AccountId").Value;
                return (!string.IsNullOrEmpty(accountIdStr)) ? Convert.ToInt32(accountIdStr) : 0;
            }

            return 0;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static bool IsValidNumber(string number)
        {
            if (string.IsNullOrEmpty(number) || string.IsNullOrWhiteSpace(number))
                return false;

            try
            {
                number = Utilities.NumberToE164(number, "1");

                return Regex.IsMatch(number, @"^[1][0-9]{10}$", RegexOptions.IgnoreCase);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static string StripNumber(string mobileNumber) //, ref string processingMessage, ref int status)
        {
            string strippedNumber = String.Empty;

            try
            {
                strippedNumber = Regex.Replace(mobileNumber, @"\D", "");
                //processingMessage += "Sending to |" + strippedNumber + "|. ";
            }
            catch (Exception ex)
            {
                //processingMessage += "Exception in StripMobileNumber: " + ex.Message + ". ";
                //status = 1;
                strippedNumber = "Parsing Error";
            }
            return strippedNumber;
        }

        public static string StripLeading1(string number)
        {
            string strippedNumber = number;

            if (!string.IsNullOrEmpty(number))
            {
                if (number.Length == 11 && number.Substring(0, 1) == "1")
                {
                    strippedNumber = number.Substring(1);
                }
            }
            return strippedNumber;
        }

        public static string NumberToE164(string number, string countryCode)
        {
            string globalNumber = string.Empty;

            try
            {
                number = Regex.Replace(number, @"\D", "");
                if (countryCode == "1")
                {
                    if (!number.StartsWith("1"))
                    {
                        globalNumber = $"1{number}";
                    }
                    else
                    {
                        globalNumber = number;
                    }
                }
                else
                {
                    globalNumber = $"{countryCode}{number}";
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
            return globalNumber;
        }

        public static string NumberToDisplayFormat(string number, int countryId)
        {
            string localNumber = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(number))
                {
                    localNumber = Regex.Replace(number, @"\D", "");
                    switch (countryId)
                    {
                        case (int)Countries.UnitedStates:
                            return $"+1 {localNumber.Substring(1, 3)}-{localNumber.Substring(4, 3)}-{localNumber.Substring(7)}";

                        case (int)Countries.UnitedKingdom:
                            return $"+{localNumber.Substring(0, 2)} {localNumber.Substring(2, 4)} {localNumber.Substring(6)}";

                        case (int)Countries.Australia: // 2-digit country codes
                        case (int)Countries.Germany:
                        case (int)Countries.India:
                        case (int)Countries.NewZealand:
                            return $"+{localNumber.Substring(0, 2)} {localNumber.Substring(2)}";

                        default:
                            return $"+{localNumber}";
                    }
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
            return localNumber;
        }

        public static string NumberToBandwidthFormat(string number)
        {
            string bwNumber = string.Empty;

            try
            {
                bwNumber = Regex.Replace(number, @"\D", "");
                if (bwNumber.StartsWith("1"))
                {
                    bwNumber = bwNumber.Substring(1);
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
            return bwNumber;
        }

        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        public static int GetSegmentCount(string messageText)
        {
            if (!string.IsNullOrEmpty(messageText))
            {
                int messageLength = messageText.Length;

                if (messageLength <= 160)
                {
                    return 1;
                }
                if (messageLength <= 306)
                {
                    return 2;
                }
                if (messageLength <= 459)
                {
                    return 3;
                }
                if (messageLength <= 612)
                {
                    return 4;
                }
                if (messageLength <= 765)
                {
                    return 5;
                }
                if (messageLength <= 918)
                {
                    return 6;
                }
                if (messageLength <= 1071)
                {
                    return 7;
                }
                if (messageLength <= 1224)
                {
                    return 8;
                }
                if (messageLength <= 1377)
                {
                    return 9;
                }
                if (messageLength <= 1530)
                {
                    return 10;
                }
            };

            return 1;
        }

        public static bool WriteEventLogEntry(string eventMessage, EventLogEntryType eventType)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEntry("Textport Communications", eventMessage, eventType);

                return true;
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
                return false;
            }
        }

        public static bool SendMessageToSMTPServer(MailMessage emailMessage, string smtpHostName)
        {
            try
            {
                SmtpClient smtp = new SmtpClient(smtpHostName);
                smtp.Send(emailMessage);
                smtp.Dispose(); // Need to do a manual dispose to ensue that the QUIT command is sent to the remote server.
                return true;
            }
            catch (Exception ex)
            {
                //Common.WriteEventLogEntry("An error occurred in Common.SendMessageToSMTPServer. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
        }

        public static string SerializeToXml<T>(T objectToSerialize)
        {
            var serializer = new XmlSerializer(typeof(T));
            //StringWriter textWriter = new Utf8StringWriter();

            //serializer.Serialize(textWriter, objectToSerialize);
            //return textWriter.ToString();

            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (XmlWriter stringWriter = XmlWriter.Create(builder, settings))
            {
                serializer.Serialize(stringWriter, objectToSerialize, ns);
                return builder.ToString();
            }
        }

        public static string GetUserHostAddress()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request != null)
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
            }

            return "0.0.0.0";
        }

        public static bool SplitEmailAddress(string address, ref string namePart, ref string domainPart)
        {
            namePart = String.Empty;
            domainPart = String.Empty;

            try
            {
                if (address.IndexOf("@") > 0)
                {
                    char[] atSign = new char[] { '@' };
                    string[] addressSegments = address.Split(atSign);
                    if (addressSegments.Length >= 2)
                    {
                        namePart = addressSegments[0].Trim();
                        domainPart = addressSegments[1].Trim();
                    }
                    return true;
                }
                else
                {
                    namePart = address;
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public static void DeleteEventLogFromSystem(string sourceName)
        //{
        //    string logName;

        //    try
        //    {
        //        if (EventLog.SourceExists(sourceName))
        //        {

        //            logName = EventLog.LogNameFromSourceName(sourceName, ".");
        //            // Delete the source and the log.
        //            EventLog.Delete(logName);
        //            EventLog.DeleteEventSource(sourceName);

        //            Console.WriteLine(logName + " deleted.");
        //        }
        //        else
        //        {
        //            // Create the event source to make next try successful.
        //            //EventLog.CreateEventSource(sourceName, sourceName+ "Log");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;
        //    }
        //}

        public static string CrLf(int timesToRepeat)
        {
            string output = String.Empty;

            for (int x = 0; x < timesToRepeat; x++)
            {
                output += "\r\n";
            }
            return output;
        }
    }
}
