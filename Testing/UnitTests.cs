using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Models.PayPal;
using TextPortCore.Helpers;
using TextPortCore.Integrations;
using TextPortCore.Integrations.AWS.SQS;

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
            string decryptedPassword = AESEncryptDecrypt.Decrypt("1≠ù∑¸", Constants.RC4Key);
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

        [TestMethod]
        public void SubmitPayPalPayment()
        {
            PurchaseDetail pd = new PurchaseDetail();

            pd.intent = "sale";
            pd.redirect_urls = new RedirectUrls()
            {
                cancel_url = "textport.com",
                return_url = "textport.com"
            };
        }

        [TestMethod]
        public void GetRecentContacts()
        {
            //List<Recent> recents = MessagesDA.GetRecentMessagesForAccount(1);
            //var bar = recents;
        }

        [TestMethod]
        public void SendAWSSQSMessage()
        {
            try
            {
                AWSSQS awssqs = new AWSSQS();
                awssqs.SendPosition("1234", "5678");

                //Task<string> foo = AWSSQS.SendMessage("This is message number 1");
                //string bar = foo.Result;
                //AWSSQS.SendPosition("1234", "5678");
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
        }
    }
}
