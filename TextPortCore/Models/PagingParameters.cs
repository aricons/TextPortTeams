namespace TextPortCore.Models
{
    public class PagingParameters
    {
        public string Operation { get; set; }
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public int PreviousRecordsPerPage { get; set; }
        public byte Filter { get; set; }
        public byte PrevFilter { get; set; }
        public string SortBy { get; set; }
        public string PrevSortBy { get; set; }
        public string SortOrder { get; set; }

        public PagingParameters()
        {
            this.Operation = "page";
            this.Page = 1;
            this.RecordsPerPage = 10;
            this.PreviousRecordsPerPage = 10;
            this.Filter = 1;
            this.PrevFilter = 1;
            this.SortBy = "TimeStamp";
            this.PrevSortBy = string.Empty;
            this.SortOrder = "desc";
        }
    }
}
