using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class EmailToSMSController : Controller
    {
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            EmailToSMSContainer etsc = new EmailToSMSContainer(accountId);

            return View(etsc);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult AddAddress()
        {
            try
            {
                EmailToSMSContainer newEmailToSmsContainer = new EmailToSMSContainer(Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current));

                return PartialView("_AddAddress", newEmailToSmsContainer);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult AddAddress(EmailToSMSAddress newAddress)
        {
            try
            {
                if (newAddress.AccountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        da.AddEmailToSmsAddress(newAddress);
                    }

                    return PartialView("_AddressList", new EmailToSMSContainer(newAddress.AccountId));
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }

            return null;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult UpdateAddress(EmailToSMSAddress address)
        {
            try
            {
                if (address.AccountId > 0 && address.AddressId > 0)
                {
                    List<EmailToSMSAddress> emailToSMSAddresses = new List<EmailToSMSAddress>();

                    using (TextPortDA da = new TextPortDA())
                    {
                        da.UpdataEmailToSMSAddress(address);
                    }

                    return PartialView("_AddressList", new EmailToSMSContainer(address.AccountId));
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult DeleteAddress(DeleteEmailToSMSAddressRequest deleteRequest)
        {
            try
            {
                if (deleteRequest.AccountId > 0 && deleteRequest.AddressId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        da.DeleteEmailToSMSAddress(deleteRequest.AddressId);
                    }

                    return PartialView("_AddressList", new EmailToSMSContainer(deleteRequest.AccountId));
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }

            return null;
        }
    }
}