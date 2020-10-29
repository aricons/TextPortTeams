using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Text.RegularExpressions;

using MimeKit;
using RestSharp;
using Talon;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace EmailToSMSGateway
{
    public static class MessageParser
    {

        public static void ParseMessage(string fileName)
        {
            if (File.Exists(fileName))
            {
                EmailToSMSMessage emailToSMSMessage = ParseMimeMessage(fileName);
                WriteLogEntry(fileName, emailToSMSMessage);
            }
        }

        private static EmailToSMSMessage ParseMimeMessage(string fileName)
        {
            EmailToSMSMessage emailToSMSMessage = new EmailToSMSMessage(fileName);

            try
            {
                if (ConfigurationManager.AppSettings["ArchiveInboundFiles"] == "true")
                {
                    emailToSMSMessage.ProcessingLog += (copyFileToArchiveFolder(fileName)) ? "Message file copied to archive folder\r\n" : "Message file copy to archive folder failed\r\n";
                }

                FileInfo fi = new FileInfo(fileName);
                if (fi.Length > Conversion.StringToIntOrZero(ConfigurationManager.AppSettings["MaxMessageFileSize"]))
                {
                    emailToSMSMessage.ProcessingLog += $"Processing stopped. The input file (size {fi.Length}) exceeds the maximum allowable size of {ConfigurationManager.AppSettings["MaxMessageFileSize"]}\r\n";
                    return emailToSMSMessage;
                }

                // Load the message file into a MIME object.
                MimeMessage mimeMsg = MimeMessage.Load(fileName);
                MimeKit.MimeMessage mimeMessage = new MimeMessage();

                // Process and validate the From address.
                if (mimeMsg.From.Count > 0)
                {
                    MailboxAddress fromAddr = mimeMsg.From.Mailboxes.FirstOrDefault();
                    if (fromAddr != null)
                    {
                        emailToSMSMessage.From = fromAddr.Address;
                        emailToSMSMessage.ProcessingLog += $"Sender address {emailToSMSMessage.From} found\r\n";
                    }
                }

                if (string.IsNullOrEmpty(emailToSMSMessage.From))
                {
                    emailToSMSMessage.ProcessingLog += "Processing stopped. Email message is missing a From address\r\n";
                    return emailToSMSMessage;
                }

                // To
                emailToSMSMessage.ProcessingLog += "Looking for To addresses\r\n";
                if (mimeMsg.Headers["X-Rcpt-To"] != "")
                {
                    //emailToSMSMessage.ProcessingLog += $"{mimeMsg.To.Count} To addresses found\r\n";
                    string toAddress = mimeMsg.Headers["X-Rcpt-To"];
                    emailToSMSMessage.ProcessingLog += $"To address '{toAddress}' found\r\n";
                    //foreach (MailboxAddress toAddr in mimeMsg.To.Mailboxes)
                    //{
                    if (!string.IsNullOrEmpty(toAddress))
                    {
                        string mobileNumber = string.Empty;
                        emailToSMSMessage.To += $"{toAddress};";
                        emailToSMSMessage.ProcessingLog += $"Parsing To address {toAddress} for mobile number\r\n";
                        if (getMobileNumberFromToAddress(toAddress, ref mobileNumber))
                        {
                            emailToSMSMessage.ProcessingLog += $"Valid number {mobileNumber} found\r\n";
                            emailToSMSMessage.DestinationNumbers.Add(mobileNumber);
                        }
                    }
                    //}
                }

                // Cc
                emailToSMSMessage.ProcessingLog += $"Looking for Cc addresses\r\n";
                if (mimeMsg.Cc.Count > 0)
                {
                    emailToSMSMessage.ProcessingLog += $"{mimeMsg.Cc.Count} Cc addresses found\r\n";
                    foreach (MailboxAddress ccAddr in mimeMsg.Cc.Mailboxes)
                    {
                        if (!string.IsNullOrEmpty(ccAddr.Address))
                        {
                            string mobileNumber = string.Empty;
                            emailToSMSMessage.To += $"{ccAddr.Address};";
                            emailToSMSMessage.ProcessingLog += $"Parsing Cc address {ccAddr.Address} for mobile number\r\n";
                            if (getMobileNumberFromToAddress(ccAddr.Address, ref mobileNumber))
                            {
                                emailToSMSMessage.ProcessingLog += $"Valid number {mobileNumber} found\r\n";
                                emailToSMSMessage.DestinationNumbers.Add(mobileNumber);
                            }
                        }
                    }
                }

                // Check that there is at least one valid destination number.
                if (emailToSMSMessage.DestinationNumbers.Count == 0)
                {
                    emailToSMSMessage.ProcessingLog += "Processing stopped. No valid destination mobile numbers were located in the To or Cc headers\r\n";
                    return emailToSMSMessage;
                }

                // Extract the SMS message from the email body. 
                // Stop parsing if any lines start with "====" or "++++"
                if (getMessageTextFromEmailBody(mimeMsg, emailToSMSMessage))
                {
                    emailToSMSMessage.ProcessingLog += $"Message body detected.\r\nMessage text:\r\n{emailToSMSMessage.MessageText}\r\n";
                    emailToSMSMessage.IsValid = true;
                }
                else
                {
                    emailToSMSMessage.ProcessingLog += "Processing stopped\r\nAn empty or unreadable message text body was encountered\r\n";
                    emailToSMSMessage.IsValid = false;
                    return emailToSMSMessage;
                }

                // Verify that the sender address is registered.
                emailToSMSMessage.ProcessingLog += $"Checking registration status for sender address '{emailToSMSMessage.From}'\r\n";
                using (TextPortDA da = new TextPortDA())
                {
                    EmailToSMSAddress emailToSmsAddr = da.GetEmailToSMSAddressByEmailAddress(emailToSMSMessage.From);
                    if (emailToSmsAddr != null)
                    {
                        emailToSMSMessage.AccountId = emailToSmsAddr.AccountId;
                        emailToSMSMessage.VirtualNumberId = emailToSmsAddr.VirtualNumberId;
                        emailToSMSMessage.ProcessingLog += $"Sender address {emailToSMSMessage.From} is a registered sender address.\r\nAccount ID is {emailToSMSMessage.AccountId}\r\nVirtual number ID is {emailToSMSMessage.VirtualNumberId}\r\n";
                        emailToSMSMessage.AddressId = emailToSmsAddr.AddressId;

                        // Check that the virtual number is valid and active.
                        emailToSMSMessage.ProcessingLog += $"Checking active status of virtual number ID {emailToSMSMessage.VirtualNumberId}\r\n";
                        DedicatedVirtualNumber dvn = da.GetVirtualNumberById(emailToSMSMessage.VirtualNumberId);
                        if (dvn != null)
                        {
                            if (!dvn.Cancelled)
                            {
                                emailToSMSMessage.ProcessingLog += $"An active virtual number {dvn.VirtualNumber} was found for sender {emailToSMSMessage.From} and account ID {emailToSMSMessage.AccountId}.\r\n";
                            }
                            else
                            {
                                emailToSMSMessage.ProcessingLog += $"A cancelled virtual number {dvn.VirtualNumber} was found for sender {emailToSMSMessage.From} and account ID {emailToSMSMessage.AccountId}. Processing ends.\r\n";
                                return emailToSMSMessage;
                            }
                        }
                        else
                        {
                            emailToSMSMessage.ProcessingLog += $"No virtual number with ID {emailToSMSMessage.VirtualNumberId} was found. Processing ends.\r\n";
                            return emailToSMSMessage;
                        }
                    }
                    else
                    {
                        emailToSMSMessage.ProcessingLog += "Processing stopped\r\nThe sender address is not a registered TextPort Email-to-SMS Gateway address.\r\n";
                        return emailToSMSMessage;
                    }

                    emailToSMSMessage.ProcessingLog += $"All checks passed. The message is clear to send (CTS) to {emailToSMSMessage.DestinationNumbers.Count} destination numbers.\r\n";
                    foreach (string destinationNumber in emailToSMSMessage.DestinationNumbers)
                    {
                        emailToSMSMessage.ProcessingLog += $"Queuing message to {destinationNumber}.\r\n";

                        Message message = new Message(emailToSMSMessage, destinationNumber);

                        decimal newBalance = 0;
                        da.InsertMessage(message, ref newBalance);
                        if (requestSemaphoreWrite(message))
                        {
                            emailToSMSMessage.ProcessingLog += $"Message queued successfully. Message ID is {message.MessageId}.\r\n";
                        }
                        else
                        {
                            emailToSMSMessage.ProcessingLog += $"Message queuing failed. Error mesage: {message.ProcessingMessage}.\r\n";
                        }
                    }
                }

                emailToSMSMessage.ProcessingLog += $"Message queuing complete.\r\nEmail to SMS gateway processing of file {fileName} complete.";
            }
            catch (Exception ex)
            {
                emailToSMSMessage.MessageText += $"An error occurred while processing message file {fileName}. Error message {ex.ToString()}\r\n";
                emailToSMSMessage.ProcessingLog += $"An error occurred while processing message file {fileName}. Error message {ex.ToString()}\r\n";
            }

            emailToSMSMessage.MessageText += deleteMessageFile(fileName);

            return emailToSMSMessage;
        }

        private static bool requestSemaphoreWrite(Message message)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["SemaphoreURL"]);
            var request = new RestRequest(Method.POST);

            request.Resource = "messages/WriteEmailToMMSSemaphore";
            request.RequestFormat = DataFormat.Json;

            request.AddJsonBody(message);

            IRestResponse response = client.Execute(request);

            if (response != null)
            {
                if (!string.IsNullOrEmpty(response.Content))
                {
                    return (response.Content.Contains("true"));
                }
            }

            return false;
        }

        private static string deleteMessageFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
                return "Message file deleted";
            }
            catch (Exception ex)
            {
                return "Error deleting message file: " + ex.Message;
            }
        }

        private static bool getMessageTextFromEmailBody(MimeMessage mimeMsg, EmailToSMSMessage message)
        {
            try
            {
                string textBody = mimeMsg.GetTextBody(MimeKit.Text.TextFormat.Plain);

                // Strip any signatures
                textBody = stripSignaturesFromMessage(textBody);

                StringReader sr = new StringReader(textBody);
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("====") || line.StartsWith("++++"))
                    {
                        break;
                    }
                    message.MessageText += line; // + "\r\n";

                    // Check for excessively long messages. Limit to 4 segments.
                    if (message.MessageText.Length > Constants.MaximumEmailToSMSMessageLength)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                message.ProcessingLog += $"An error occurred while parsing the email body for a text message. Error: {ex.Message}. Exception: {ex.InnerException}.";
            }

            // Strip any blank lines
            message.MessageText = message.MessageText.Replace("\r\n\r\n", "\r\n");

            return !string.IsNullOrEmpty(message.MessageText);
        }

        private static bool getMobileNumberFromToAddress(string address, ref string fromNumber)
        {
            string namePart = string.Empty;
            string domainPart = string.Empty;

            splitEmailAddress(address, ref namePart, ref domainPart);

            string regexEmailAddress = @"^[0-9][0-9][0-9][0-9][0-9][0-9][0-9]";
            Regex rx = new Regex(regexEmailAddress, RegexOptions.IgnoreCase);
            Match m = rx.Match(namePart);
            if (m.Success)
            {
                fromNumber = NumberToE164Temp(namePart, "1");
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string NumberToE164Temp(string number, string countryCode)
        {
            string globalNumber = string.Empty;

            try
            {
                globalNumber = $"{countryCode}{Regex.Replace(number, @"\D", "")}";
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

        private static string extractUserNameFromToAddress(string address)
        {
            string namePart = string.Empty;
            string domainPart = string.Empty;

            splitEmailAddress(address, ref namePart, ref domainPart);

            return namePart;
        }

        private static bool splitEmailAddress(string address, ref string namePart, ref string domainPart)
        {
            namePart = string.Empty;
            domainPart = string.Empty;

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

        private static string stripSignaturesFromMessage(string messageText)
        {
            string strippedMessage = messageText;

            Tuple<string, string> signature = Bruteforce.ExtractSignature(messageText);

            if (signature.Item2 != null)
            {
                strippedMessage = signature.Item1; // The body part only, if the parse worked.
                // signature.Item2 is the signature section.
            }

            return strippedMessage;
        }

        //private static string parseMIMEPartsForText(Chilkat.Mime mimePart)
        //{
        //    // Recursive routine to search MIME messages for all plain-text parts.
        //    string outString = string.Empty;

        //    if (mimePart.NumParts == 0 && mimePart.IsPlainText())
        //    {
        //        outString = mimePart.GetBodyDecoded().Trim();
        //    }
        //    else if (mimePart.NumParts > 0)
        //    {
        //        for (int subPartCount = 0; subPartCount <= mimePart.NumParts - 1; subPartCount++)
        //        {
        //            Chilkat.Mime mimeSubPart = mimePart.GetPart(subPartCount);
        //            outString += parseMIMEPartsForText(mimeSubPart);
        //        }
        //    }
        //    else
        //    {
        //        outString = stripTrailersFromMessage(mimePart.GetBodyDecoded().Trim());
        //    }
        //    outString = outString.TrimEnd('\r', '\n');
        //    return outString;
        //}

        private static bool copyFileToArchiveFolder(string fileName)
        {
            try
            {
                string archiveFolder = ConfigurationManager.AppSettings["ArchiveFolder"];
                if (!String.IsNullOrEmpty(archiveFolder))
                {
                    string fileNameOnly = fileName.Substring(fileName.LastIndexOf(@"\") + 1).Replace(@"\", "").Trim();
                    File.Copy(fileName, archiveFolder + fileNameOnly, true);

                    return (File.Exists(archiveFolder + fileNameOnly));
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string stripTrailersFromMessage(string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    string testString = message.ToLower();

                    if (testString.Contains("original message"))
                    {
                        return message.Substring(0, testString.IndexOf("original message")).Replace("-----", "");
                    }
                }
                return message;
            }
            catch (Exception)
            {
                return message;
            }
        }

        public static void WriteLogEntry(string fileName, EmailToSMSMessage message)
        {
            try
            {
                using (StreamWriter w = File.AppendText($"{ConfigurationManager.AppSettings["LogFolder"]}Log.txt"))
                {
                    Log(message.ProcessingLog, w);
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                WriteDebugEntry($"Exception in WriteLogEntry: {ex.Message}");
            }
        }

        public static void WriteDebugEntry(string text)
        {
            try
            {
                using (StreamWriter w = File.AppendText($"{ConfigurationManager.AppSettings["LogFolder"]}ErrorLog.txt"))
                {
                    DebugLog(text, w);
                    w.Close();
                }
            }
            catch (Exception)
            {
                // In case the file is already in use and cannot be opened or written to.
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("TextPort Email-to-SMS Gateway Processing Agent Log Entry: ");
            w.WriteLine("{0} on {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            w.WriteLine("{0}", logMessage);
            w.WriteLine("====================================================================================================\r\n");
            w.Flush();
        }

        public static void DebugLog(string debugMessage, TextWriter w)
        {
            w.Write("TextPort Email-to-SMS Gateway Agent Debug Log Entry: ");
            w.WriteLine("{0} on {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            w.WriteLine("{0}", debugMessage);
            w.WriteLine("====================================================================================================\r\n");
            w.Flush();
        }
    }
}