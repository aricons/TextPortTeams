using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public class GroupsDA
    {
        private readonly TextPortContext _context;

        public GroupsDA(TextPortContext context)
        {
            this._context = context;
        }

        #region "Select Methods"
        public List<Group> GetGroupsForAccount(int accountId)
        {
            try
            {
                return _context.Groups.Where(x => x.AccountId == accountId).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("GroupsDA.GetGroupsForAccount", ex);
            }
            return null;
        }

        #endregion

    }
}
