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

            ContactsContainer cc = new ContactsContainer(accountId);

            return View(cc);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddContact(int id)
        {
            try
            {
                Contact newContact = new Contact()
                {
                    AccountId = id,
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
        public ActionResult AddMember(Contact newContact)
        {
            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

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

        [Authorize]
        [HttpPost]
        public ActionResult DeleteMember(DeleteGroupMemberRequest request)
        {
            try
            {
                List<GroupMember> groupMembers = new List<GroupMember>();
                using (TextPortDA da = new TextPortDA())
                {
                    da.DeleteGroupMember(request.MemberId);
                    groupMembers = da.GetMembersForGroup(request.GroupId);
                }

                return PartialView("_MembersList", groupMembers);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetMembers(int id)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    return PartialView("_MembersList", da.GetMembersForGroup(id));
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }
    }
}