using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Nexmo;
using TextPortServices.Processes;

namespace Testing
{
    [TestClass]
    public class NexmoTests
    {

        [TestMethod]
        public void GetNexmoNumbersForCountry()
        {
            string countryCode = "GB";

            using (Nexmo nexmo = new Nexmo())
            {
                List<string> numbers = nexmo.GetVirtualNumbersList(44, 10, 1);
                var foo = numbers;
            }
        }
    }
}
