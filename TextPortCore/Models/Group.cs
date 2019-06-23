using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class Group
    {
        public int GroupId { get; set; }

        public int AccountId { get; set; }

        [Required(ErrorMessage = "A group name is required")]
        [Display(Name = "Group Name")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string GroupName { get; set; }

        public List<GroupMember> Members { get; set; }

        // Constructors
        public Group()
        {
            this.GroupId = 0;
            this.AccountId = 0;
            this.GroupName = string.Empty;
            this.Members = new List<GroupMember>();
        }
    }
}
