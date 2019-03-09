﻿using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net.Mail;

namespace TextPortCore.Helpers
{
    public static class Utilities
    {
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
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
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

        public static string StripMobileNumber(string mobileNumber, ref string processingMessage, ref int status)
        {
            string strippedNumber = String.Empty;

            try
            {
                strippedNumber = Regex.Replace(mobileNumber, @"\D", "");
                processingMessage += "Sending to |" + strippedNumber + "|. ";
            }
            catch (Exception ex)
            {
                processingMessage += "Exception in StripMobileNumber: " + ex.Message + ". ";
                status = 1;
            }
            return strippedNumber;
        }

        public static string NumberToGlobalFormat(string number)
        {
            string globalNumber = string.Empty;

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
            return globalNumber;
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
