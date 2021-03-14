using System;

namespace TextPortCore.Models
{
    public class User
    {
        public int AccountId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int BranchId { get; set; }

        public string BranchIds { get; set; }

        public int BranchCount
        {
            get
            {
                if (!string.IsNullOrEmpty(this.BranchIds))
                {
                    return this.BranchIds.Split(',').Length;
                }
                return 1;
            }
        }

        public string BranchName { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public User() { }
    }
}
