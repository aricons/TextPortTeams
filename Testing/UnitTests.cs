using System;
//using System.Web;
//using System.Web.Mvc;
using System.Configuration;
using System.Web.Script.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortServices.Processes;


namespace Testing
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void ProcessOutboundMessage()
        {
            int messageId = 2933866;

            var optionsBuilder = new DbContextOptionsBuilder<TextPortContext>();
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            TextPortContext context = new TextPortContext(optionsBuilder.Options);

            using (TextPortDA da = new TextPortDA(context))
            {
                Message message = da.GetMessageById(messageId);

                if (message.MessageId > 0)
                {
                    Communications comms = new Communications(context);
                    if (comms.GenerateAndSendMessage(message))
                    {
                        message.ProcessingMessage += " Comms OK. ";
                        message.QueueStatus = 1;
                    }
                    else
                    {
                        message.ProcessingMessage += " Comms Failed. GenerateAndSendMessage failed. ";
                        message.QueueStatus = 2;
                    }
                }
            }
        }

        [TestMethod]
        public void TestRC4Decryption()
        {
            string decryptedPassword = RC4.Decrypt("McKtwp3Ct8O8FQ==", Constants.RC4Key);
            string foo = decryptedPassword;

            // McKtwp3Ct8O8FQ==
        }

        [TestMethod]
        public void TestAESDecryption()
        {
            string decryptedPassword = AESEncryptDecrypt.Decrypt("1­·ü", Constants.RC4Key);
            string foo = decryptedPassword;

            // McKtwp3Ct8O8FQ==
        }

        [TestMethod]
        public void GetBandwidthNumbersForAreaCode()
        {
            string areaCode = "949";
            //List<string> numbers = Bandwidth.GetVirtualNumbersList(areaCode);
            //var foo = numbers;
        }

        [TestMethod]
        public void TestPayPalGetAuthToken()
        {
            //PayPal.GetAuthToken();
        }

        //[TestMethod]
        //public void SubmitPayPalPayment()
        //{
        //    PurchaseDetail pd = new PurchaseDetail();

        //    pd.intent = "sale";
        //    pd.redirect_urls = new RedirectUrls()
        //    {
        //        cancel_url = "textport.com",
        //        return_url = "textport.com"
        //    };
        //}

        [TestMethod]
        public void GetRecentContacts()
        {
            //List<Recent> recents = MessagesDA.GetRecentMessagesForAccount(1);
            //var bar = recents;
        }

        [TestMethod]
        public void GetRandomNumberString()
        {
            for (int x = 0; x <= 20; x++)
            {
                Console.WriteLine(RandomString.RandomNumberString());
            }
        }

        [TestMethod]
        public void GetUniqueToken()
        {
            for (int x = 0; x <= 10; x++)
            {
                Console.WriteLine(RandomString.GenerateRandomToken(30));
            }
        }

        [TestMethod]
        public void StringifyMessageToJSON()
        {
            try
            {
                Message msg = new Message()
                {
                    AccountId = 1,
                    CarrierId = 172,
                    MessageText = "Test message text",
                    MobileNumber = "19492339386",
                    VirtualNumberId = 1895,
                    Ipaddress = "200.1.1.1",
                    Direction = 0
                };

                msg.MMSFiles = new System.Collections.Generic.List<MMSFile>();
                msg.MMSFiles.Add(new MMSFile()
                {
                    FileName = "File1.jpg",
                    //DataBytes = null
                });
                msg.MMSFiles.Add(new MMSFile()
                {
                    FileName = "File2.jpg",
                    //DataBytes = null
                });

                string json = new JavaScriptSerializer().Serialize(msg);
                string foo = json;
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
        }

        [TestMethod]
        public void NumberFormattingTests()
        {
            string number = "19492339386";
            //string number = "(949) 233-9386";

            DedicatedVirtualNumber num = new DedicatedVirtualNumber()
            {
                VirtualNumber = number
            };

            string foo = num.ToLocalFormat();

            string bar = foo;
        }
    }
}