using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

using Dapper;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.ViewModels;

namespace TextPort.Controllers
{
    public class BranchesController : Controller
    {
        public readonly TextPortDA da = new TextPortDA();
        public readonly TextPortContext _context = new TextPortContext();

        [Authorize]
        public ActionResult Index()
        {
            BranchListContainer branchesContainer = new BranchListContainer();
            return View(branchesContainer);
        }

        [Authorize]
        [HttpGet]
        [ActionName("add-branch")]
        public ActionResult Add()
        {
            BranchViewModel bvm = new BranchViewModel();
            return PartialView("_AddBranch", bvm);
        }

        [Authorize]
        [HttpPost]
        [ActionName("add-branch")]
        public ActionResult Add(BranchViewModel bvm)
        {
            try
            {
                int newBranchId = da.AddBranch(bvm);

                return View("Index", new BranchListContainer());
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [Authorize]
        [HttpGet]
        [ActionName("edit-branch")]
        public ActionResult Edit(int id)
        {
            BranchViewModel bvm = new BranchViewModel(id);
            return PartialView("_EditBranch", bvm);
        }

        [Authorize]
        [HttpPost]
        [ActionName("edit-branch")]
        public ActionResult Edit(BranchViewModel bvm)
        {
            try
            {
                da.UpdateBranch(bvm);

                return View("Index", new BranchListContainer());
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("delete-branch")]
        public ActionResult Delete(BranchViewModel bvm)
        {
            try
            {
                //da.DeleteAccount(bvm);

                return View("Index", new UserListContainer());
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [HttpPost]
        [ActionName("get-User-details")]
        public ActionResult GetUserDetails(int id)
        {
            Account results = _context.Accounts.FirstOrDefault(x => x.AccountId == id);
            return Json(results);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetBranchPage(PagingParameters parameters)
        {
            BranchListContainer branchesContainer = GetDataFromDatabase(parameters);

            return Json(new
            {
                page = branchesContainer.CurrentPage,
                recordsPerPage = branchesContainer.RecordsPerPage,
                pageCount = branchesContainer.PageCount,
                recordLabel = branchesContainer.RecordLabel,
                sortOrder = branchesContainer.SortOrder,
                html = renderRazorViewToString("_BranchList", branchesContainer)
            });
        }

        public BranchListContainer GetDataFromDatabase(PagingParameters pp)
        {
            int resultsCount = 0;
            int page = pp.Page;
            int recordsPerPage = pp.RecordsPerPage;
            int previousRecordsPerPage = pp.PreviousRecordsPerPage;
            string sortBy = pp.SortBy;
            string sortOrder = pp.SortOrder;

            string connectionString = ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString;
            List<Branch> results = new List<Branch>();

            try
            {
                string filter = string.Empty;
                string sqlBase = "SELECT B.BranchId, B.BranchName, B.Address, B.City, B.State, B.Zip, B.Phone, B.Manager, B.Notes FROM Branches B ";

                // Apply filters
                if (!string.IsNullOrEmpty(pp.SearchString))
                {
                    pp.SearchString = pp.SearchString.ToLower();
                    filter += (!string.IsNullOrEmpty(filter)) ? " AND " : "WHERE ";
                    filter += $" (CHARINDEX('{pp.SearchString}', A.{pp.SearchBy}) > 0) ";
                }

                if (pp.Operation == "sort")
                {
                    if (pp.SortBy == pp.PrevSortBy)
                    {
                        sortOrder = (sortOrder == "asc") ? "desc" : "asc";
                    }
                }

                BranchListContainer branchListContainer = new BranchListContainer();
                branchListContainer.RecordsPerPage = recordsPerPage;
                branchListContainer.SortOrder = sortOrder;

                // Paging
                string sort = $"ORDER BY B.{sortBy} {sortOrder} ";
                string paging = $"OFFSET {(pp.Page - 1) * recordsPerPage} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY";

                // Build final query
                string query = $"{sqlBase} {filter} {sort} {paging}";

                using (var connection = new SqlConnection(connectionString))
                {
                    resultsCount = connection.ExecuteScalar<int>($"SELECT COUNT(B.BranchId) FROM Branches B {filter}");

                    results = connection.Query<Branch>(query).ToList();
                }

                branchListContainer.RecordCount = resultsCount;

                if ((previousRecordsPerPage > 0 && recordsPerPage != previousRecordsPerPage)) // || (prevFilterBy != filterBy))
                {
                    int firstRecord = page * previousRecordsPerPage;
                    if (firstRecord > branchListContainer.RecordCount)
                    {
                        firstRecord = branchListContainer.RecordCount;
                    }

                    for (int x = 1; x <= firstRecord; x++)
                    {
                        if (x * recordsPerPage >= firstRecord)
                        {
                            page = x;
                            x += firstRecord;
                        }
                    }
                }

                branchListContainer.CurrentPage = pp.Page;
                branchListContainer.LowRecord = (pp.RecordsPerPage * (pp.Page - 1)) + 1;
                branchListContainer.HighRecord = branchListContainer.LowRecord + pp.RecordsPerPage - 1;
                branchListContainer.BranchList = results;

                if (branchListContainer.HighRecord > branchListContainer.RecordCount)
                {
                    branchListContainer.HighRecord = branchListContainer.RecordCount;
                }

                if (branchListContainer.RecordCount > 0 && branchListContainer.RecordsPerPage > 0)
                {
                    float pageCount = (float)branchListContainer.RecordCount / (float)branchListContainer.RecordsPerPage;
                    branchListContainer.PageCount = (int)pageCount;
                    if (pageCount - branchListContainer.PageCount > 0)
                    {
                        branchListContainer.PageCount++;
                    }
                }

                // Need to return at least 1 as the page count to prevent twbsPagination from erroring.
                if (branchListContainer.PageCount < 1)
                    branchListContainer.PageCount = 1;

                return branchListContainer;
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }

            return null;
        }

        //public UserSearchResults PerformUserSearch(UserSearchParams sp)
        //{
        //    int recordsPerPage = 50;

        //    string connectionString = ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString;
        //    UserSearchResults results = new UserSearchResults()
        //    {
        //        items = new List<UserSearchResult>()
        //    };

        //    List<UserSearchResult> resultItems = new List<UserSearchResult>();

        //    try
        //    {
        //        string filter = string.Empty;
        //        string sqlBase = "SELECT A.AccountId AS id, A.FirstName + ' ' + A.LastName AS text FROM Accounts A ";

        //        // Apply filters
        //        if (!string.IsNullOrEmpty(sp.term))
        //        {
        //            sp.term = sp.term.ToLower();
        //            filter += (!string.IsNullOrEmpty(filter)) ? " AND " : "WHERE ";
        //            filter += $" (CHARINDEX('{sp.term}', A.FirstName) > 0) ";
        //        }

        //        UserSearchResults searchResults = new UserSearchResults();

        //        // Paging
        //        string sort = $"ORDER BY A.FirstName ASC ";
        //        string paging = $"OFFSET {(sp.page - 1) * recordsPerPage} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY";

        //        // Build final query
        //        string query = $"{sqlBase} {filter} {sort} {paging}";

        //        using (var connection = new SqlConnection(connectionString))
        //        {
        //            resultItems = connection.Query<UserSearchResult>(query).ToList();
        //        }

        //        results.items = resultItems;

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        string foo = ex.Message;
        //    }

        //    return null;
        //}

        private string renderRazorViewToString(string viewName, object model)
        {
            try
            {
                ViewData.Model = model;
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
                return ex.Message;
            }
        }
    }

    //public class BranchListParam
    //{
    //    public int BranchId { get; set; }
    //}

    //public class BranchSearchParams
    //{
    //    public string term { get; set; }
    //    public int page { get; set; }
    //}

    //public class BranchSearchResults
    //{
    //    public List<UserSearchResult> items { get; set; }
    //}

    //public class BranchSearchResult
    //{
    //    public string id { get; set; }
    //    public string text { get; set; }
    //}

}