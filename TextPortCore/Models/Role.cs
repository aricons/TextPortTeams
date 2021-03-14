using System;

namespace TextPortCore.Models
{
    public partial class Role
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }


        public Role()
        {
            this.RoleId = 0;
            this.RoleName = string.Empty;
        }
    }
}
