using System;
using System.Linq;
using System.Collections.Generic;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public BlogPost GetPostByUrlName(string urlName)
        {
            try
            {
                return _context.BlogPosts.Where(x => x.UrlName == urlName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BlogDA.GetPostByUrlName", ex);
            }

            return null;
        }

        public List<BlogPost> GetPostsList(int pageNumber, int postsPerPage, ref int totalPosts)
        {
            totalPosts = 0;
            try
            {
                totalPosts = _context.BlogPosts.Where(x => x.Active == true).Count();

                return _context.BlogPosts.Where(x => x.Active == true).OrderByDescending(x => x.Date).Skip(postsPerPage * (pageNumber - 1)).Take(postsPerPage).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BlogDA.GetPostsList", ex);
            }

            return null;
        }

        public List<BlogPost> GetRecentPosts(int postCount)
        {
            try
            {
                return (from post in _context.BlogPosts
                        select new BlogPost()
                        {
                            PostId = post.PostId,
                            Title = post.Title,
                            Date = post.Date,
                            UrlName = post.UrlName,
                        }).OrderByDescending(x => x.Date).Take(postCount).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BlogDA.GetRecentPosts", ex);
            }

            return null;
        }

        #endregion

        #region "Insert Methods"
        #endregion

        #region "Delete Methods"
        #endregion

    }
}
