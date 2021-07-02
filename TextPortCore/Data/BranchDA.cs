using System;
using System.Linq;
using System.Collections.Generic;

using TextPortCore.Models;
using TextPortCore.ViewModels;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public Branch GetBranchByBranchId(int branchId)
        {
            try
            {
                return _context.Branches.FirstOrDefault(x => x.BranchId == branchId);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BranchDA.GetBranchByBranchId", ex);
            }

            return null;
        }

        public List<Branch> GetAllBranches()
        {
            try
            {
                return _context.Branches.OrderBy(x => x.BranchName).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BranchDA.GetAllBranches", ex);
            }

            return null;
        }

        public List<Branch> GetBranchesForIds(List<int> branchIds)
        {
            try
            {
                return _context.Branches.Where(x => branchIds.Contains(x.BranchId)).OrderBy(x => x.BranchName).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BranchDA.GetBranchesForIds", ex);
            }

            return null;
        }

        #endregion

        #region "Update Methods"

        public bool UpdateBranch(BranchViewModel bvm)
        {
            try
            {
                if (bvm != null)
                {
                    Branch dbRecord = _context.Branches.FirstOrDefault(x => x.BranchId == bvm.BranchId);
                    if (dbRecord != null)
                    {
                        dbRecord.BranchName = bvm.BranchName;
                        dbRecord.TimeZoneId = bvm.TimeZoneId;
                        dbRecord.Address = bvm.Address;
                        dbRecord.City = bvm.City;
                        dbRecord.State = bvm.State;
                        dbRecord.Zip = bvm.Zip;
                        dbRecord.Phone = bvm.Phone;
                        dbRecord.Manager = bvm.Manager;
                        dbRecord.Notes = bvm.Notes;

                        _context.Branches.Update(dbRecord);

                        this.SaveChanges();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BranchDA.UpdateBranch", ex);
            }

            return false;
        }

        #endregion

        #region "Insert Methods"

        public int AddBranch(BranchViewModel bvm)
        {
            try
            {
                Branch newBranch = new Branch();

                newBranch.BranchName = bvm.BranchName;
                newBranch.TimeZoneId = bvm.TimeZoneId;
                newBranch.Address = bvm.Address;
                newBranch.City = bvm.City;
                newBranch.State = bvm.State;
                newBranch.Zip = bvm.Zip;
                newBranch.Phone = bvm.Phone;
                newBranch.Manager = bvm.Manager;
                newBranch.Notes = bvm.Notes;

                _context.Branches.Add(newBranch);

                _context.SaveChanges();

                return newBranch.BranchId;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BranchDA.AddBranch", ex);
            }

            return 0;
        }

        #endregion

        #region "Delete Methods"
        #endregion

    }
}
