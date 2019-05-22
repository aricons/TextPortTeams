using System;
//using System.Web;
//using System.Web.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;
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
            int messageId = 10160214;

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
            var optionsBuilder = new DbContextOptionsBuilder<TextPortContext>();
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            TextPortContext context = new TextPortContext(optionsBuilder.Options);

            string areaCode = "949";

            using (Bandwidth bw = new Bandwidth(context))
            {
                List<string> numbers = bw.GetVirtualNumbersList(areaCode);
                var foo = numbers;
            }
        }

        [TestMethod]
        public void BandwidthPurchaseNumber()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TextPortContext>();
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            TextPortContext context = new TextPortContext(optionsBuilder.Options);

            RegistrationData regData = new RegistrationData()
            {
                //VirtualNumber = "9495551212",
                //VirtualNumber = "9496880745",
                //VirtualNumber = "9495035607",
                //VirtualNumber = "9095052389",
                VirtualNumber = "8122692012",
                AccountId = 1
            };

            using (Bandwidth bw = new Bandwidth(context))
            {
                bool foo = bw.PurchaseVirtualNumber(regData);
                bool bar = foo;
            }
        }

        [TestMethod]
        public void BandwidthDisconnectNumber()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TextPortContext>();
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            TextPortContext context = new TextPortContext(optionsBuilder.Options);
            string processingMessage = string.Empty;

            DedicatedVirtualNumber number = new DedicatedVirtualNumber()
            {
                AccountId = 1,
                VirtualNumber = "19095052389"
            };

            using (Bandwidth bw = new Bandwidth(context))
            {
                bool foo = bw.DisconnectVirtualNumber(number, ref processingMessage);
                bool bar = foo;
                string msg = processingMessage;
            }
        }

        [TestMethod]
        public void BandwidthCheckOrderStatus()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TextPortContext>();
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            TextPortContext context = new TextPortContext(optionsBuilder.Options);

            //string orderId = "f54817fd-6fb4-4573-9463-78e09d65b47a"; // Complete
            string orderId = "39bc2d9c-0d29-4dbe-996f-3123d5c10d85"; // Failed

            using (Bandwidth bw = new Bandwidth(context))
            {
                string errorMessage = string.Empty;
                string foo = bw.CheckOrderStatus(orderId, ref errorMessage);
                string bar = foo;
            }
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

            string foo = num.NumberDisplayFormat;

            string bar = foo;
        }
    }
}