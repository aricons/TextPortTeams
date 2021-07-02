using System;

namespace TextPortCore.Models
{
    public partial class Branch
    {
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public int TimeZoneId { get; set; }

        public string Manager { get; set; }

        public string Notes { get; set; }

        public Branch()
        {
            this.BranchId = 0;
            this.BranchName = string.Empty;
            this.Address = string.Empty;
            this.City = string.Empty;
            this.State = string.Empty;
            this.Zip = string.Empty;
            this.Phone = string.Empty;
            this.Manager = string.Empty;
            this.Notes = string.Empty;
        }
    }
}
