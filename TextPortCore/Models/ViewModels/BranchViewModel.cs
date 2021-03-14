using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Models;

namespace TextPortCore.ViewModels
{
    public class BranchViewModel
    {
        [Display(Name = "BranchId")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "A branch name is required")]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "An address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "A city is required")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "A state is required")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "A zip is required")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Manager Name")]
        public string Manager { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public List<SelectListItem> StatesList { get; set; }

        public BranchViewModel()
        {
            using (TextPortDA da = new TextPortDA())
            {
                this.StatesList = da.GetStatesForDropDown();
            }
        }

        public BranchViewModel(int branchId)
        {
            using (TextPortDA da = new TextPortDA())
            {
                Branch branch = da.GetBranchByBranchId(branchId);
                if (branch != null)
                {
                    this.BranchId = branch.BranchId;
                    this.BranchName = branch.BranchName;
                    this.Address = branch.Address;
                    this.City = branch.City;
                    this.State = branch.State;
                    this.Zip = branch.Zip;
                    this.Phone = branch.Phone;
                    this.Manager = branch.Manager;
                    this.Notes = branch.Notes;
                };

                this.StatesList = da.GetStatesForDropDown();
            }
        }
    }
}
