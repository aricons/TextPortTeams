using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TextPortCore.Models;
using TextPortCore.Helpers;

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
                    DataBytes = null
                });
                msg.MMSFiles.Add(new MMSFile()
                {
                    FileName = "File2.jpg",
                    DataBytes = null
                });

                string json = new JavaScriptSerializer().Serialize(msg);
                string foo = json;
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
        }
    }
}