using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    public class TrialController : Controller
    {
        // GET: Trial
        public ActionResult Index()
        {
            try
            {
                RegistrationData rd = new RegistrationData("FreeTrial", 0);
                return View(rd);
            }
            catch (Exception ex)
            {
                string foo = "error";
            }
            return null;
        }
    }
}