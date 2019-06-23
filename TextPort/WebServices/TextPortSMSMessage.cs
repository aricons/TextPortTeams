using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPort.WebServices
{
    public class TextPortSMSMessages
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public List<TextPortSMSMessage> Messages { get; set; }
    }

    public class TextPortSMSMessage
    {
        private string countryCode;
        private string mobileNumber;
        private string messageText;

        public string CountryCode
        {
            get
            {
                return this.countryCode;
            }
            set
            {
                this.countryCode = value;
            }
        }

        public string MobileNumber
        {
            get
            {
                return this.mobileNumber;
            }
            set
            {
                this.mobileNumber = value;
            }
        }

        public string MessageText
        {
            get
            {
                return this.messageText;
            }
            set
            {
                this.messageText = value;
            }
        }
    }
}