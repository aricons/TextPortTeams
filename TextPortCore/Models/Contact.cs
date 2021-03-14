using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class Contact
    {
        public int? ContactId { get; set; }

        public int BranchId { get; set; }

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        [Required(ErrorMessage = "A name is required")]
        [Display(Name = "Contact Name")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "A number is required")]
        [Display(Name = "Phone Number")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string MobileNumber { get; set; }
    }
}
