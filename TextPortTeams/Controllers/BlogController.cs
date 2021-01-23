using System.Web.Mvc;
using System.Collections.Generic;

using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortTeams.Controllers
{
    public class BlogController : Controller
    {
        public const int postsToDisplayPerPage = 4;

        public ActionResult Index(string page)
        {
            int pageNumber = (!string.IsNullOrEmpty(page)) ? Conversion.StringToIntOrZero(page) : 1;
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }

            BlogPostsContainer posts = new BlogPostsContainer(pageNumber, postsToDisplayPerPage);
            return View(posts);
        }

        public ActionResult Article(string id)
        {
            using (TextPortDA da = new TextPortDA())
            {
                BlogPost blogPost = da.GetPostByUrlName(id);
                if (blogPost != null)
                {
                    return View(blogPost);
                }
            }

            return View(new BlogPost());
        }

        public ActionResult Draft()
        {
            return View();
        }
    }
}