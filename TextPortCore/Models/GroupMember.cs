using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class GroupMember
    {
        public int GroupMemberId { get; set; }

        public int GroupId { get; set; }

        [Required(ErrorMessage = "A phone number is required")]
        [Display(Name = "Phone Number")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "A name is required")]
        [Display(Name = "Contact Name")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string MemberName { get; set; }

        public int CarrierId { get; set; }
    }
}
