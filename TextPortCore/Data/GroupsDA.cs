using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {

        #region "Select Methods"

        public List<Group> GetGroupsForAccount(int accountId)
        {
            try
            {
                return _context.Groups.Where(x => x.AccountId == accountId).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("GroupsDA.GetGroupsForAccount", ex);
            }
            return null;
        }

        public List<GroupMember> GetMembersForGroup(int groupId)
        {
            try
            {
                return _context.GroupMembers.Where(x => x.GroupId == groupId).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("GroupsDA.GetMembersForGroup", ex);
            }
            return null;
        }

        public List<SelectListItem> GetGroupsList(int accountId)
        {
            List<SelectListItem> groupsList = new List<SelectListItem>();
            try
            {
                foreach (Group group in _context.Groups.Where(x => x.AccountId == accountId).ToList())
                {
                    SelectListItem listItem = new SelectListItem();

                    listItem.Text = group.GroupName;
                    listItem.Value = group.GroupId.ToString();

                    groupsList.Add(listItem);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("GroupsDA.GetGroupsList", ex);
            }
            return groupsList;
        }

        #endregion

        #region "Insert Methods"

        public bool AddGroup(Group group)
        {
            try
            {
                _context.Groups.Add(group);
                _context.SaveChanges();

                int foo = group.GroupId;

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("GroupsDA.AddGroup", ex);
            }
            return false;
        }

        public bool AddGroupMember(GroupMember newMember)
        {
            try
            {
                _context.GroupMembers.Add(newMember);
                _context.SaveChanges();

                int foo = newMember.GroupMemberId;

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("GroupsDA.AddGroup", ex);
            }
            return false;
        }

        #endregion

        #region "Delete Methods"

        public bool DeleteGroupMember(int groupMemberId)
        {
            try
            {
                // Need to check if the group member is part of the current context. If it is, detach it first before deleting.
                GroupMember localGm = _context.Set<GroupMember>().Local.FirstOrDefault(x => x.GroupMemberId == groupMemberId);
                if (localGm != null)
                {
                    _context.Entry(localGm).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
                else
                {
                    localGm = _context.GroupMembers.FirstOrDefault(x => x.GroupMemberId == groupMemberId);
                }
                _context.GroupMembers.Remove(localGm);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("GroupsDA.DeleteMember", ex);
            }
            return false;
        }

        #endregion

    }
}
