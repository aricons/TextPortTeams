using System;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Models.IPData;
using TextPortCore.Helpers;
using TextPortCore.Integrations.IPData;

namespace TextPort.Controllers
{
    public class ToolsController : Controller
    {
        [ActionName("number-lookup")]
        public ActionResult NumberLookup(string n)
        {
            n = Utilities.StripLeading1(Utilities.StripNumber(n));
            if (!string.IsNullOrEmpty(n) && n.Length >= 7)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    NumberLookupResult lookupResult = new NumberLookupResult();
                    lookupResult = da.LookupNumber(n);
                    return View(lookupResult);
                }
            }
            return View(new NumberLookupResult());
        }

        [HttpPost]
        [ActionName("number-lookup")]
        [ValidateAntiForgeryToken]
        public ActionResult NumberLookup(NumberLookupResult res)
        {
            string phoneNumber = Utilities.StripNumber(res.Number);
            NumberLookupResult lookupResult = new NumberLookupResult();

            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length >= 7)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    lookupResult = da.LookupNumber(phoneNumber);
                }
            }
            return View(lookupResult);
        }

        [ActionName("ip-address-lookup")]
        public ActionResult IpAddressLookup()
        {
            return View(new IPDataResult());
        }

        [HttpPost]
        [ActionName("ip-address-lookup")]
        [ValidateAntiForgeryToken]
        public ActionResult IpAddressLookup(IPDataResult res)
        {
            if (!string.IsNullOrEmpty(res.ip))
            {
                using (IPData ipd = new IPData())
                {
                    res = ipd.LookupIP(res.ip);
                }
            }
            return View(res);
        }

        //[HttpPost]
        //public ActionResult ExecNumberLookup(string phoneNumber)
        //{
        //    phoneNumber = Utilities.StripNumber(phoneNumber);
        //    NumberLookupResult lookupResult = new NumberLookupResult();

        //    if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length >= 7)
        //    {
        //        using (TextPortDA da = new TextPortDA())
        //        {
        //            lookupResult = da.LookupNumber(phoneNumber);
        //        }
        //    }
        //    return PartialView("_NumberLookupResult", lookupResult);
        //}
    }
}