using System;
using System.Collections.Generic;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public partial class BlogRecentPosts
    {
        public List<BlogPost> Posts { get; set; }


        public BlogRecentPosts()
        {
            this.Posts = new List<BlogPost>();
        }

        public BlogRecentPosts(int postCount)
        {
            using (TextPortDA da = new TextPortDA())
            {
                this.Posts = da.GetRecentPosts(postCount);
            }
        }
    }
}
