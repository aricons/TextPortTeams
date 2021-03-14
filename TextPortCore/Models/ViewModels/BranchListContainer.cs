using System.Collections.Generic;

using TextPortCore.Data;
using TextPortCore.Models;

namespace TextPortCore.ViewModels
{
    public class BranchListContainer
    {
        public int PageCount { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int RecordCount { get; set; }
        public int LowRecord { get; set; }
        public int HighRecord { get; set; }
        public string SortOrder { get; set; }
        public string RecordLabel
        {
            get
            {
                return $"Items {this.LowRecord} to {this.HighRecord} of {this.RecordCount}";
            }
        }

        public List<Branch> BranchList { get; set; }

        // Constructors
        public BranchListContainer()
        {
            this.RecordCount = 0;
            this.PageCount = 0;
            this.RecordsPerPage = 0;
            this.CurrentPage = 1;
            this.LowRecord = 0;
            this.HighRecord = 0;
            this.SortOrder = "desc";
            using (TextPortDA da = new TextPortDA())
            {
                this.BranchList = new List<Branch>();
            }
        }
    }
}