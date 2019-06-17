using System;
using System.Linq;
using System.Collections.Generic;

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

        #endregion

    }
}
