using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortTeams.Controllers
{
    public class ContactsController : Controller
    {
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            ContactsContainer cc = new ContactsContainer(accountId);

            return View(cc);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Add()
        {
            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

                Contact newContact = new Contact()
                {
                    AccountId = accountId,
                    Name = string.Empty,
                    MobileNumber = string.Empty
                };
                return PartialView("_AddContact", newContact);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Add(Contact newContact)
        {
            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                newContact.AccountId = accountId;
                newContact.DateAdded = DateTime.Now;

                List<Contact> contacts = new List<Contact>();

                using (TextPortDA da = new TextPortDA())
                {
                    da.AddContact(newContact);

                    contacts = da.GetContactsForAccount(accountId);
                }

                return PartialView("_ContactsList", contacts);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                Contact contact = null;
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

                using (TextPortDA da = new TextPortDA())
                {
                    contact = da.GetContactByContactId(accountId, id);
                    contact.MobileNumber = Utilities.NumberToBandwidthFormat(contact.MobileNumber);
                }

                return PartialView("_EditContact", contact);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Edit(Contact contact)
        {
            try
            {
                List<Contact> contacts = new List<Contact>();

                using (TextPortDA da = new TextPortDA())
                {
                    contact.AccountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                    contact.MobileNumber = Utilities.NumberToE164(contact.MobileNumber, "1");
                    da.UpdateContact(contact);

                    contacts = da.GetContactsForAccount(contact.AccountId);
                }

                return PartialView("_ContactsList", contacts);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Apply(int id, string number)
        {
            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                Contact contact = new Contact();

                if (id > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        contact = da.GetContactByContactId(accountId, id);
                    }
                }
                else
                {
                    contact.AccountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                    contact.Name = string.Empty;
                    contact.MobileNumber = number;
                };

                return PartialView("_ApplyContact", contact);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Apply(Contact contact)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    //contact.MobileNumber = contact.MobileNumber;
                    contact.AccountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                    da.AddOrReplaceContact(contact);
                }
                return null;
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult DeleteContact(DeleteContactRequest request)
        {
            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

                List<Contact> contacts = new List<Contact>();

                using (TextPortDA da = new TextPortDA())
                {
                    da.DeleteContact(request.ContactId);

                    contacts = da.GetContactsForAccount(accountId);
                }

                return PartialView("_ContactsList", contacts);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        public class DeleteContactRequest
        {
            public int ContactId { get; set; }
        }
    }
}