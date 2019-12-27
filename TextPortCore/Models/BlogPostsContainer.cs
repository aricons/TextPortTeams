using System;
using System.Collections.Generic;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public partial class BlogPostsContainer
    {
        public int Page { get; set; }
        public int PostsPerPage { get; set; }
        public int PageCount { get; set; }

        public List<BlogPost> Posts { get; set; }


        public BlogPostsContainer()
        {
            this.Page = 1;
            this.PostsPerPage = 3;
            this.PageCount = 1;
            this.Posts = new List<BlogPost>();
        }

        public BlogPostsContainer(int pageNumber, int postsPerPage)
        {
            this.Page = pageNumber;
            this.PostsPerPage = postsPerPage;
            this.PageCount = 0;

            using (TextPortDA da = new TextPortDA())
            {
                int totalPosts = 0;
                this.Posts = da.GetPostsList(pageNumber, postsPerPage, ref totalPosts);

                if (totalPosts > 0 && this.PostsPerPage > 0)
                {
                    float pageCount = (float)totalPosts / (float)this.PostsPerPage;
                    this.PageCount = (int)pageCount;
                    if (pageCount > this.PageCount)
                    {
                        this.PageCount++;
                    }
                    if (this.PageCount == 0)
                    {
                        this.PageCount = 1;
                    }
                }
            }
        }
    }
}
