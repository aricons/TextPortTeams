using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class EmailMessage : IDisposable
    {
        private string from;
        private string fromName;
        private string to;
        private string subject;
        private string body;

        public string To
        {
            get { return this.to; }
            set { this.to = value; }
        }

        public string From
        {
            get { return this.from; }
            set { this.from = value; }
        }

        public string FromName
        {
            get { return this.fromName; }
            set { this.fromName = value; }
        }

        public string Subject
        {
            get { return this.subject; }
            set { this.subject = value; }
        }

        public string Body
        {
            get { return this.body; }
            set { this.body = value; }
        }

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
