using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class ContactsController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
            string role = Utilities.GetRoleFromClaim(ClaimsPrincipal.Current);

            ContactsContainer cc = new ContactsContainer(branchId, accountId, role);

            return View(cc);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetContactsForBranch(int id)
        {
            try
            {
                int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
                if (id > 0)
                {
                    branchId = id;
                }

                List<Contact> contacts = new List<Contact>();
                using (TextPortDA da = new TextPortDA())
                {
                    contacts = da.GetContactsForBranch(branchId);
                }

                return PartialView("_ContactsList", contacts);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Add(int id)
        {
            try
            {
                int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
                string role = Utilities.GetRoleFromClaim(ClaimsPrincipal.Current);

                if (id > 0)
                {
                    branchId = id;
                }

                Contact newContact = new Contact()
                {
                    BranchId = branchId,
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

        [Authorize]
        [HttpPost]
        public ActionResult Add(Contact newContact)
        {
            try
            {
                newContact.DateAdded = DateTime.Now;

                List<Contact> contacts = new List<Contact>();

                using (TextPortDA da = new TextPortDA())
                {
                    da.AddContact(newContact);

                    contacts = da.GetContactsForBranch(newContact.BranchId);
                }

                return PartialView("_ContactsList", contacts);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                Contact contact = null;
                int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);

                using (TextPortDA da = new TextPortDA())
                {
                    contact = da.GetContactByContactId(branchId, id);
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

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Contact contact)
        {
            try
            {
                List<Contact> contacts = new List<Contact>();

                using (TextPortDA da = new TextPortDA())
                {
                    contact.BranchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
                    contact.MobileNumber = Utilities.NumberToE164(contact.MobileNumber, "1");
                    da.UpdateContact(contact);

                    contacts = da.GetContactsForBranch(contact.BranchId);
                }

                return PartialView("_ContactsList", contacts);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
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
                    contact.BranchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
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

        [Authorize]
        [HttpPost]
        public ActionResult Apply(Contact contact)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    contact.BranchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
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

        [Authorize]
        [HttpPost]
        public ActionResult DeleteContact(DeleteContactRequest request)
        {
            try
            {
                int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);

                List<Contact> contacts = new List<Contact>();

                using (TextPortDA da = new TextPortDA())
                {
                    da.DeleteContact(request.ContactId);

                    contacts = da.GetContactsForBranch(branchId);
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