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
    public class EmailToSMSController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            EmailToSMSContainer etsc = new EmailToSMSContainer(accountId);

            return View(etsc);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddAddress()
        {
            try
            {
                EmailToSMSContainer newEmailToSmsContainer = new EmailToSMSContainer()
                {
                    AccountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current)
                };

                return PartialView("_AddAddress", newEmailToSmsContainer);
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
        public ActionResult AddAddress(Group newGroup)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    da.AddGroup(newGroup);
                }

                return View("Index", new GroupsContainer(newGroup));
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateAddress(EmailToSMSAddress address)
        {
            try
            {
                //int groupId = newMember.GroupId;
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                List<EmailToSMSAddress> emailToSMSAddresses = new List<EmailToSMSAddress>();

                using (TextPortDA da = new TextPortDA())
                {
                    da.UpdataEmailToSMSAddress(address);
                    emailToSMSAddresses = da.GetEmailToSMSAddressesForAccount(accountId);
                }

                return PartialView("_AddressList", emailToSMSAddresses);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }
    }
}