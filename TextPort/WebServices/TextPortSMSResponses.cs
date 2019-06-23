using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPort.WebServices
{
    public class TextPortSMSResponses
    {
        public List<TextPortSMSResponse> Responses { get; set; }
    }

    public class TextPortSMSResponse
    {
        private int itemNumber;
        private string mobileNumber;
        private string result;
        private int messageID;
        private string processingMessage;
        private string errorMessage;

        public int ItemNumber
        {
            get
            {
                return this.itemNumber;
            }
            set
            {
                this.itemNumber = value;
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

        public string Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
            }
        }

        public int MessageID
        {
            get
            {
                return this.messageID;
            }
            set
            {
                this.messageID = value;
            }
        }

        public string ProcessingMessage
        {
            get
            {
                return this.processingMessage;
            }
            set
            {
                this.processingMessage = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            set
            {
                this.errorMessage = value;
            }
        }
    }
}