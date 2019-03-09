using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.Rendering;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    public class NumbersController : Controller
    {
        private readonly TextPortContext _context;

        public NumbersController(TextPortContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ConfirmPurchase(RegistrationData regData)
        {
            try
            {
                return PartialView("_Purchase", regData);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetAreaCodeName(string areaCode)
        {
            using (TextPortDA da = new TextPortDA(_context))
            {
                return Json(da.GetAreaCodeName(areaCode));
            }
        }

        public JsonResult GetAvailableNumbers(string areaCode)
        {
            List<SelectListItem> numbersItems = new List<SelectListItem>();

            using (Bandwidth bw = new Bandwidth(_context))
            {
                List<string> numbers = bw.GetVirtualNumbersList(areaCode);
                if (numbers.Any())
                {
                    foreach (string number in numbers)
                    {
                        numbersItems.Add(new SelectListItem() { Text = number, Value = number });
                    }
                }
                else
                {
                    numbersItems.Add(new SelectListItem() { Text = "No available numbers", Value = string.Empty });
                }
            }

            return Json(numbersItems);
        }
    }
}