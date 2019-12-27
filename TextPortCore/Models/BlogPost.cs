using System;

namespace TextPortCore.Models
{
    public partial class BlogPost
    {
        public int PostId { get; set; }

        public bool Active { get; set; }

        public string UrlName { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string PostedBy { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public string Introduction { get; set; }

        public string ImageUrl { get; set; }

        public string PostContent { get; set; }


        public BlogPost()
        {
            this.PostId = 0;
            this.Active = true;
            this.UrlName = string.Empty;
            this.Title = string.Empty;
            this.Date = DateTime.MinValue;
            this.PostedBy = string.Empty;
            this.Description = string.Empty;
            this.Keywords = string.Empty;
            this.Introduction = string.Empty;
            this.ImageUrl = string.Empty;
            this.PostContent = string.Empty;
        }
    }
}
