using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class EmailMessage : IDisposable
    {
        public string To { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        
        // Constructors
        public EmailMessage()
        {
            this.From = ConfigurationManager.AppSettings["EmailFrom"].ToString();
            this.FromName = "TextPort Support";
            this.To = string.Empty;
            this.Subject = string.Empty;
            this.Body = string.Empty;
        }

        public EmailMessage(string toAddress, string subject, string body)
        {
            this.From = ConfigurationManager.AppSettings["EmailFrom"].ToString();
            this.FromName = "TextPort Notifications";
            this.To = toAddress;
            this.Subject = subject;
            this.Body = body;
        }


        // Public methods
        public bool Send()
        {
            return Send(true);
        }

        public bool Send(bool isBodyHtml)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(this.From, this.FromName);
            message.To.Add(this.To);
          
            //For monitoring

            string bccAddress = ConfigurationManager.AppSettings["EmailNotificationsBCCAddress"];
            if (!string.IsNullOrEmpty(bccAddress))
            {
                message.Bcc.Add(bccAddress);
            }
            //message.Bcc.Add("rdegley@gmail.com");

            if (!string.IsNullOrEmpty(this.ReplyTo))
            {
                message.ReplyToList.Add(new MailAddress(this.ReplyTo, this.FromName));
            }

            message.Subject = this.Subject;
            message.IsBodyHtml = isBodyHtml;
            message.Body = this.Body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = true;

            smtpClient.Host = ConfigurationManager.AppSettings["SMTPServer"];
            smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPEnableSSL"]);
            smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTPUserName"], ConfigurationManager.AppSettings["SMTPPassword"]);
            try
            {
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("EmailMessage.Send", ex);
            }

            return false;
        }

        #region "Disposal"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
        }

        #endregion
    }
}
