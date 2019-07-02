using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing
{
    [TestClass]
    public class WebServiceTests
    {
        [TestMethod]
        public void TestProductionASMX()
        {
            ProductionASMX.SMSClientSoapClient client = new ProductionASMX.SMSClientSoapClient();

            ProductionASMX.TextPortSMSMessage message = new ProductionASMX.TextPortSMSMessage();
            message.CountryCode = "US";
            message.MessageText = "Test Messge from ASMX service";
            message.MobileNumber = "9492339386";

            List<ProductionASMX.TextPortSMSMessage> messageList = new List<ProductionASMX.TextPortSMSMessage>();
            messageList.Add(message);

            ProductionASMX.TextPortSMSMessages messages = new ProductionASMX.TextPortSMSMessages();
            messages.UserName = "regley";
            messages.Password = "Zealand!4";
            messages.Messages = messageList;

            ProductionASMX.TextPortSMSResponses responses = client.SendMessages(messages);

            var foo = responses;
        }

        [TestMethod]
        public void TestProductionSVC()
        {
            try
            {
                ProductionSVC.TextPortSMSClient client = new ProductionSVC.TextPortSMSClient();

                ProductionSVC.TextPortSMSMessage message = new ProductionSVC.TextPortSMSMessage();
                message.CountryCode = "US";
                message.MessageText = "Test Messge from SVC service";
                message.MobileNumber = "9492339386";

                List<ProductionSVC.TextPortSMSMessage> messageList = new List<ProductionSVC.TextPortSMSMessage>();
                messageList.Add(message);

                ProductionSVC.TextPortSMSMessages messages = new ProductionSVC.TextPortSMSMessages();
                messages.UserName = "regley";
                messages.Password = "Zealand!4";
                messages.Messages = messageList;

                ProductionSVC.TextPortSMSResponses responses = client.SendMessages(messages);

                var foo = responses;
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
        }
    }
}
