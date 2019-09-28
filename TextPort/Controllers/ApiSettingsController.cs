using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class ApiSettingsController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            ApiApplicationsContainer ac = new ApiApplicationsContainer(accountId, 0, 0);

            return View(ac);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Add(int id)
        {
            try
            {
                APIApplication blankApp = new APIApplication(Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current));
                return PartialView("_ApplicationDetails", blankApp);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(APIApplication newApplication)
        {
            try
            {
                int newAppId = 0;
                using (TextPortDA da = new TextPortDA())
                {
                    newApplication.APIToken = $"{newApplication.AccountId}-{RandomString.GenerateRandomToken(8)}";
                    newApplication.APISecret = RandomString.GenerateRandomToken(12);
                    newAppId = da.SaveAPIApplication(newApplication);
                }

                return View("Index", new ApiApplicationsContainer(newApplication.AccountId, newAppId, 0));
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpGet]
        public string GenerateNewSecret(int id)
        {
            try
            {
                string newKey = RandomString.GenerateRandomToken(20);
                if (id > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        da.UpdateAPISecret(id, newKey);
                    }
                }
                return newKey;
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(APIApplication apiApp)
        {
            try
            {
                int savedAppId = 0;
                using (TextPortDA da = new TextPortDA())
                {
                    savedAppId = da.SaveAPIApplication(apiApp);
                }

                // Add slight delay for spintiller effect
                System.Threading.Thread.Sleep(500);

                return PartialView("_FullApplication", new ApiApplicationsContainer(apiApp.AccountId, savedAppId, 0));
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteApp(APIApplication apiApp)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    da.DeleteAPIApplication(apiApp);
                }
                ModelState.Clear();

                return PartialView("_FullApplication", new ApiApplicationsContainer(apiApp.AccountId, -1, 0));
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetAppDetails(int applicationId)
        {
            try
            {
                APIApplication application = new APIApplication();
                using (TextPortDA da = new TextPortDA())
                {
                    application = da.GetAPIApplicationById(applicationId);
                }
                return PartialView("_ApplicationDetails", application);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }
    }
}